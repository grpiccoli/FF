rm(list=ls())

library(readxl)
library(data.table)
library(fpfi3)

icond <- data.table(read_excel("E:/WebProjects/FPFI/3/Input_clean_3_FDS_2018.xlsx", 1))
products <- data.table(read_excel("E:/WebProjects/FPFI/3/Input_clean_3_FDS_2018.xlsx", 2))
coefs <- data.table(read_excel("E:/WebProjects/FPFI/3/Input_clean_3_FDS_2018.xlsx", 3))
input_data <- list(df=icond, params=coefs, prods=products)

ans <- FullSimulation(in_data = input_data, 
                      age_range = c(15:30),
                      distribution="normal",
                      distribution_thinning="normal",
                      include_thinning = TRUE, 
                      species="eucalyptus_grandis", 
                      model="dss", 
                      volform="smalian", 
                      byClass = T,
                      way = "lmak", 
                      region = "uruguay_solid", 
                      stump = 0.15, 
                      mg_disc = 0,
                      length_disc = 0)
