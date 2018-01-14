/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/* 
 * File:   data_out.h
 * Author: ms-lin
 *
 * Created on 7 stycznia 2018, 11:12
 */

#ifndef DATA_OUT_H
#define DATA_OUT_H

#ifdef __cplusplus
extern "C" {
#endif

#include "io.h"
    
#define inOutBufferSize 90
// przy zmianie ponizszych danych nalezy zmienic wartosc endInOutBuffer
#define endInOutBufferByte 0xFF
#define endInOutBufferSize 3

// sekwencja bajtow oznaczajaca koniec bufora
extern const unsigned int endInOutBuffer; //= (unsigned int)(0xFFFFFFFF);

// kody liczbowe rozkazow przyjmowanych i wykonywanych przez serwer
typedef enum {
    eMaxPing = 0x98,
    eConnection = 0x99,
    eLogin = 0xA0,
    eLogout = 0xA1,
    eServerData = 0xC0,
    eDeleteServer = 0xB0,
    eDeleteAllServers = 0xB1,
    eAddServer = 0xB2
} OrderCode; 

/*  pakuje potrzebne dane na podstawie kodu rozkazu do bufora wyjsciowego 
    return ilosc spakowanych bajtow po kompresji, wliczajac bajty zakonczenia*/
int CompressData(unsigned char *outBuffer, OrderCode outCode, char orderResponse, ServerInfo *serverInfo);
/*  rozpakowuje potrzebne dane na podstawie kodu rozkazu z bufora wyjsciowego 
    return ilosc odebranych bajtow po dekompresji, bez bajtow zakonczenia */
int DecompressData(unsigned char *inBuffer, OrderCode *inCode, char * userName, char *userIndex, char *serverDomain, unsigned short *port, short *maxPing);
/*  wypisuje do konsoli rozpakowane dane bufora wyjsciowego */
void PrintDecompressedData(OrderCode inCode, char *userName, char *serverIndex, char *serverDomain, unsigned short port, short maxPing);
/*  wypisuje do konsoli cala zawartosc bufora wyjsciowego w formacie hex */
void PrintInOutBuffer(unsigned char *outBuffer);





#ifdef __cplusplus
}
#endif

#endif /* DATA_OUT_H */

