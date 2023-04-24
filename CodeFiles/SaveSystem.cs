using Godot;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;

//
// Create faux data to test with
// Create a command to filter faux data
// Create a command to enter data from the application
//

public partial class SaveSystem : Control
{
	private SQLiteConnection SQLiteConn;
	private SQLiteCommand CommandOutput;

	public override void _Ready()
	{
		if (File.Exists("SaveFile.db"))
		{
			SQLiteConn = new SQLiteConnection("Data Source=SaveFile.db");
		}

		else
		{
			File.Create("SaveFile.db").Dispose();

			SQLiteConn = new SQLiteConnection("Data Source=SaveFile.db");
		}
		
		SQLiteConn.Open();
		CommandOutput = SQLiteConn.CreateCommand();
		CommandOutput.CommandText = @"INSERT INTO movies VALUES (12093012009, 1, 1, 'movie was not rejected', " +
                           @"'stinky movie', 3, 3, 2 ,3, 'Worst Fake Movie')";

		CommandOutput.ExecuteNonQuery();
	}

	public override void _ExitTree()
	{
		SQLiteConn.Close();
		SQLiteConn.Dispose();
	}

	public void AddMovieEntryToDB()
	{
		//
	}
	
	public override void _Process(double delta)
	{
	}
}
