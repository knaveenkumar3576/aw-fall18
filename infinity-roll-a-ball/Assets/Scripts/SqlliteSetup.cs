using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;

public class SqlliteSetup : MonoBehaviour {

	// Use this for initialization
	void Start () {
        // Create database
        string connection = "URI=file:" + Application.persistentDataPath + "/" + "My_Database";

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

        string q_createTable = "CREATE TABLE IF NOT EXISTS my_table1 (id INTEGER PRIMARY KEY, level INTEGER, gameType VARCHAR(10), operation VARCHAR(10), current_datetime DATETIME, status VARCHAR(10) )";

        dbcmd.CommandText = q_createTable;
        dbcmd.ExecuteReader();

        // Insert values in table
        IDbCommand cmnd = dbcon.CreateCommand();
        cmnd.CommandText = "INSERT INTO my_table1 (id, level, gameType, operation, current_datetime, status) VALUES (0, 5,'', 'plus', null,'')";
        cmnd.ExecuteNonQuery();

        // Read and print all values in table
        IDbCommand cmnd_read = dbcon.CreateCommand();
        IDataReader reader;
        string query = "SELECT * FROM my_table1";
        cmnd_read.CommandText = query;
        reader = cmnd_read.ExecuteReader();

        while (reader.Read())
        {
            Debug.Log("id: " + reader[0].ToString());
            Debug.Log("level: " + reader[1].ToString());
        }

        // Close connection
        dbcon.Close();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
