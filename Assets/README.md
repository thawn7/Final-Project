# MySQL commands to set up the database 

http://localhost/phpmyadmin

SQL tab -> copy and paste the below code in sql tab:


CREATE DATABASE mathgame;

USE mathgame;

CREATE TABLE scores (
    mode VARCHAR(50),
    bestTime FLOAT
);


then, put the Assets/www/mathgame <-this folder in C:\wamp64\www\HERE 
so it should be C:\wamp64\www\mathgame with score.php included
