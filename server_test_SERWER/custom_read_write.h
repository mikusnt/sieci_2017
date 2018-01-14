/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/* 
 * File:   custom_read_write.h
 * Author: ms-lin
 *
 * Created on 14 stycznia 2018, 19:46
 */

#ifndef CUSTOM_READ_WRITE_H
#define CUSTOM_READ_WRITE_H

#ifdef __cplusplus
extern "C" {
#endif

#include "all_includes.h"
#include "data_out.h"

int CustomRead(int fd, unsigned char *buffer, int bufferSize);
int CustomWrite(int fd, unsigned char *buffer, int bufferSize);

#ifdef __cplusplus
}
#endif

#endif /* CUSTOM_READ_WRITE_H */

