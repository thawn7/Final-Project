# MySQL commands to set up the database 
# install wampserver64 then go to
http://localhost/phpmyadmin

SQL tab above -> copy and paste the below code in sql tab:


CREATE DATABASE mathgame;

USE mathgame;

CREATE TABLE scores (
    mode VARCHAR(50),
    bestTime FLOAT
);

then, put the Assets/www/sangn <-this folder in C:\wamp64\www\HERE 
so it should be C:\wamp64\www\sangn with score.php included
