/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/* 
 * File:   all_includes.h
 * Author: ms-lin
 *
 * Created on 9 stycznia 2018, 17:58
 */

#ifndef ALL_INCLUDES_H
#define ALL_INCLUDES_H

#ifdef __cplusplus
extern "C" {
#endif

#include <sys/types.h>
#include <sys/wait.h>
#include <sys/socket.h>
#include <sys/shm.h>
#include <sys/ipc.h>
#include <sys/msg.h>
#include <sys/time.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <netdb.h>
#include <stdio.h>
#include <fcntl.h>
#include <unistd.h>
#include <string.h>
#include <stdlib.h>
#include <signal.h>
#include <strings.h>
#include <time.h>
#include <termios.h>
#include <pthread.h>

    /*
     
     Zawiera biblioteki wykorzystywane w projekcie, kazdy inny plik naglowkowy 
     * korzysta z tej biblioteki
     
     */

#ifdef __cplusplus
}
#endif

#endif /* ALL_INCLUDES_H */

