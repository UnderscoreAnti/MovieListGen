using Godot;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using Godot.Collections;

public partial class SaveSystem : Control
{
	private SQLiteConnection SQLiteConn;
	private SQLiteCommand CommandOutput;
	private SQLiteDataReader CommandReader;

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
			// Code that will pull data from a remote location and fill the DB with update data
		}
		
		SQLiteConn.Open();
		CommandOutput = SQLiteConn.CreateCommand();
		// CommandOutput.CommandText = @"INSERT INTO movies VALUES (12093012009, 1, 1, 'movie was not rejected', " +
        //                    @"'stinky movie', 3, 3, 2 ,3, 'Worst Fake Movie')";

		CommandOutput.ExecuteNonQuery();
	}

	public Array<MovieEntry> GetUnwatchedMovieList()
	{
		CommandOutput.CommandText = @"SELECT * from movies WHERE watched = 0;";
		
		CommandReader = CommandOutput.ExecuteReader();

		if (CommandReader.HasRows)
		{
			
		}

		else
		{
			GD.PushWarning("Data could not be collected from the database. Please check the command and try again.");
		}
		return new Array<MovieEntry>();
	}

	public override void _ExitTree()
	{
		SQLiteConn.Close();
		SQLiteConn.Dispose();
	}
	
	public override void _Process(double delta)
	{
	}
}
