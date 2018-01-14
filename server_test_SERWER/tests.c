/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

#include "tests.h"

int Test_io() {
    char username[userNameBufferSize] = "pierwszy";
    AddUser(username);
    int index;
    FILE *file;
    ServerInfo info;
    printf("%lu\n", sizeof(info));
    OpenUser(username, &file);
    AddServer("www.onet.pl", 443, &file, username);
    //RemoveServer(2, file);
    ToZeroIndex(&index, file);
    info = TestServer(&index, file, 50);
    info = TestServer(&index, file, 50);
    TestServer(&index, file, 50);
    TestServer(&index, file, 50);
    TestServer(&index, file, 50);

    //RemoveAllServers(file);
    CloseUser(&file);
    CloseUser(&file);
    //printf("Dane serwera:\n");
    PrintServerInfo(&info);
    PrintServerInfoHex(&info);
    return (EXIT_SUCCESS);
}

int Test_data_out() {
    unsigned char dane[inOutBufferSize] = "abcde";
    unsigned int liczba = 0xFFFFFFFF;
    memcpy(dane+6, &liczba, 4);
    PrintInOutBuffer(dane);
    return (EXIT_SUCCESS);
}
int Test_server_table() {
    ServerTable serverTable;
    ServerTableInit(&serverTable);
    char pos;
    InsertUserName("mikolaj", NULL, 5, &pos, &serverTable);
    InsertUserName("stankowiak", NULL, 6, &pos, &serverTable);
    InsertUserName("adam", NULL, 2, &pos, &serverTable);
    RemoveUserName(1, &serverTable);
    PrintServerTableSomeIndex(&serverTable, 3);
    pos = GetIndexFromUserName("mikolaj", &serverTable);
    printf("%d\n", (int)pos);
    return (EXIT_SUCCESS);
}