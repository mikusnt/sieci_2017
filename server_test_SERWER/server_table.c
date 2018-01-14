/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

#include "server_table.h"

/* zwieksza o 1 cykliczna pozycje w ServerTable*/
void IncrementIndex(ServerTable *t) {
    t->newIndex = (t->newIndex + 1) % maxServerDomainIndex;
}

/* wypisuje do konsoli zawartosc struktury ServerIndex, nie blokuje dostepu do serverTable*/
void PrintServerIndex(UserIndex *t) {
    if (t->userName[0] != 0) {
        printf("Proces potomny: %d\n", t->state);
        printf("Nazwa uzytkownika: %s\n", t->userName);
        printf("Wskaznik na deskryptor pliku: %p\n", (void*)t->file);
        printf("Maksymalny ping: %hd", t->maxPing);
        //printf("IP klienta: %s\n", t->clientIP);
        //printf("Mutex: %l\n", &mutex);
    } else {
        printf("pusty rekord\n");
    }
}
/* inicjalizacja struktury - zerowanie wszystkich bajtow w userNames i newIndex, inicjalizacja mutexow*/
void ServerTableInit(ServerTable *t) {
    memset(t, 0, sizeof(ServerTable));
    t->newIndex = 0;
    for (int i = 0; i <= maxServerDomainIndex; i++) {
        pthread_mutex_init(&(t->index[i].mutex), NULL);
        t->index[i].maxPing = defaultMaxPing;
    }
    pthread_mutex_init(&serverTableMutex, NULL);
}

/* wyszukuje indeks na podstawie danego loginu, zaklada blokade dostepu do serverTable
   return indeks, w przypadku bledu -1*/
char GetIndexFromUserName(char *userName, ServerTable *t) {
    int i;
    //pthread_mutex_lock(&serverTableMutex);
    for (i = 0; i <= maxServerDomainIndex; i++) {
        if (strcmp(userName, t->index[i].userName) == 0) {
            //pthread_mutex_unlock(&serverTableMutex);
            return i;
        }
    }
    //pthread_mutex_unlock(&serverTableMutex);
    if (i > maxServerDomainIndex) return -1;
}

/* dodaje okreslone dane uzytkownika do tablicy, index_out zawiera indeks nowego elementu,
 * nie zaklada blokady dostepu na serverTable
   return 0 gdy znaleziono wolne miejsce i wstawiono, -1 gdy blad*/
int InsertUserName(char *userName, FILE **file, int childProcess, char *index_out, ServerTable *t) {
    //unsigned short int newIndex;
    int i;
    //pthread_mutex_lock(&serverTableMutex);
    for (i = 0; i <= maxServerDomainIndex; i++) {
        if (t->index[t->newIndex].userName[0] == 0) {
            break;
        }
        IncrementIndex(t);
    }
    if (i <= maxServerDomainIndex) {
        strncpy(t->index[t->newIndex].userName, userName, userNameBufferSize);
        t->index[t->newIndex].file = *file;
        //t->index[t->newIndex].mutex = mutex;
        t->index[t->newIndex].state = childProcess;
        *index_out = t->newIndex;
        // wypelnienie danymi tablicy
        IncrementIndex(t);
        //pthread_mutex_unlock(&serverTableMutex);
        return 0;
    } else {
        //pthread_mutex_unlock(&serverTableMutex);
        return -1;
    }
}

/* oznacza rekord jako pusty poprzez wyzerowanie pola userName, zaklada blokade dostepu na serverTable
   return 0 gdy wyczysci, -1 w przypadku bledu */
int RemoveUserName(int index, ServerTable *t) {
    if ((index > maxServerDomainIndex) || (index < 0))
        return -1;
    else {
        //pthread_mutex_lock(&serverTableMutex);
        memset(t->index[index].userName, 0, sizeof(t->index[index].userName));
        //pthread_mutex_unlock(&serverTableMutex);
        return 0;
    }
}

/*  wypisuje do konsoli strukture ServerTable, dopoki nie natrafi na pusty rekord,
 nie zaklada blokady dpstepu na serverTable */
void PrintServerTable(ServerTable *t) {
    unsigned int i = 0;
    //pthread_mutex_lock(&serverTableMutex);
    while((t->index[i].userName[0] != 0) && (i <= maxServerDomainIndex)) {
        PrintServerIndex(&t->index[i++]);
        printf("\n");
    }
    //pthread_mutex_unlock(&serverTableMutex);
    if (i == 0) printf("pusta tablica\n");
    
}

/* wypisuje do konsoli pierwsze n indeksow ze struktury ServerTable, niezaleznie od tego czy puste,
  nie zaklada blokady dostepu na serverTable*/
void PrintServerTableSomeIndex(ServerTable *t, unsigned char number) {
    unsigned int i = 0;
    //pthread_mutex_lock(&serverTableMutex);
    for (i = 0; i < number; i++) {
        PrintServerIndex(&t->index[i]);
        printf("\n");
    }
    //pthread_mutex_unlock(&serverTableMutex);
}
/* przylacza pamiec wspoldzielona serverTable do wskaznika, nie zaklada blokady na serverTable*/
ServerTable *ConnectSharedTable() {
    ServerTable *sharedTable;
    //DateTime();
    int sharedTableId = shmget(shared_addr, sizeof(ServerTable), 0660 | IPC_CREAT);
    sharedTable = shmat(sharedTableId, NULL, 0);
    return sharedTable;
}

/* usuwa z pamieci systemu pamiec wspoldzielona serverTable, nie zaklada blokady na serverTable*/
void RemoveSharedTable() {
    int sharedTableId = shmget(shared_addr, sizeof(ServerTable), 0660 | IPC_CREAT);
    shmctl(sharedTableId, IPC_RMID, NULL);
}