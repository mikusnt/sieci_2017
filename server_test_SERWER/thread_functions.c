/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

#include "thread_functions.h"

/*  wymaga uruchomienia z argumentem 1 aby kbhit dzialalo*/
void changemode(int dir) {
    static struct termios oldt, newt;

    if (dir == 1) {
        tcgetattr(STDIN_FILENO, &oldt);
        newt = oldt;
        newt.c_lflag &= ~(ICANON | ECHO);
        tcsetattr(STDIN_FILENO, TCSANOW, &newt);
    } else
        tcsetattr(STDIN_FILENO, TCSANOW, &oldt);
}

/*  return !=0 gdy wcisniety jakis klawisz, 0 w przeciwnym wypadku*/
int kbhit(void) {
    struct timeval tv;
    fd_set rdfs;

    tv.tv_sec = 0;
    tv.tv_usec = 0;

    FD_ZERO(&rdfs);
    FD_SET(STDIN_FILENO, &rdfs);

    select(STDIN_FILENO + 1, &rdfs, NULL, NULL, &tv);
    return FD_ISSET(STDIN_FILENO, &rdfs);

}

/*  obsluga watka realizacji polecen od klienta, konczy sie po obsluzeniu, wyjatkiem 
 * eServerData ktory wykonuje sie  dopoki zalogowany klient*/ 
void *ServiceClient(void* ServiceClientStructPointer) {
    ServiceClientStruct serviceClientStruct = *(ServiceClientStruct*)ServiceClientStructPointer;
    int fdx = serviceClientStruct.fdx;
    // pamiec wspoldzielona
    serverTable = ConnectSharedTable();
    int tempId = shmget(quitMemoryId, sizeof (int), 0660 | IPC_CREAT);
    quit = shmat(tempId, NULL, 0);

    // dane obslugi
    OrderCode orderCode;
    char userName[userNameBufferSize] = "NuLl";
    char serverIndex;
    char serverDomain[serverDomainBufferSize] = "NuLl";
    char *userIP;
    unsigned short userPort;
    short userMaxPing;
    ServerInfo serverInfo;
    int processId;
    pthread_t sendingToClientThread;

    unsigned char readBuffer[inOutBufferSize];
    unsigned char writeBuffer[inOutBufferSize]; // bufory odczytu i zapisu danych sieciowych
    memset(readBuffer, 0, sizeof (readBuffer));
    memset(writeBuffer, 0, sizeof (writeBuffer));

    // nalezy zmienic na funkcje odporna na maly rozmiar bufora
    //printf("before read\n");
    read(fdx, readBuffer, sizeof (readBuffer));
    //printf("after read\n");
    // wypisanie i rozlozenie danych od klienta na skladowe
    DecompressData(readBuffer, &orderCode, userName, &serverIndex, serverDomain, &userPort, &userMaxPing);
    if (DEBUG) {
        PrintInOutBuffer(readBuffer);
        printf("Otrzymano dane:\n");
        PrintDecompressedData(orderCode, userName, &serverIndex, serverDomain, userPort, userMaxPing);
    }
    
    
    userIP = inet_ntoa((struct in_addr)serviceClientStruct.serverStorage.sin_addr);

    char orderRes = 0;
    char userIndex;
    printf("Obsluga polecenia 0x%02X dla uzytkownika '%s'\n", orderCode, userName);
    if (orderCode != 0) {
        if ((orderCode != eLogin) && (orderCode != eConnection) && (orderCode != eMaxPing)) {
            // pozyskanie polozenia uzytkownika w tablicy uzytkownikow w przypadku uzytkownika po logowaniu
            //pthread_mutex_lock(&serverTableMutex);
            userIndex = GetIndexFromUserName(userName, serverTable);
            //pthread_mutex_unlock(&serverTableMutex);
            if (orderCode != eServerData) {
                // zablokuj dostep do pliku uzytkownika innym watkom w przypadku pracy na pliku
                if (userIndex < 0) {
                    DateTime();
                    printf("Opuszczenie semafora: brak wpisu w tablicy uzytkownikow dla uzytkownika '%s'\n", userName);
                    orderRes = -1;
                } else {
                    //pthread_mutex_lock(&serverTableMutex);
                    pthread_mutex_lock(&serverTable->index[userIndex].mutex);
                    //pthread_mutex_unlock(&serverTableMutex);
                     if (DEBUG) {
                         DateTime();
                         printf("Opuszczono semafor dla uzytkownika '%s'\n", userName);
                     }
                    
                }
            }
        }
    } else orderRes = -1;
    // gdy nie wystapil blad wyszukiwania uzytkownika w tablicy obsluz zadanie klienta
    //DateTime();
    if (orderRes == 0) {
        switch (orderCode) {
            case eMaxPing: {
                pthread_mutex_lock(&serverTableMutex);
                serverTable->index[userIndex].maxPing = userMaxPing;
                pthread_mutex_unlock(&serverTableMutex);
                EnableClientInfo(serverTable);
                printf("Zmieniono maksymalny ping na %hd dla uzytkownika '%s' od %s\n", userMaxPing, userName, userIP);
            } break;
                // nawiazanie proby polaczenia
            case eConnection:
            {
                // brak czynnosci, dane odpowiedzi takie same jak te od klienta
                printf("Obsluga testu polaczenia od %s\n", userIP);
            }
            break;
                // logowanie
            case eLogin:
            {
                //printf("userIndex: %d\n", userIndex);
                AddUser(userName);
                //pthread_mutex_lock(&serverTableMutex);
                orderRes = OpenUser(userName, &serverTable->index[userIndex].file);
                // zaladwanie danych do pamieci wspoldzielonej w przypadku braku bledu
                if (orderRes >= 0) {
                    orderRes = InsertUserName(userName, &serverTable->index[userIndex].file, 0, &userIndex, serverTable);
                }
                if (orderRes >= 0) {
                        serverTable->index[userIndex].state = 1;
                        //strncpy(serverTable->index[userIndex].clientIP, userIP, IPBufferSize);
                        //printf("%s\n", serverTable->index[userIndex].clientIP);
                        //pthread_create(&sendingToClientThread, NULL, SendingToClient, (void*)&userIndex);
                        orderRes = 0;
                        
                }
                //pthread_mutex_unlock(&serverTableMutex);
                //if (orderRes == 0) printf("Zalogowanie uzytkownika '%s' od %s\n", userName, userIP);
                //else printf("Blad zalogowania uzytkownika '%s' od %s\n", userName, userIP);
            }
            break;
            // zadanie danych serwerow ktore odblokowuje watek
            case eServerData:
            {
                if (orderRes == 0) {
                    pthread_mutex_lock(&serverTableMutex);
                    serverTable->index[userIndex].newInfoFlag = 1;
                    pthread_mutex_unlock(&serverTableMutex);
                    //printf("1\n");
                    while (serverTable->index[userIndex].state > 0) {
                        //printf("2\n");
                        // wyslanie danych do klienta gdy flaga > 0 
                        
                        if (serverTable->index[userIndex].newInfoFlag > 0) {
                            //printf("3\n");
                            DateTime();
                            printf("Test serwerow uzytkownika %s:\n", userName);
                            pthread_mutex_lock(&serverTable->index[userIndex].mutex);
                            pthread_mutex_lock(&serverTableMutex);
                            serverTable->index[userIndex].newInfoFlag = 0;
                            //userIndex = GetIndexFromUserName(userName, serverTable);
                            //if (index < 0) break;
                            //printf("4\n");
                            /*

                             Wlasciwa obsluga watku

                             */
                            //printf("w petli\n");
                            serverInfo.structure.index = 0;
                            int index;
                            ToZeroIndex(&index, serverTable->index[userIndex].file);
                            int workingServers = 0;
                            int allServers = 0;
                            while(1) {
                                allServers++;
                                serverInfo = TestServer(&index, serverTable->index[userIndex].file, serverTable->index[userIndex].maxPing);
                                if (serverInfo.structure.ping >= 0) workingServers++;
                                if (serverInfo.structure.index != -1) {
                                    CompressData(writeBuffer, eServerData, 0, &serverInfo);
                                    write(fdx, writeBuffer, sizeof (writeBuffer));
                                } else break;
                            } 
                            printf("   wszystkie: %d, dzialajace: %d\n", allServers, workingServers);
    //                        snprintf(serverinfo.structure.name, serverDomainBufferSize, "mikusnt09");
    //                        serverInfo.structure.index = 5;
    //                        serverInfo.structure.ping = 32;
    //                        serverInfo.structure.port = 81;

                            //CompressData(writeBuffer, eServerData, 0, &serverInfo);
                            //write(fdx, writeBuffer, sizeof (writeBuffer));

                            /*

                             Koniec wlasciwej obslugi watku

                             */
                            
                            pthread_mutex_unlock(&serverTableMutex);
                            pthread_mutex_unlock(&serverTable->index[userIndex].mutex);

                        } else {
                            
                            // wstrzymanie watku na 10ms
                            usleep(delayMSSendingInfo * 1000);
                        }
                        
                    }
                    // powinno byc -2
                    pthread_mutex_lock(&serverTableMutex);
                    serverTable->index[userIndex].state = -2;
                    pthread_mutex_unlock(&serverTableMutex);
                } else {
                    printf("Blad funkcji wysylania informacji uzytkownika '%s'do %s\n", userName, userIP);
                }
            }
            break;
                // wylogowanie
            case eLogout:
            {
                // nieblokujacy
                // zamyka proces potomny, deskryptor pliku i czysci tablice uzytkownikow
                if (serverTable->index[userIndex].state >= 0) {
                    pthread_mutex_lock(&serverTableMutex);
                    // zakonczenie procesu wysylania danych do klienta
                    serverTable->index[userIndex].state = -1;
                    pthread_mutex_unlock(&serverTableMutex);
                    // oczekiwanie na zamkniecie watku, nie nalezy blokowac
                    while (serverTable->index[userIndex].state != -2) {} 
                    // zamkniecie deskryptora pliku uzytkownika
                    CloseUser(&serverTable->index[userIndex].file);
                    // usuniecie uzytkownika z tablicy uzytkownikow
                    orderRes = RemoveUserName(userIndex, serverTable);
                    //PrintServerTable(serverTable);
                } else orderRes = -1;
                if (orderRes == 0) printf("Wylogowanie uzytkownika '%s' od %s\n", userName, userIP);
                else printf("Blad wylogowania uzytkownika '%s' od %s\n", userName, userIP);
            }
            break;
                // usuwanie pojedynczego serwera na podstawie indeksu
            case eDeleteServer:
            {
                //pthread_mutex_lock(&serverTableMutex);
                orderRes = (RemoveServer(serverIndex, &serverTable->index[userIndex].file, userName) >= 0) ? 0 : -1;
                //pthread_mutex_unlock(&serverTableMutex);
                if (orderRes == 0) {
                    EnableClientInfo(serverTable);
                    //printf("Usuniecie serwera o indeksie %d od uzytkownika '%s' od %s\n", (int)serverIndex, userName, userIP);
                }
                //else printf("Blad usuniecia serwera o indeksie %d od uzytkownika '%s' od %s\n", (int)serverIndex, userName, userIP);
            }
            break;
                // usuwanie wszystkich serwerow
            case eDeleteAllServers:
            {
                //pthread_mutex_lock(&serverTableMutex);
                orderRes = (RemoveAllServers(&serverTable->index[userIndex].file, userName) >= 0) ? 0 : -1;
                
                if (orderRes == 0) {
                    EnableClientInfo(serverTable);
                    //printf("Usuniecie wszystkich serwerow uzytkownika '%s' od %s\n", userName, userIP);
                }
                //else printf("Blad usuniecia wszystkich serwerow uzytkownika '%s' od %s\n", userName, userIP);
                //pthread_mutex_unlock(&serverTableMutex);
            }
            break;
                // dodanie serwera 
            case eAddServer:
            {
                //pthread_mutex_lock(&serverTableMutex);
                orderRes = (AddServer(serverDomain, userPort, &serverTable->index[userIndex].file, userName) >= 1) ? 0 : -1;
                
                if (orderRes == 0) {
                    EnableClientInfo(serverTable);
                    //printf("Dodanie serwera '%s' uzytkownika '%s' od %s\n", serverDomain, userName, userIP);
                }
                //else printf("Blad dodania serwera '%s' uzytkownika '%s' od %s\n", serverDomain, userName, userIP);
                //pthread_mutex_unlock(&serverTableMutex);
            }
            break;
            default:
            {
                orderRes = 1;
                printf("Niepoprawny kod rozkazu uzytkownika '%s' od %s\n", userName, userIP);
            }
            break;
        }
    } else {
        printf("Wycofanie obslugi uzytkownika '%s' od %s z powodu bledu\n", userName, userIP);
    }
    if (orderCode != 0) {
        // podnies mutex gdy zrealizowano polecenie klienta, a dostep do pliku zostal udzielony
        if ((orderCode != eLogin) && (orderCode != eConnection) && (orderCode != eMaxPing)) {
            
            if (orderCode != eServerData) {
                if (userIndex < 0)  {
                    DateTime();    
                    printf("Podniesienie semafora - brak wpisu w tablicy uzytkownikow dla uzytkownika %s\n", userName);
                    orderRes = -1;
                } else {
                    //pthread_mutex_lock(&serverTableMutex);
                    pthread_mutex_unlock(&serverTable->index[userIndex].mutex);
                    //pthread_mutex_unlock(&serverTableMutex);
                    if (DEBUG) {
                        DateTime();
                        printf("Podniesiono semafor dla uzytkownika '%s'\n", userName);
                    }
                }
            }
        }
        if (orderCode != eServerData) {
            orderRes = orderRes == 0 ? 0 : 1;
            CompressData(writeBuffer, orderCode, orderRes, &serverInfo);
            //DateTime();

            //DisconnectSharedTable(serverTable);
            // nalezy zmienic na funkcje odporna na maly rozmiar bufora
            write(fdx, writeBuffer, sizeof (writeBuffer));
            if (DEBUG) {
                printf("Wyslano dane:\n");
                PrintInOutBuffer(writeBuffer);
            }
        }
    } else {
        printf("Puste dane o kodzie 0x00\n");
    }
    //if (orderCode != eServerData)
    close(fdx);
    if (*quit == 0) {
        printf("---\nWcisnij 'q' aby zakonczyc dzialanie po obsluzeniu klienta.\n---\n");
        DateTime();
        printf("Oczekiwanie na dane...\n");
    } else {
        *quit = 255;
    }
    //shmdt(quit);
}

/*  obsluga wysylania cyklicznych danych do klienta, konczy sie gdy childProcess w userTable ustawiony na 0 */
/*void *SendingToClient(void* userIndexPointer) {
    char userIndex = *((char*)(userIndexPointer));
    serverTable = ConnectSharedTable();

//    struct sockaddr_in serverAddr;
//    struct sockaddr_in serverStorage;
//    
//    int fd, fdx;
      ServerInfo serverInfo;
      unsigned char writeBuffer[inOutBufferSize];
//    
//    serverAddr.sin_family = PF_INET;
//    serverAddr.sin_port = htons(networkPort);
//    serverAddr.sin_addr.s_addr = inet_addr(serverTable->index[userIndex].clientIP);
//    
//    fd = socket(PF_INET, SOCK_STREAM, 0);
//    int on = 1;
//    setsockopt(fd, SOL_SOCKET, SO_REUSEADDR, (char*)&on, sizeof(on));
//    
//    bind(fd, (struct sockaddr*)(&serverAddr), sizeof(serverAddr));
//    listen(fd, 1);
//    int size = sizeof(serverStorage);
//    printf("przed\n");
//    fdx = accept(fd, (struct sockaddr*)(&serverStorage), &size);
//    printf("po\n");
    // cykliczne wysylanie danych do lkienta dopoki zezwolenie na prace
    while(serverTable->index[userIndex].state > 0) {
        // wyslanie danych do klienta gdy flaga > 0 
        if (serverTable->index[userIndex].newInfoFlag > 0) {
            pthread_mutex_lock(&serverTable->index[userIndex].mutex);
            
             
             Wlasciwa obsluga watku
             
             
            sprintf(serverInfo.structure.name, "mikint09");
            serverInfo.structure.index = 5;
            serverInfo.structure.ping = 32;
            serverInfo.structure.port = 81;
            
            CompressData(writeBuffer, eServerData, 0, &serverInfo);
            
            //write(serverTable->index[userIndex].fdx, writeBuffer, sizeof(inOutBufferSize));
            printf("\n\nwysylanie danych\n\n");
            
            
             
             Koniec wlasciwej obslugi watku
             
             
            pthread_mutex_unlock(&serverTable->index[userIndex].mutex);
            serverTable->index[userIndex].newInfoFlag = 0;
        } else {
            // wstrzymanie watku na 10ms
            usleep(delayMSSendingInfo * 1000);
        }
    }
    //close(serverTable->index[userIndex].fdx);
    serverTable->index[userIndex].state  = -1;
    // DisconnectSharedTable(serverTable);
}*/


/*  ustawia 1 dla zmiennej wspoldzielonej quit gdy chociaz raz przycisniety klawisz zamkniecia serwera*/
void *Quit() {
    int tempId = shmget(quitMemoryId, sizeof (int), 0660 | IPC_CREAT);
    quit = shmat(tempId, NULL, 0);
    char znak;
    while(1) {
        if (kbhit()) {
            znak = getchar();
            if (znak == 'q') {
                printf("Przyjeto zadanie wylaczenia serwera\n");
                *quit = 1;
                break;
            }
        }
    }
    //shmctl(tempId, IPC_RMID, NULL);
    //RemoveSharedTable();
   // exit(0);
}

/*  jednorazowo ustawia flage uzytkownikow w serverTable zezwalajac na ponowne wyslanie danych do klienta,
 zaklada blokade dostepu do ServerTable*/
void EnableClientInfo(ServerTable *sT) {
    pthread_mutex_lock(&serverTableMutex);
    for (int i = 0; i <= maxServerDomainIndex; i++) {
        if ((sT->index[i].userName[0] != 0) && (serverTable->index[i].newInfoFlag == 0)) {
            sT->index[i].newInfoFlag = 1;
        }
    }
    pthread_mutex_unlock(&serverTableMutex);
}

/*  cyklicznie ustawia flage uzytkownikow w serverTable zezwalajac na ponowne wyslanie danych do klienta 
 * co interwal czasowy, zaklada blokade dostepu do ServerTable*/
void *EnableClientInfoRepeated() {
    serverTable = ConnectSharedTable();
    int count;
    while(1) {
        count = 0;
        pthread_mutex_lock(&serverTableMutex);
        for(int i = 0; i <= maxServerDomainIndex; i++) {
            if ((serverTable->index[i].userName[0] != 0)  && (serverTable->index[i].newInfoFlag == 0)) {
                serverTable->index[i].newInfoFlag = 1;
                count++;
            }
        }
        pthread_mutex_unlock(&serverTableMutex);
        //printf("Zezwolono na dzialanie %d watkom\n", count);
        sleep(secondsToNewInfo);
    }
}