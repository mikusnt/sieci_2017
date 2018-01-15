/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

#include "custom_read_write.h"

int CustomRead(int fd, unsigned char *buffer, int bufferSize) {
    int byteCount = 0;
    int delta;
    int endCounter = 0;
    while(1) {
        delta = read(fd, buffer + byteCount, bufferSize - byteCount);
        if (delta == -1) return -1;
        for (int i = byteCount; i < byteCount + delta; i++) {
            if (buffer[i] == endInOutBufferByte) 
                endCounter++;
            else 
                endCounter = 0;
        }
        byteCount += delta;
        if (endCounter >= endInOutBufferSize) break;
        if ((bufferSize - byteCount) <= 0) return -1;
        
    }
    return 0;
    
}
int CustomWrite(int fd, unsigned char *buffer, int bufferSize){
    int byteCount = 0;
    int delta;
    int endCounter = 0;
    while(1) {
        delta = write(fd, buffer + byteCount, bufferSize - byteCount);
        if (delta == -1) return -1;
        for (int i = byteCount; i < byteCount + delta; i++) {
            if (buffer[i] == endInOutBufferByte) 
                endCounter++;
            else
                endCounter = 0;
        }
        byteCount += delta;
        if (endCounter >= endInOutBufferSize) break;
        if ((bufferSize - byteCount) <= 0) return -1;   
    }
    return 0;
}


