<?php
require "connector.php";

$connector = new Connector();
$connector->connect();

// Variable
$hasData = false;
$now = new DateTime();
$start = new DateTime();
$end = new DateTime();

// Load the start / end date
$query = "SELECT MIN(date), MAX(date) FROM plan WHERE active = 1";
$cmd = $connector->prepare($query);
$cmd->execute();
$cmd->bind_result($min, $max);
$fetchResult = $cmd->fetch();
if ($fetchResult)
{
    $hasData = true;
    $start = new DateTime($min);
    $end = new DateTime($max);
}
$cmd->close();
?>

<html>

<head>
    <meta charset="utf-8" />
    <meta name="author" content="A. Pouwels">
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Shift-Plan</title>
    <!-- Latest compiled and minified CSS -->
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.4.1/css/bootstrap.min.css">

    <!-- jQuery library -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.4.1/jquery.min.js"></script>

    <!-- Popper JS -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.16.0/umd/popper.min.js"></script>

    <!-- Latest compiled JavaScript -->
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/4.4.1/js/bootstrap.min.js"></script>

    <!-- Material icons -->
    <link rel="stylesheet" href="https://fonts.googleapis.com/icon?family=Material+Icons">

    <style>
        td {
            vertical-align: center;
        }

        .today {
            font-weight: bold;
        }
    </style>
</head>

<body class="bg-secondary">
    <nav class="navbar navbar-expand-sm bg-dark navbar-dark fixed-top">
        <a class="navbar-brand" href="http://www.de-boddels.de/shiftplan/">Shift-Plan</a>
    </nav>
    <div class="container" style="margin-top:70px;">
        <h3 class="text-white"><?php echo $start->format("d.m.Y") . " - " . $end->format("d.m.Y"); ?></h3>
<?php
if ($hasData)
{
?>
        <table class="table table-dark table-hover">
            <caption class="text-white">Created with <i>ShiftPlan</i>. Last update: <?php echo $now->format("d.m.Y H:i:s"); ?></caption>
            <thead>
                <th>Week</th>
                <th>Date</th>
                <th>Person</th>
            </thead>
            <tbody>
<?php
$query = "SELECT
            dateView,
            calendarWeek,
            person,
            CASE WHEN date = curdate() THEN 1 ELSE 0 END AS today,
            CASE DAYOFWEEK(date)
                WHEN 6 THEN 1
                WHEN 7 THEN 1
                ELSE 0
            END AS weekend
          FROM
            plan
          ORDER BY
            date;";
$cmd = $connector->prepare($query);
$cmd->execute();
$cmd->bind_result($date, $week, $person, $today, $weekend);

while ($cmd->fetch())
{
    if ($today)
    {
        echo "<tr class='table-primary text-dark today'>";
    }
    else if ($weekend)
    {
        echo "<tr class='table-dark text-dark'>";
    }
    else
    {
        echo "<tr>";
    }
    echo "<td>$week</td><td>$date</td><td>$person</td></tr>";
}

?>
            </tbody>
        </table>
<?php
}
else
{
?>
        <div class="alert alert-danger">
            <strong>No data!</strong> Currently no data available.
        </div>
<?php
}
?>
    </div>
</body>

</html>