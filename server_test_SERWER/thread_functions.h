/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/* 
 * File:   thread_functions.h
 * Author: ms-lin
 *
 * Created on 9 stycznia 2018, 18:03
 */

#ifndef THREAD_FUNCTIONS_H
#define THREAD_FUNCTIONS_H

#ifdef __cplusplus
extern "C" {
#endif

#include "all_includes.h"
#include "io.h"
#include "data_out.h"
#include "server_table.h"
#include "custom_read_write.h"

    // idektyfikator pamieci zmiennej wspoldzielonej quit
#define quitMemoryId 99999 
    // ilosc sekond interwalu czasowego pomiedzy cyklicznym wysylanim danych
    // odpytywanych serwerow do klienta
#define secondsToNewInfo 10
    // sztuczna przerwa dzialania watku ServiceClient w kodzie ServerData
#define delayMSSendingInfo 10
    // sztuczna przerwa podczas oczekiwania na zamkniecie
#define delayMsQuit 10
    // port komunikacji miedzy klientem a serwerem
#define networkPort 1234
    
    // wskaznik pamieci wspoldzielonej
ServerTable *serverTable;
// 0 - dzialanie serwera, 1 - zadanie zamkniecia, 255 - zezwolenie na zamkniecie przez watek potomny
unsigned char *quit;

// skruktura przechowujaca argumenty wywolania ServiceClient
typedef struct {
    int fdx;
    struct sockaddr_in serverStorage;
    
} ServiceClientStruct;

/*  wymaga uruchomienia z argumentem 1 aby kbhit dzialalo*/
void changemode(int dir);
/*  return !=0 gdy wcisniety jakis klawisz, 0 w przeciwnym wypadku*/
int kbhit(void);
/*  obsluga watka realizacji polecen od klienta, konczy sie po obsluzeniu, wyjatkiem 
 * eServerData ktory wykonuje sie  dopoki zalogowany klient*/ 
void *ServiceClient(void* ServiceClientStructPointer);
/*  obsluga wysylania cyklicznych danych do klienta, konczy sie gdy childProcess w userTable ustawiony na 0 */
/*  ustawia 1 dla zmiennej wspoldzielonej quit gdy chociaz raz przycisniety klawisz zamkniecia serwera*/
void *Quit();
/*  jednorazowo ustawia flage uzytkownikow w serverTable zezwalajac na ponowne wyslanie danych do klienta,
 zaklada blokade dostepu do ServerTable*/
void EnableClientInfo(ServerTable *sT);
/*  cyklicznie ustawia flage uzytkownikow w serverTable zezwalajac na ponowne wyslanie danych do klienta 
 * co interwal czasowy, zaklada blokade dostepu do ServerTable*/
void *EnableClientInfoRepeated();
    
#ifdef __cplusplus
}
#endif

#endif /* THREAD_FUNCTIONS_H */

