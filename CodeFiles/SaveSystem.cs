using Godot;
using System;
using System.Configuration;
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
			// Tbh this should never happen. I have no clue how to prevent this from happening. 
			// Code that will pull data from a remote location and fill the DB with update data
		}
		
		SQLiteConn.Open();
		CommandOutput = SQLiteConn.CreateCommand();

		GetUnwatchedMovieList();
	}

	public void GetUnwatchedMovieList()
	{
		GD.Print("We are indeed entering the Spider-Verse...");
		CommandOutput.CommandText = @"SELECT * FROM movies WHERE watched = 1";
		CommandReader = CommandOutput.ExecuteReader();

		while (CommandReader.Read())
		{
			var GetVals = CommandReader.GetValues();
			GD.Print($"Movie: {CommandReader.GetValue(7).GetType()} (id: {CommandReader.GetValue(1).GetType()}) has the rank: {CommandReader.GetValue(3)}");
			
			// So that's the beginning...uhhhhhhhh.... what the hell am I gonna do to finish all this :(
			// Gonna need to make a quick sandbox project so I can thoroughly debug and study the "GetValues" function.
		}
	}

	public override void _ExitTree()
	{
		SQLiteConn.Close();
		SQLiteConn.Dispose();
	}
}
