<?php
$servername = "localhost";
$username = "root";
$password = "";
$dbname = "mathgame";

$mode = $_POST['mode'] ?? 'Unknown';
$bestTime = $_POST['bestTime'] ?? 0;

$conn = new mysqli($servername, $username, $password, $dbname);
if ($conn->connect_error) {
    die("DB connection failed: " . $conn->connect_error);
}

$stmt = $conn->prepare("INSERT INTO scores (mode, bestTime) VALUES (?, ?)");
$stmt->bind_param("sd", $mode, $bestTime); 
$stmt->execute();

echo "go to http://localhost/mathgame/display.php";
$conn->close();
?>
