# MySQL commands to set up the database 
install wampserver64 then go to
http://localhost/phpmyadmin

SQL tab above -> copy and paste the below code in sql tab:


CREATE DATABASE mathgame;

USE mathgame;

CREATE TABLE scores (
    mode VARCHAR(50),
    bestTime FLOAT
);

then, put the Assets/www/mathgame <-this folder in C:\wamp64\www\HERE 
so it should be C:\wamp64\www\mathgame with score.php/display.php included

# score save
Note that scores are saved to gamesavefile.json after game completion and can be modify as needed.
You can visit http://localhost/mathgame/display.php to see the scores upon completing the math game.
