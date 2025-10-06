# MySQL commands to set up the database 

http://localhost/phpmyadmin

SQL tab -> copy and paste the below code in sql tab:


CREATE DATABASE sangn;

USE sangn;

CREATE TABLE scores (
    name VARCHAR(50),
    score INT
);

then, put the Assets/www/sangn <-this folder in C:\wamp64\www\HERE 
so it should be C:\wamp64\www\sangn with score.php included
