<?php
class Connector
{
    private $connection;
    
    // Connection variables
    private $server = "server";
    private $database = "database";
    private $user = "user";
    private $password = "password";

    /**
     * Creates the connection to the defined database
     */
    public function connect() {
        $this->connection = new mysqli($this->server, $this->user, $this->password, $this->database);

        if ($this->connection->connect_error) {
            die("Connection fail: " . $this->connection->connect_error);
        }
    }

    /**
     * Returns the connection
     */
    public function getConnection() {
        return $this->connection;
    }

    /**
     * Prepares the given query
     */
    public function prepare($query)
    {
        $result = $this->connection->prepare($query);
        if (!$result)
        {
            echo "<br />Error start:<br /><br />";
            print_r($this->connection->error_list);
            echo "<br /><br />Error end<br />";
        }
        else
        {
            return $result;
        }
    }
}
?>