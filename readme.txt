Projekt przedmiotu Sieci Komputerowe II semestr, 2018r
Projekt nr 16 czyli cykliczne odpytywanie serwerow o adresach URL dla poszczegolnych uzytkownikow.
Podzial na dwa projekty: serwera i klienta

Serwer realizowany na Linux w C
Klient realizowany na Windows w C#

kod wykonania: 0 - poprawność
kod wykonania: -1 - błąd

Komunikacja klient <--> serwer:

Próba połączenia ('eConnection'):
-->
0x99 + 0xFFFFFFF0
Rozmiar: 1 + 4 = 5
<--
0x99 + 0xFFFFFFF0
Rozmiar: 1 + 4 = 5


Logowanie ('eLogin', AddUser, OpenUser, uruchomienie procesu potomnego z wysylaniem danych polaczenia):
-->
0xA0 + login + 0xFFFFFFF0
Rozmiar: 1 + 16 + 4 = 21
<--
0xA0 + kod wykonania + 0xFFFFFFF0
Rozmiar: 1 + 1 + 4 = 6


Wylogowanie ('eLogout', CloseUser, zamkniecie procesu potomnego):
-->
0xA1 + login + 0xFFFFFFF0
Rozmiar: 1 + 16 + 4 = 21
<--
0xA1 + kod wykonania + 0xFFFFFFF0
Rozmiar: 1 + 1 + 4 = 6


Zmiana maksymalnego pingu ('eMaxPing')
-->
0x98 + login + ping + 0xFFFFFF00
Rozmiar: 1 + 16 + 2 + 4 = 23
<--
0x98 + kod wykonania + 0xFFFFFFF0
Rozmiar: 1 + 1 + 4 = 6


Dane polaczenia ('eServerData', ToZeroIndex, TestServer dopoki nie koniec pliku):
-->
0xC0 + login + 0xFFFFFFF0
Rozmiar: 1 + 16 + 4 = 21
<--
0xC0 + serverInfo.bytes + 0xFFFFFFF0
Rozmiar: 1 + 70 + 4 = 75


Usuwanie serwera po indeksie ('eRemoveServer'):
-->
0xB0 + login + id wpisu + 0xFFFFFFF0
Rozmiar: 1 + 16 + 1 + 4 = 22
<-- 
0xB0 + kod wykonania + 0xFFFFFFF0
Rozmiar: 1 + 1 + 4 = 6

Usuwanie wszystkich serwerow ('eRemoveAllServers'):
-->
0xB1 + login + 0xFFFFFFF0
Rozmiar: 1 + 16 + 4 = 21
<--
0xB1 + kod wykonania + 0xFFFFFFF0
Rozmiar: 1 + 1 + 4 = 6


Dodawanie serwera na ostatnia pozycje ('esAddServer'):
-->
0xB2 + login + port + nazwa domenowa + 0xFFFFFFF0
Rozmiar: 1 + 16 + 2 + 64 + 4 = 87
<-- 
0xB2 + kod wykonania + 0xFFFFFFF0
Rozmiar: 1 + 1 + 4 = 6
