using Godot;
using System;
using System.Data.SQLite;
using System.IO;
using System.Collections.Generic;
using Godot.Collections;
using Array = Godot.Collections.Array;
using Environment = System.Environment;
using FileAccess = Godot.FileAccess;

public partial class SaveSystem : Control
{
	[Signal] public delegate void UpdateStatusBarEventHandler(string Message);
	[Signal] public delegate void CreateSettingsDialogueEventHandler();
	
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
		
		string TempPath = @"%APPDATA%\Godot\app_userdata\MovieListGenerator";
		string NuPath = Environment.ExpandEnvironmentVariables(TempPath);
		
		if (!File.Exists($"{NuPath}\\SaveFile.db"))
		{
			File.Create($"{NuPath}\\SaveFile.db").Dispose();
		}
		
		else if (File.Exists("SaveFile.db") && !File.Exists($"{NuPath}\\SaveFile.db"))
		{
			File.Copy("SaveFile.db", $"{NuPath}\\SaveFile.db");
		}
		
		SQLiteConn = new SQLiteConnection($"Data Source={NuPath}\\SaveFile.db");
		SQLiteConn.Open();
		CommandOutput = SQLiteConn.CreateCommand();
	}
	
	public override void _ExitTree()
	{
		SQLiteConn.Close();
		SQLiteConn.Dispose();
	}

	public Array LoadSettings()
	{
		Error Err = ConFile.Load("user://Settings.cfg");
		Array ReturnArray = new Array();

		if (Err != Error.Ok)
		{
			EmitSignal(SignalName.UpdateStatusBar, "Canon Event Disruption: Settings file");
			EmitSignal(SignalName.CreateSettingsDialogue);
		}

		if (Err == Error.Ok)
		{
			string Sect = "Settings";
			
			if(ConFile.HasSection(Sect))
			{
				if (ConFile.HasSectionKey(Sect, "User") && ConFile.HasSectionKey(Sect, "AutoSave") &&
				    ConFile.HasSectionKey(Sect, "Online"))
				{
					ReturnArray.Add((int)ConFile.GetValue(Sect, "User"));
					ReturnArray.Add((bool)ConFile.GetValue(Sect, "AutoSave"));
					ReturnArray.Add((bool)ConFile.GetValue(Sect, "Online"));
				}
				
				else
				{
					EmitSignal(SignalName.UpdateStatusBar, "Your spider is in a different universe.");
					EmitSignal(SignalName.CreateSettingsDialogue);
				}
			}
		}

		return ReturnArray;

	}

	public void CreateConfigFile(int User, bool AutoSave, bool Online)
	{
		ConfigFile NewF = new();
		
		NewF.SetValue("Settings", "User", User);
		NewF.SetValue("Settings", "AutoSave", AutoSave);
		NewF.SetValue("Settings", "Online", Online);

		NewF.Save("user://Settings.cfg");
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
		}
	}
	
	protected void GetWatchedMovieList()
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
