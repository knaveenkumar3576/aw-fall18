using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;

public class SqlliteSetup : MonoBehaviour {

    // Use this for initialization
    string connection;

    void Start () {
        // Create database
        connection = "URI=file:" + Application.persistentDataPath + "/" + "My_Database";

        // Open connection
        IDbConnection dbcon = new SqliteConnection(connection);
        dbcon.Open();

        //drop table if exist *comment if you don't want table to drop*
        IDbCommand dbdmd;
        dbdmd = dbcon.CreateCommand();

        string q_dropTable = "DROP TABLE IF EXISTS my_table1";
        dbdmd.CommandText = q_dropTable;
        dbdmd.ExecuteReader();

        // Create table
        IDbCommand dbcmd;
        dbcmd = dbcon.CreateCommand();

        string q_createTable = "CREATE TABLE IF NOT EXISTS my_table1 (id INTEGER PRIMARY KEY AUTOINCREMENT, level INTEGER, gameType VARCHAR(10), operation VARCHAR(10), time_taken INTEGER, status VARCHAR(10) )";

        dbcmd.CommandText = q_createTable;
        dbcmd.ExecuteReader();

        dbcon.Close();

        insertData(5, "Normal", "+", 0, "W");
        insertData(5, "Normal", "*", 0, "W");
        insertData(2, "Normal", "/", 0, "L");
        insertData(3, "Normal", "-", 0, "L");
        insertData(9, "Normal", "-", 0, "W");
        insertData(10, "Death", "*", 0, "L");

        getData("+");
    }

    void insertData(int level, string gameType, string operation, int time_taken, string status)
    {
        IDbConnection dbcon = new SqliteConnection(connection);
        dbcon.Open();
        IDbCommand cmnd = dbcon.CreateCommand();
        cmnd.CommandText = string.Format("INSERT INTO my_table1 (level, gameType, operation, time_taken, status) VALUES ({0}, '{1}', '{2}', {3}, '{4}')", level, gameType, operation, time_taken, status);
        Debug.Log("Insert " + cmnd.CommandText);
        cmnd.ExecuteNonQuery();
        dbcon.Close();
    }

    void getData(string operation)
    {
        IDbConnection dbcon = new SqliteConnection(connection);
        dbcon.Open();
        IDbCommand cmnd_read = dbcon.CreateCommand();
        IDataReader reader;
        string query = string.Format("SELECT * FROM my_table1 where operation = '{0}'", operation);
        cmnd_read.CommandText = query;
        reader = cmnd_read.ExecuteReader();

        while (reader.Read())
        {
            Debug.Log("id: " + reader[0].ToString());
            Debug.Log("level: " + reader[1].ToString());
            Debug.Log("gameType: " + reader[2].ToString());
            Debug.Log("operation: " + reader[3].ToString());
            Debug.Log("current_datetime: " + reader[4].ToString());
            Debug.Log("status: " + reader[5].ToString());
        }
        dbcon.Close();
    }


    // Update is called once per frame
    void Update () {
		
	}
}
