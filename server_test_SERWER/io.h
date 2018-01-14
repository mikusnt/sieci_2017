/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/* 
 * File:   io.h
 * Author: ms-lin
 *
 * Created on 27 grudnia 2017, 11:55
 */

#ifndef IO_H
#define IO_H

#ifdef __cplusplus
extern "C" {
#endif
    
#include "all_includes.h"
   
   

#define serverDomainBufferSize 64
#define userNameBufferSize 16
#define usernamePathBufferSize 32
#define lineFileBufferSize 75
    
#define maxServerDomainIndex 127
#define IPBufferSize 16
   
/* struktura przechowujaca dane o stanie serwera */
// rozmiar struktury to 70 bajtow
typedef struct { 
    // indeks serwera jako nr linii z pliku uzytkownika, -1 gdy koniec pliku
    // 1 bajt
    char index;
    // nr portu
    // 2 bajty
    unsigned short port;
    // URL serwera
    // 64 bajty
    char name[serverDomainBufferSize];
    // >= 0 - connected, -1 - not connected
    // 2 bajty
    short int ping;
} ServerInfoStruct;

typedef union {
    ServerInfoStruct structure;
    unsigned char bytes[sizeof(ServerInfoStruct)];
} ServerInfo;

/* drukuje do konsoli aktualna date i czas bez znaku nowej linii */
void DateTime();
/* sprawdza czy istnieje plik danego uzytkownika, jesli tak zwraca ilosc serwerow 
 * w pliku i deskryptor pliku, w przeciwnym wypadku zwraca -1*/
int OpenUser(char *username, FILE **file_out);
/* zamyka deskryptor pliku danego uzytkownika
 return 0 w przypadku powodzenia, -1 w przeciwnym wypadku*/
int CloseUser(FILE **file);
/* dodaje serwer o okreslonej sciezce i porcie na ostatnia pozycje otwartego pliku
 return >= 0 jako ilosc dodanych znakow, -1 gdy pusty deskryptor*/
int AddServer(char *serverName, unsigned short int port, FILE **file, char *userName);
/*zwraca skrukture z informacja o funkconowaniu serwera o konkretnej pozycji
 * z otwartego pliku*/
ServerInfo TestServer(int *serverNr, FILE *file, unsigned short int timeMs);
/* oznacza jako usuniety wpis serwera (okreslony wiersz od 0) z otwartego pliku
   return >= 0 jako nr usunietego indeksu, -1 w przypadku bledu*/
int RemoveServer(int serverIndex, FILE **file, char *userName);
/* oznacza jako usuniety wszystkie wpisy serwera otwartego pliku
   return >= 0 jako ilosc usunietych, -1 w przypadku bledu*/
int RemoveAllServers(FILE **file, char *userName);
/* tworzy plik txt uzytkownika
 return 0 w przypadku dodania, -1 gdy niepowodzenie*/
int AddUser(char *userName);
/* Usuwa plik txt wskazanego uzytkownika, plik uzytkownika powinien byc zamkniety
 return 0 w przypadku usuniecie, kod bledu gdy niepowodzenie*/
int RemoveUser(char *userName);
/* przeskok do poczatku otwartego pliku w celu sekwencyjnego odczytu linia po linii
   return 0 w przypadku powodzenia, inaczej -1, serverNr_out - polozenie kursora jako nr 
   linii od 0, ladowany w przypadku powodzenia */
int ToZeroIndex(int *serverNr_out, FILE *file);
/* inicjalizacja struktury ServerInfo poprzez zerowanie calej struktury */
void InitServerInfo(ServerInfo *s);
/* wypisuje do konsoli strukture Serverinfo jako ciag bajtow heksadecymalnych*/
void PrintServerInfoHex(ServerInfo *s);
/* wypisuje  do konsoli pola struktury ServerInfo linia po linii */
void PrintServerInfo(ServerInfo *s);

int ReopenFile(FILE **file, char *userName);

#ifdef __cplusplus
}
#endif

#endif /* IO_H */

