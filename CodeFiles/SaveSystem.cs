using Godot;
using System;
using System.Data.SQLite;
using System.IO;
using System.Collections.Generic;
using Godot.Collections;
using Array = Godot.Collections.Array;
using FileAccess = Godot.FileAccess;

public partial class SaveSystem : Control
{
	[Signal] public delegate void UpdateStatusBarEventHandler(string Message); 
	
	private SQLiteConnection SQLiteConn;
	private SQLiteCommand CommandOutput;
	private SQLiteDataReader CommandReader;

	private static Action LoadUnwatchedMovieList;
	private static Action LoadWatchedMovieList;
	
	public Array<MovieEntryData> OutputArray = new();

	private ConfigFile ConFile = new ConfigFile();

	protected System.Collections.Generic.Dictionary<DbActionsEnum, Action> DBActions = new();
	
	public enum DbActionsEnum
	{
		LoadUnWatchedMovieList,
		LoadWatchedMovieList,
	}

	public override void _EnterTree()
	{
		LoadUnwatchedMovieList = GetUnwatchedMovieList;
		LoadWatchedMovieList = GetWatchedMovieList;
		
		DBActions.Add(DbActionsEnum.LoadUnWatchedMovieList, LoadUnwatchedMovieList);
		DBActions.Add(DbActionsEnum.LoadWatchedMovieList, LoadWatchedMovieList);
		
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

	public void LoadSettings()
	{
		Error Err = ConFile.Load("user://Settings.cfg");

		if (Err != Error.Ok)
		{
			EmitSignal(SignalName.UpdateStatusBar, "Canon Event Disruption: Settings file");
			FileAccess.Open("user://Settings.cfg", FileAccess.ModeFlags.Read);
			// EmitSignal()
		}
		
	}

	public void DBActionIO(DbActionsEnum ACTION)
	{
		if (DBActions.ContainsKey(ACTION))
			DBActions[ACTION]();

		else
		{
			EmitSignal(SignalName.UpdateStatusBar, "ANOMALY DETECTED IN THE ACTION-IO WING OF THE PROGRAM!!!");
		}
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
		CommandOutput.CommandText = @"SELECT * FROM movies WHERE watched = 0";
		CommandReader = CommandOutput.ExecuteReader();

		while (CommandReader.Read())
		{
			MovieEntryData DBEntry = CreateMovieEntryData();
			OutputArray.Add(DBEntry);
			// So that's the beginning...uhhhhhhhh.... what the hell am I gonna do to finish all this :(
			// Gonna need to make a quick sandbox project so I can thoroughly debug and study the "GetValues" function.
		}
	}
	
	public void GetWatchedMovieList()
	{
		CommandOutput.CommandText = @"SELECT * FROM movies WHERE watched = 1";
		CommandReader = CommandOutput.ExecuteReader();

		while (CommandReader.Read())
		{
			MovieEntryData DBEntry = CreateMovieEntryData();
			OutputArray.Add(DBEntry);
		}
	}

	private MovieEntryData CreateMovieEntryData()
	{
		MovieEntryData NewDat = new();
		NewDat.ConvertFromDB(Convert.ToInt64(CommandReader.GetValue(0)),
			Convert.ToInt64(CommandReader.GetValue(1)),
			Convert.ToInt64(CommandReader.GetValue(2)),
			Convert.ToString(CommandReader.GetValue(3)),
			Convert.ToString(CommandReader.GetValue(4)),
			Convert.ToInt64(CommandReader.GetValue(5)),
			Convert.ToInt64(CommandReader.GetValue(6)),
			Convert.ToInt64(CommandReader.GetValue(7)),
			Convert.ToInt64(CommandReader.GetValue(8)),
			Convert.ToString(CommandReader.GetValue(9)));

		return NewDat;
	}

}
