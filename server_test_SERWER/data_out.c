/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

#include "data_out.h"

// sekwencja bajtow oznaczajaca koniec bufora
const unsigned int endInOutBuffer = (unsigned int)(0xFFFFFFF0);

/*  pakuje potrzebne dane na podstawie kodu rozkazu do bufora wyjsciowego 
    return ilosc spakowanych bajtow po kompresji, wliczajac bajty zakonczenia*/
int CompressData(unsigned char *outBuffer, OrderCode outCode, char orderResponse, ServerInfo *serverInfo) {
    memset(outBuffer, 0, inOutBufferSize);
    outBuffer[0] = outCode;
    if (outCode == eConnection) {
        memcpy(outBuffer + 1, &endInOutBuffer, sizeof(endInOutBuffer));
        return 1 + endInOutBufferSize;
    } else {
        if (outCode == eServerData) {
            memcpy(outBuffer + 1, serverInfo->bytes, sizeof(ServerInfo));
            memcpy(outBuffer + sizeof(ServerInfo) + 1, &endInOutBuffer, sizeof(endInOutBuffer));
            return 1 + sizeof(ServerInfo) + endInOutBufferSize;
        } else {
            outBuffer[1] = orderResponse;
            memcpy(outBuffer + 2, &endInOutBuffer, sizeof(endInOutBuffer));
            return 2 + endInOutBufferSize;
        }
    }   
}

/*  rozpakowuje potrzebne dane na podstawie kodu rozkazu z bufora wyjsciowego 
    return ilosc odebranych bajtow po dekompresji, bez bajtow zakonczenia */
int DecompressData(unsigned char *inBuffer, OrderCode *inCode, char * userName, char *serverIndex, char *serverDomain, unsigned short *port, short *maxPing) {
    *inCode = inBuffer[0];
    if (*inCode == eConnection) return 1;
    memcpy(userName, inBuffer + 1, userNameBufferSize);
    if ((*inCode == eLogin) || (*inCode == eLogout) || (*inCode == eDeleteAllServers) || (*inCode == eServerData)) {
        return 1 + userNameBufferSize;
    }else if (*inCode == eMaxPing) {
        memcpy(maxPing, inBuffer + 1 + userNameBufferSize, sizeof(short));
        return 1 + userNameBufferSize + sizeof(short);
    } else  if (*inCode == eDeleteServer) {
        *serverIndex = inBuffer[1 + userNameBufferSize];
        return 1 + userNameBufferSize + 1;
    } else if (*inCode == eAddServer){
        memcpy(port, inBuffer + 1 + userNameBufferSize, sizeof(unsigned short));
        memcpy(serverDomain, inBuffer + 1 + userNameBufferSize + sizeof(unsigned short), serverDomainBufferSize);
        return 1 + userNameBufferSize + sizeof(unsigned short) + serverDomainBufferSize;
    } else {
        printf("niepoprawna wartosc polecenia '0x%02X' podczas dekompresji\n", (unsigned char)*inCode);
        return -1;
    }     
}

/*  wypisuje do konsoli rozpakowane dane bufora wyjsciowego */
void PrintDecompressedData(OrderCode inCode, char *userName, char *serverIndex, char *serverDomain, unsigned short port, short maxPing) {
    printf("Rozpakowane dane bufora we/wy:\n");
    printf("Kod rozkazu: 0x%02X\n", inCode);
    printf("Nazwa uzytkownika: %s\n", userName);
    printf("Indeks serwera: %d\n", *serverIndex);
    printf("Nazwa domenowa: %s\n", serverDomain);
    printf("Port: %hu\n", port);
    printf("Maksymalny ping: %hd\n", maxPing);
}

/*  wypisuje do konsoli cala zawartosc bufora wyjsciowego w formacie hex */
void PrintInOutBuffer(unsigned char *outBuffer) {
    char delta = 0;
    char endCount = 0;
    printf("Heksadecymalde dane bufora we/wy:\n");
    while ((delta < inOutBufferSize) && (endCount < endInOutBufferSize)) {
        printf("0x%02X ", (unsigned char)outBuffer[delta]);
        if (outBuffer[delta++] == endInOutBufferByte)
            endCount++;
        else 
            endCount = 0;
    }
    printf("\\EOB\n");
}