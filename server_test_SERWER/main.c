/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/* 
 * File:   main.c
 * Author: ms-lin
 *
 * Created on 27 grudnia 2017, 11:54
 */

#include "all_includes.h"
#include "thread_functions.h"


/*

	Funkcja glowna zawierajaca poczatkowy kod serwera w tym oczekiwanie na nowe dane

*/

#define maxPing 2000

// glowna funkcja serwera
int MainServer();

void childEnd(int signo) {
    wait(NULL);
    printf("Koniec poprzedniego procesu serwera\n");
}

/*
 * 
 */
int main(int argc, char** argv) {
    //return Test_io();
    return MainServer();
    //return Test_server_table();
}

/* mutex operations
 pthread_mutex_t	mutex = PTHREAD_MUTEX_INITIALIZER;
 pthread_mutex_lock(&mutex);
 pthread_mutex_unlock(&mutex);
 pthread_mutex_destroy(&mutex);
 */


int MainServer() {
    signal(SIGCHLD, childEnd);
    changemode(1);

    // dane sieciowe
    int fd, fdx;
    int maxClients = 5;
    struct sockaddr_in serverAddr;
    struct sockaddr_in serverStorage;
    ServiceClientStruct serverClientStruct;
    pthread_t serviceClientThread;
    pthread_t quitThread;
    pthread_t enableClientInfoThread;

    // segment pamieci wspoldzielonej czyli tablica uzytkownikow
    serverTable = ConnectSharedTable();
    ServerTableInit(serverTable);
    //tempId = 
    int quitId = shmget(quitMemoryId, sizeof (int), 0660 | IPC_CREAT);
    quit = shmat(quitId, NULL, 0);
    *quit = 0;
    
    //pthread_create(&quitThread, NULL, Quit, NULL);
    pthread_create(&quitThread, NULL, Quit, NULL);
    pthread_create(&enableClientInfoThread, NULL, EnableClientInfoRepeated, NULL);

    // konfiguracja do polaczenia obslugi polecen
    serverAddr.sin_family = PF_INET;
    serverAddr.sin_port = htons(networkPort);
    serverAddr.sin_addr.s_addr = INADDR_ANY;
    fd = socket(PF_INET, SOCK_STREAM, 0);
    int on = 1;
    setsockopt(fd, SOL_SOCKET, SO_REUSEADDR, (char*) &on, sizeof (on));

    bind(fd, (struct sockaddr*) (&serverAddr), sizeof (serverAddr));
    listen(fd, maxClients);
    // petla glownego watku odpowiedzialnego za odczekiwanie na polecenia od klientow
    printf("---\nWcisnij 'q' aby zakonczyc dzialanie po obsluzeniu klienta.\n---\n");
    DateTime();
    printf("Oczekiwanie na dane...\n");
    while (1) {
        // obsluga zamkniecia serwera
        while (*quit == 1) { usleep(delayMsQuit * 1000); }
        // ustawiane przez watek ServerClient gdy wcisnieto klawisz i ServerClient
        // zakonczone
        if (*quit == 255) break;

        int size = sizeof (serverStorage);
        // uzyskanie polaczenia
        fdx = accept(fd, (struct sockaddr*) (&serverStorage), &size);
        serverClientStruct.fdx = fdx;
        serverClientStruct.serverStorage = serverStorage;
        pthread_create(&serviceClientThread, NULL, ServiceClient, (void*)&serverClientStruct);
        
        //close(fdx);
        // obsluga danych od klientow przez proces potomny
    }
    shmctl(quitId, IPC_RMID, NULL);
    close(fd);
    RemoveSharedTable(serverTable);
    return EXIT_SUCCESS;
}



