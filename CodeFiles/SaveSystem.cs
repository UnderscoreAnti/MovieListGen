using Godot;
using System;
using System.Data.SQLite;
using System.IO;
using System.Collections.Generic;
using Godot.Collections;
using Array = Godot.Collections.Array;

public partial class SaveSystem : Control
{
	private SQLiteConnection SQLiteConn;
	private SQLiteCommand CommandOutput;
	private SQLiteDataReader CommandReader;

	private static Action LoadUnwatchedMovieList;
	
	public Array<MovieEntryData> OutputArray = new();

	public enum DataBaseActionsEnum
	{
		LoadUnWatchedMovieList
	}

	private System.Collections.Generic.Dictionary<DataBaseActionsEnum, Action> DBActions = new()
	{
		{DataBaseActionsEnum.LoadUnWatchedMovieList, LoadUnwatchedMovieList}
	};
	
	public override void _Ready()
	{
		LoadUnwatchedMovieList = GetUnwatchedMovieList;
		
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
	}
	
	public override void _ExitTree()
	{
		SQLiteConn.Close();
		SQLiteConn.Dispose();
	}

	public void DBActionIO(DataBaseActionsEnum ACTION)
	{
		This right here is the fucking problem occifer
		DBActions[ACTION]();
	}

	public Array<MovieEntryData> ReturnIO()
	{
		Array<MovieEntryData> Output = new();
		Output = OutputArray.Duplicate();
		OutputArray.Clear();
		return Output;
	}

	protected void GetUnwatchedMovieList()
	{
		GD.Print("We are indeed entering the Spider-Verse...");
		CommandOutput.CommandText = @"SELECT * FROM movies WHERE watched = 0";
		CommandReader = CommandOutput.ExecuteReader();

		while (CommandReader.Read())
		{
			MovieEntryData DBEntry = new();

			DBEntry.ConvertFromDB(Convert.ToInt64(CommandReader.GetValue(0)),
				Convert.ToInt64(CommandReader.GetValue(1)),
				Convert.ToInt64(CommandReader.GetValue(2)),
				Convert.ToString(CommandReader.GetValue(3)),
				Convert.ToString(CommandReader.GetValue(4)),
				Convert.ToInt64(CommandReader.GetValue(5)),
				Convert.ToInt64(CommandReader.GetValue(6)),
				Convert.ToInt64(CommandReader.GetValue(7)),
				Convert.ToInt64(CommandReader.GetValue(8)),
				Convert.ToString(CommandReader.GetValue(9)));
			
			OutputArray.Add(DBEntry);
			// So that's the beginning...uhhhhhhhh.... what the hell am I gonna do to finish all this :(
			// Gonna need to make a quick sandbox project so I can thoroughly debug and study the "GetValues" function.
		}
	}

}
