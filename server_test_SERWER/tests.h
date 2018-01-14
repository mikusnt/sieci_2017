/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/* 
 * File:   tests.h
 * Author: ms-lin
 *
 * Created on 9 stycznia 2018, 08:26
 */

#ifndef TESTS_H
#define TESTS_H

#ifdef __cplusplus
extern "C" {
#endif

#include "all_includes.h"
    
#include "io.h"
#include "data_out.h"
#include "server_table.h"
    
// prowizoryczny test io.h
int Test_io();
// prowizoryczny test data_out.h
int Test_data_out();
// prowizoryczny test server_table.h
int Test_server_table();


#ifdef __cplusplus
}
#endif

#endif /* TESTS_H */

