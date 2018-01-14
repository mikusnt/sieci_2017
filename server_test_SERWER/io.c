/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

#include "io.h"

#define extension ".txt"
#define userFolder "usernames/"

/*
 
 * funkcje wewnetrzne
 
 */ 
/* drukuje do konsoli aktualna date i czas bez znaku nowej linii */
void DateTime() {
    time_t t = time(NULL);
    struct tm tm = *localtime(&t);
    printf("%d-%d-%d %d:%d:%d ", tm.tm_year + 1900, tm.tm_mon + 1, tm.tm_mday, tm.tm_hour, tm.tm_min, tm.tm_sec);
}

// generuje pelna nazwe pliku uzytkownika na podstawie nazwy uzytkownika
void createFilename(char *userName, char *filename, int filenameSize) {
    snprintf(filename, filenameSize, "%s%s%s", userFolder, userName, extension);
}

// zwraca adres ip jako tablice znakowa na podstawie nazwy hosta
int find_ip_address(char *hostname, char *ip_address) 
{  
      struct hostent *host_name;     
      struct in_addr **ipaddress;     
      int count;   
      
      if((host_name = gethostbyname(hostname)) == NULL)     
      { 
            DateTime();
            printf("Nie mozna ustalic adresu IP dla hosta '%s'\n", hostname);      
            return -1;
      }   
      else
      {  
            ipaddress = (struct in_addr **) host_name->h_addr_list;
            for(count = 0; ipaddress[count] != NULL; count++)
            {   
                  strcpy(ip_address, inet_ntoa(*ipaddress[count]));
                  if (DEBUG) {
                    DateTime();
                    printf("Znaleziono IP '%s' dla hosta '%s'\n", ip_address, hostname);
                  }
                  return 0;
            }
      }
      return -1;
}

/* sprawdza czy mozna sie polaczyc z serwerem o podanej nazwie
  return >= 0 co oznacza ping gdy uzyskano polaczenie, -1 gdy blad*/
short int tryConnectToServer(char *serverName, unsigned short int port, unsigned short int timeMs) {
    int nSocket;
    int nConnect;
    int nBytes, out = -1;
    struct sockaddr_in stServerAddr;
    char cbBuf[lineFileBufferSize];
    
    char ip[IPBufferSize];
    
    nSocket = socket(PF_INET, SOCK_STREAM, 0);
    if (nSocket == -1) {
        //printf("socket error\n");
        return -1;
    }
    
    int curflags = fcntl(nSocket, F_GETFL, 0);
    fcntl(nSocket, F_SETFL, curflags | O_NONBLOCK);
    //fcntl(nSocket, F_SETFL, O_NONBLOCK);
    
    memset(&stServerAddr, 0, sizeof(struct sockaddr));
    stServerAddr.sin_family = AF_INET;

    if (find_ip_address(serverName, ip) != 0) {
        return -1;
    }
    //printf("%s\n", ip);
    stServerAddr.sin_port = htons(port);
    stServerAddr.sin_addr.s_addr = inet_addr(ip);
    connect(nSocket, (struct sockaddr*)&stServerAddr, sizeof(stServerAddr));
    
    fd_set fdset;
    struct timeval tv;
    FD_ZERO(&fdset);
    FD_SET(nSocket, &fdset);
    tv.tv_sec = timeMs / 1000 ;             /* second timeout */
    tv.tv_usec = (timeMs % 1000) * 1000;             /* microseconds timeout */
    if (select(nSocket + 1, NULL, &fdset, NULL, &tv) == 1) {
        int so_error;
        socklen_t len = sizeof so_error;

        getsockopt(nSocket, SOL_SOCKET, SO_ERROR, &so_error, &len);

        if (so_error == 0) {
            //printf("\nis open, ping %d\n", ((timeMs * 1000) - tv.tv_usec) / 1000) ;
            out = ((timeMs * 1000) - tv.tv_usec) / 1000;
        }
    }
    //usleep(1000000);
//    if ( (out != -1) && (errno != EINPROGRESS ))
//           return -1;
    //printf("%d\n", out);
    fcntl(nSocket, F_SETFL, curflags);
    return out;
}

/*
 
 Z pliku h
 
 */

/* sprawdza czy istnieje plik danego uzytkownika, jesli tak zwraca ilosc serwerow 
 * w pliku i deskryptor pliku, w przeciwnym wypadku zwraca -1*/
int OpenUser(char *username, FILE **file_out) {
    char filename[usernamePathBufferSize];
    char buffer[lineFileBufferSize];
    int count = 0;
    mkdir(userFolder, 0766);
    createFilename(username, filename, sizeof(filename));
    //printf("%s\n", filename);
    FILE *file;
    *file_out = fopen(filename, "r+");
    //printf("pos-1\n");
    DateTime();
    if (*file_out == NULL) {
        printf("Brak pliku uzytkownika %s\n", username);
        return -1;
    }
    
    // procedura usuwania oznaczonych serwerow
    FILE *temp;
    char tempName[userNameBufferSize];
    char tempFileName[usernamePathBufferSize];
    int usuniete = 0;
    
    snprintf(tempName, sizeof(tempName), "%d", fileno(*file_out));
    //printf("pos-0.5\n");
    createFilename(tempName, tempFileName, sizeof(tempFileName));
    
    temp = fopen(tempFileName, "w");  
    //printf("pos0\n");
    while(fgets(buffer, sizeof(buffer), *file_out)) {
        if (buffer[0] == ' ') {
            fputs(buffer, temp); 
            
        } else usuniete++;
    }
    //printf("pos1\n");
    fclose(temp);
    fclose(*file_out);
    remove(filename);
    rename(tempFileName, filename);
    //printf("pos2\n");
    *file_out = fopen(filename, "r+");
    if (*file_out == NULL) {
        printf("Blad odczytu pliku uzytkownika %s po usuwaniu oznaczonych serwerow\n", username);
        return -1;
    }
    
    while(fgets(buffer, sizeof(buffer), *file_out)) {
        count++;
    }
    
    printf("Uzytkownik '%s' posiada %d serwerow, %d zostalo usunietych\n", username, count, usuniete);
    return count;
}

/* zamyka deskryptor pliku danego uzytkownika
 return 0 w przypadku powodzenia, -1 w przeciwnym wypadku*/
int CloseUser(FILE **file) {
    DateTime();
    if (*file == NULL) {
        printf("Pusty deskryptor pliku, nie mozna zamknac\n");
        return -1;
    }
    
    int out = fclose(*file);
    if (out == 0) {
        *file = NULL;
        printf("Zamknieto deskryptor pliku uzytkownika\n");
    } else {
        printf("Blad podczas zamykania deskryptora pliku o kodzie %d\n", out);
    }
    return out;
}

/* dodaje serwer o okreslonej sciezce na ostatnia pozycje otwartego pliku
 return >= 0 jako ilosc dodanych znakow, -1 gdy pusty deskryptor*/
int AddServer(char *serverName, unsigned short int port, FILE **file, char *userName) {
    char buffer[lineFileBufferSize];
    char addBuffer[lineFileBufferSize];
    char isInFile = 0;
    
    if (*file == NULL) {
        DateTime();
        printf("Pusty deskryptor pliku, nie mozna dodac serwera '%s' do uzytkownika '%s'\n", serverName, userName);
        return -1;
    }
    int out = fseek(*file, 0, SEEK_SET); 
    int charCount;
    if (out != 0) {
        DateTime();
        printf("Blad podczas zmiany pozycji pliku na poczatkowa o kodzie %d podczas dodawania serwera '%s' do uzytkownika '%s'\n", out, serverName, userName);
        return -1;
    } else {
        snprintf(addBuffer, lineFileBufferSize, " %s;%05d\n", serverName, port);
        while (fgets(buffer, sizeof(buffer), *file) && (!isInFile)) {
            //printf("'%s' '%s'\n", buffer, serverName );
            if (!strcmp(buffer, addBuffer)) isInFile = 1;
        }
        if (!isInFile) {
            charCount = fprintf(*file, "%s", addBuffer);
            if (ReopenFile(file, userName) != 0) 
                return -1;
            DateTime();
            printf("Dodano serwer o nazwie '%s' i porcie %hu do uzytkownika '%s'\n", serverName, port, userName);
            //printf("'%s'\n", addBuffer);
        } else {
            charCount = 0;
            DateTime();
            printf("Serwer o nazwie '%s' i porcie %hu jest juz w pliku uzytkownika '%s'\n", serverName, port, userName);
        }
        
    }

    return charCount;
}

/*zwraca skrukture z informacja o funkcjonowaniu serwera o konkretnej pozycji
 * z otwartego pliku*/
ServerInfo TestServer(int *serverNr, FILE *file, unsigned short int timeMs) {
    char buffer[usernamePathBufferSize];
    char kodPortu[usernamePathBufferSize];
    ServerInfo info;
    InitServerInfo(&info);
    if ((fgets(buffer, sizeof(buffer), file)) && (buffer[0] == ' ')) {
        char * adrSrednika = strchr(buffer, ';');
        int pozycja = adrSrednika - buffer;
        buffer[pozycja] = 0;
        strncpy(info.structure.name, buffer+1, pozycja);
        strncpy(kodPortu, adrSrednika+1, 5);
        info.structure.index = *serverNr;
        info.structure.port = atoi(kodPortu);
        
        info.structure.ping = tryConnectToServer(info.structure.name, info.structure.port, timeMs);
        //printf("%d\n", info.structure.ping);
        if (info.structure.ping >= 0) {
            if (DEBUG) {
                DateTime();
                printf("Uzyskano polaczenie do serwera '%s' o porcie %hu i indeksie %d, ping %hi\n", info.structure.name, info.structure.port, *serverNr, info.structure.ping);
            }
        } else {
            DateTime();
            printf("Brak polaczenia do serwera '%s' o porcie %hu i indeksie %d\n", info.structure.name, info.structure.port, *serverNr);
        }
        (*serverNr)++;
    } else {
        info.structure.index = -1;
        if (DEBUG) {
            DateTime();
            printf("Brak serwerow to sprawdzenia\n");
        }
        //return NULL;
    }
    return info;
}

/* oznacza jako usuniety wpis serwera (okreslony wiersz od 0) z otwartego pliku
   return >= 0 jako nr usunietego indeksu, -1 w przypadku bledu*/
int RemoveServer(int serverIndex, FILE **file, char *userName) {
    DateTime();
    if (*file == NULL) {
        printf("Pusty deskryptor pliku, nie mozna usunac serwera o indeksie %d uzytkownika '%s'\n", serverIndex, userName);
        return -1;
    }
    int count = 0;
    char buffer[lineFileBufferSize];
    
    fseek(*file, 0, SEEK_END); // seek to end of file
    int size = ftell(*file); // get current file pointer
    fseek(*file, 0, SEEK_SET); // seek back to beginning of file
    printf("\n\n%d\n\n", size);
    if (size == 0) {
        printf("Nie mozna usunac serwera gdyz plik uzytkownika '%s' pusty\n", userName);
        return -1;
    }
    while((count < serverIndex) && (fgets(buffer, sizeof(buffer), *file))) {
        count++;
    }
    if (count == serverIndex) {
        fputs("*", *file);
        if (ReopenFile(file, userName) != 0) {
            return -1;
        }
        printf("Usunieto serwer o indeksie %d uzytkownika '%s'\n", serverIndex, userName);
        return count;
    } else {
        printf("Nie mozna usunac serwera o indeksie %d uzytkownika '%s' gdyz jest poza plikiem\n", serverIndex, userName);
        return -1;
    }
}

/* oznacza jako usuniety wszystkie wpisy serwera otwartego pliku
   return >= 0 jako ilosc usunietych, -1 w przypadku bledu*/
int RemoveAllServers(FILE **file, char *userName) {
    DateTime();
    fpos_t rozmiar;
    fpos_t aktualny;
    DateTime();
    if (*file == NULL) {
        printf("Pusty deskryptor pliku, nie mozna usunac wszystkich serwerow uzytkownika '%s'\n", userName);
        return -1;
    }
    int count = 0;
    char buffer[lineFileBufferSize];
    fseek (*file, 0, SEEK_END); /* ustawiamy wskaźnik na koniec pliku */
    fgetpos (*file, &rozmiar);
    fseek(*file, 0, SEEK_SET);
    do {
        fgetpos (*file, &aktualny);
        if (aktualny.__pos < rozmiar.__pos) {
            count++;
            fputs("*", *file); 
        } else break;
    } while(fgets(buffer, sizeof(buffer), *file));
    if (ReopenFile(file, userName) != 0) {
        //printf("Nieokreslony blad ponownego otwarcia plikow uzytkownika '%s'\n", userName);
        return -1;  
    }
    printf("Usunieto wszystkie serwery uzytkownika '%s'\n", userName);      
    return count;
}

/* tworzy plik txt uzytkownika jesli nie istnieje
 return 0 w przypadku dodania, -1 gdy niepowodzenie*/
int AddUser(char *userName) {
    FILE *file;
    char filename[usernamePathBufferSize];
    createFilename(userName, filename, sizeof(filename));
    file = fopen(filename, "a");
    DateTime();
    if (file != NULL) {
        printf("Dodano/otwarto uzytkownika '%s'\n", userName);

        fclose(file);
        return 0;
    }
    printf("Blad podczas dodawania uzytkownika '%s'\n", userName);
    return -1;
}

/* Usuwa plik txt wskazanego uzytkownika, plik uzytkownika powinien byc zamkniety
 return 0 w przypadku usuniecie, kod bledu gdy niepowodzenie*/
int RemoveUser(char *userName) {
    char filename[usernamePathBufferSize];
    createFilename(userName, filename, sizeof(filename));
    //printf("%s", filename);
    int out = remove(filename);
    DateTime();
    if (out == 0) {
        printf("Usunieto uzytkownika %s\n", userName);
    } else printf("Blad podczas usuwania uzytkownika '%s' o kodzie %d\n", userName, out);
    return out;
}

/* przeskok do poczatku otwartego pliku w celu sekwencyjnego odczytu linia po linii
   return 0 w przypadku powodzenia, inaczej -1, serverNr_out - polozenie kursora jako nr 
   linii od 0, ladowany w przypadku powodzenia */
int ToZeroIndex(int *serverNr_out, FILE *file) {
    
    if (file == NULL) {
        DateTime();
        printf("Pusty deskryptor pliku, nie mozna przeskoczyc do poczatku pliku\n");
        return -1;
    }
    int out = fseek(file, 0, SEEK_SET);
    if (out == 0) {
        if (DEBUG) {
            DateTime();
            printf("Zmieniono pozycje pliku na poczatkowa\n");
        }
        *serverNr_out = 0;
    }
    else {
        DateTime();
        printf("Blad podczas zmiany pozycji pliku na poczatkowa o kodzie %d", out); 
    }
    return out;
}

/* inicjalizacja struktury ServerInfo poprzez zerowanie calej struktury */
void InitServerInfo(ServerInfo *s) {
    memset(s->bytes, 0, sizeof(ServerInfo));
}

/* wypisuje do konsoli strukture Serverinfo jako ciag bajtow heksadecymalnych*/
void PrintServerInfoHex(ServerInfo *s) {
    printf("Dane hex struktury ServerInfo:\n");
    for(int i = 0; i < sizeof(ServerInfo); i++) {
        printf("%02X ", (unsigned int)s->bytes[i]);
    }
    printf("\\EOB\n");
}

/* wypisuje  do konsoli pola struktury ServerInfo linia po linii */
void PrintServerInfo(ServerInfo *s) {
    printf("Dane struktury ServerInfo:\n");
    printf("Index: %d\n", (int)s->structure.index);
    printf("Port: %hu\n", s->structure.port);
    printf("Nazwa serwera: %s\n", s->structure.name);
    printf("Ping: %hd\n", s->structure.ping);
}

/* zamyka i ponownie otwiera wskazany plik z otwartym deskryptorem */
int ReopenFile(FILE **file, char *userName) {
    int out = 0;
    if (CloseUser(file) == 0) {
        if (OpenUser(userName, file) < 0)
            out = -1;
    } else
        out = -1;
    if (out != 0) {
        DateTime();
        printf("Blad ponownego otwarcia pliku uzytkownika '%s'\n", userName);
    }
    return out;
}