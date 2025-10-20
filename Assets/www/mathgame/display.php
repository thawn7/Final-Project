<?php
$hostname = "localhost";
$username = "root";
$password = "";
$dbname = "mathgame";

$conn = new mysqli($hostname, $username, $password, $dbname);
if ($conn->connect_error) {
    die("DB connection failed: " . $conn->connect_error);
}

$sql = "SELECT mode, bestTime FROM scores ORDER BY bestTime ASC";
$result = $conn->query($sql);

if ($result->num_rows > 0) {
    echo "<h2>High Scores</h2>";
    echo "<table border='0' cellpadding='6'><tr><th>Mode</th><th>Best Time (s)</th></tr>";
    while($row = $result->fetch_assoc()) {
        echo "<tr><td>" . htmlspecialchars($row["mode"]) . "</td><td>" . $row["bestTime"] . "</td></tr>";
    }
    echo "</table>";
} else {
    echo "No scores found.";
}

$conn->close();
?>
