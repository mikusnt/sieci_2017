/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/* 
 * File:   server_table.h
 * Author: ms-lin
 *
 * Created on 3 stycznia 2018, 15:18
 */

#ifndef SERVER_TABLE_H
#define SERVER_TABLE_H

#ifdef __cplusplus
extern "C" {
#endif

#include "io.h"
  
#define shared_addr 12345
#define defaultMaxPing 200
    
typedef struct {
    // 1 - watek aktywny
    // 0 - watek wylaczony
    // -1 - zadanie wylaczenia watku przez inny proces
    // -2 - watek zakonczyl swoje dzialanie po wylaczeniu
    int state;
    // wyzerowany pierwszy znak oznacza ze rekord pusty
    char userName[usernamePathBufferSize];
    FILE *file;
    pthread_mutex_t mutex;
    // 0 - oczekiwanie na zezwolenie
    // 1 - zezwolenie na wyslanie danych
    unsigned char newInfoFlag;
    short maxPing;
    char clientIP[IPBufferSize];
} UserIndex;    
    
typedef struct {
    UserIndex index[maxServerDomainIndex + 1];
    unsigned int newIndex;
} ServerTable; 

pthread_mutex_t serverTableMutex;

/* inicjalizacja struktury - zerowanie wszystkich bajtow w userNames i newIndex, inicjalizacja mutexow*/
void ServerTableInit(ServerTable *t);
/* wyszukuje indeks na podstawie danego loginu
   return indeks, w przypadku bledu -1*/
char GetIndexFromUserName(char *userName, ServerTable *t);
/* dodaje okreslone dane uzytkownika do tablicy, index_out zawiera indeks nowego elementu 
   return 0 gdy znaleziono wolne miejsce i wstawiono, -1 gdy blad*/
int InsertUserName(char *userName, FILE **file, int childProcess, char *index_out, ServerTable *t);
/* oznacza rekord jako pusty poprzez wyzerowanie pola userName
   return 0 gdy wyczysci, -1 w przypadku bledu */
int RemoveUserName(int index, ServerTable *t);
/*  wypisuje do konsoli strukture ServerTable, dopuki nie natrafi na pusty rekord */
void PrintServerTable(ServerTable *t);
/* wypisuje do konsoli pierwsze n indeksow ze struktury ServerTable, niezaleznie od tego czy puste*/
void PrintServerTableSomeIndex(ServerTable *t, unsigned char number);
/* inicjalizuje dostep do ServerTable jako do pamieci wspoldzielonej */
ServerTable *ConnectSharedTable();
/* odlacza wskaznik od pamieci wspoldzielonej dostepu do ServerTable */
void RemoveSharedTable();


#ifdef __cplusplus
}
#endif

#endif /* SERVER_TABLE_H */

