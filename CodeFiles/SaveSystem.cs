using Godot;
using System;
using System.Data.SQLite;
using System.IO;
using System.Collections.Generic;
using System.Data;
using Godot.Collections;
using Array = Godot.Collections.Array;
using Environment = System.Environment;

public partial class SaveSystem : Control
{
	[Signal] public delegate void UpdateStatusBarEventHandler(string Message);
	[Signal] public delegate void CreateSettingsDialogueEventHandler();
	
	[Signal] public delegate void ForceCreateSettingsDialogueEventHandler();
	
	private SQLiteConnection SQLiteConn;
	private SQLiteCommand CommandOutput;
	private SQLiteDataReader CommandReader;

	private static Action LoadUnwatchedMovieList;
	private static Action LoadWatchedMovieList;
	
	public Array<MovieEntryData> OutputArray = new();

	private ConfigFile ConFile = new ConfigFile();

	protected System.Collections.Generic.Dictionary<DbActionsEnum, Action> DBActions = new();

	public enum UsersEnum
	{
		Lenzo = 0,
		Jason,
		Shai,
		Dev
	}
	
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
	}
	
	public override void _ExitTree()
	{
		CloseDBConnection();
	}

	public Array<Variant> LoadSettings()
	{
		Error Err = ConFile.Load("user://Settings.cfg");
		
		Array<Variant> ReturnArray = new();

		if (Err != Error.Ok)
		{
			EmitSignal(SignalName.UpdateStatusBar, "Canon Event Disruption: Settings file");
			EmitSignal(SignalName.ForceCreateSettingsDialogue);
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

	public void CreateConfigFile(Godot.Collections.Dictionary<string, Variant> Set)
	{
		Error Err = ConFile.Load("user://Settings.cfg");
		if (Err == Error.Ok)
		{
			UpdateConfigFile(Set);
		}
		
		ConFile.SetValue("Settings", "User", Set["User"]);
		ConFile.SetValue("Settings", "AutoSave", Set["AutoSave"]);
		ConFile.SetValue("Settings", "Online", Set["Online"]);
		
		ConFile.Save("user://Settings.cfg");
	}

	public void UpdateConfigFile(Godot.Collections.Dictionary<string, Variant> Set)
	{
		if ((int) ConFile.GetValue("Settings", "User") != (int) Set["User"])
		{
			ConFile.SetValue("Settings", "User", (int) Set["User"]);
		}
		if ((bool) ConFile.GetValue("Settings", "AutoSave") != (bool) Set["AutoSave"])
		{
			ConFile.SetValue("Settings", "AutoSave", (bool) Set["AutoSave"]);
		}
		if ((bool) ConFile.GetValue("Settings", "Online") != (bool) Set["Online"])
		{
			ConFile.SetValue("Settings", "Online", (bool) Set["Online"]);
		}

		ConFile.Save("user://Settings.cfg");
	}
	
	public void GetDataFromDB(bool isDevEnabled=false)
	{
		string WinPath = @"%APPDATA%\Godot\app_userdata\MovieListGenerator";
		string LinPath = @"godot/app_userdata/MovieListGenerator";

		string NuPath = String.Empty;
		string CurrentOS = OS.GetName();

		if (CurrentOS == "Linux")
		{
			NuPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			
			if(!File.Exists($"{NuPath}/{LinPath}/SaveFile.db"))
			{
				GD.Print("Creating new file");
				File.Create($"{NuPath}/SaveFile.db").Dispose();
			}
			
			else if (File.Exists("SaveFile.db") && !File.Exists($"{NuPath}/SaveFile.db"))
			{
				File.Copy("SaveFile.db", $"{NuPath}/SaveFile.db");
			}

			NuPath += $"/{LinPath}";
		}
		
		else if (CurrentOS == "Windows" || CurrentOS == "UWP")
		{
			NuPath = Environment.ExpandEnvironmentVariables(WinPath);

			if (!File.Exists($"{NuPath}\\SaveFile.db"))
			{
				File.Create($"{NuPath}\\SaveFile.db").Dispose();
			}

			else if (File.Exists("SaveFile.db") && !File.Exists($"{NuPath}\\SaveFile.db"))
			{
				File.Copy("SaveFile.db", $"{NuPath}\\SaveFile.db");
			}
		}
		
		if(NuPath != String.Empty && !isDevEnabled)
		{
			if(CurrentOS == "Windows" || CurrentOS == "UWP")
				SQLiteConn = new SQLiteConnection($"Data Source={NuPath}\\SaveFile.db");
			
			if(CurrentOS == "Linux")
				SQLiteConn = new SQLiteConnection($"Data Source={NuPath}/SaveFile.db");
			
			SQLiteConn.Open();
			CommandOutput = SQLiteConn.CreateCommand();
		}
		
		else if (NuPath != String.Empty && isDevEnabled)
		{
			SQLiteConn = new SQLiteConnection("Data Source=SaveFile.db");
			SQLiteConn.Open();
			CommandOutput = SQLiteConn.CreateCommand();
		}

		else
		{
			EmitSignal(SignalName.UpdateStatusBar, "CANON EVENT DISRUPTED: FILE SYSTEM NOT DETECTED");
		}
	}

	public void CloseDBConnection()
	{
		SQLiteConn.Close();
		SQLiteConn.Dispose();
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

	public void AddNewDataToDB(MovieEntryData WriteData)
	{
		CommandOutput.CommandText = @"INSERT INTO movies (movId, watched, findable, reject, review, " +
		                            @"grank, lrank, jrank, srank, title) VALUES (@id, @wa, @fi, @rj, " +
		                            @"@rv, @gr, @lr, @jr, @sr, @ti)";

		CommandOutput.Parameters.AddWithValue("@id", WriteData.MovieID.ToString());
		CommandOutput.Parameters.AddWithValue("@wa", WriteData.AlreadyWatched.ToString());
		CommandOutput.Parameters.AddWithValue("@fi", WriteData.IsFinable.ToString());
		CommandOutput.Parameters.AddWithValue("@rj", WriteData.MovieRejectReason);
		CommandOutput.Parameters.AddWithValue("@rv", WriteData.MovieReview);
		CommandOutput.Parameters.AddWithValue("@gr", WriteData.GeneralMovieRanking.ToString());
		CommandOutput.Parameters.AddWithValue("@lr", WriteData.LenzoMovieRanking.ToString());
		CommandOutput.Parameters.AddWithValue("@jr", WriteData.JasonMovieRanking.ToString());
		CommandOutput.Parameters.AddWithValue("@sr", WriteData.ShaiMovieRanking.ToString());
		CommandOutput.Parameters.AddWithValue("@ti", WriteData.MovieTitle);

		CommandOutput.ExecuteNonQuery();
	}

	public void UpdateDataInDB(Array<MovieEntryData> DataList)
	{
		foreach (MovieEntryData Data in DataList)
		{
			CommandOutput.CommandText = @"UPDATE movies SET watched = @wa, findable = @fi, reject = @rj, " + 
			                            @"review = @rv, grank = @gr, lrank = @lr, jrank = @jr, srank = @sr " +
			                            @"WHERE movID = @id";

			CommandOutput.Parameters.AddWithValue("@wa", Data.AlreadyWatched.ToString());
			CommandOutput.Parameters.AddWithValue("@fi", Data.IsFinable.ToString());
			CommandOutput.Parameters.AddWithValue("@rj", Data.MovieRejectReason);
			CommandOutput.Parameters.AddWithValue("@rv", Data.MovieReview);
			CommandOutput.Parameters.AddWithValue("@gr", Data.GeneralMovieRanking.ToString());
			CommandOutput.Parameters.AddWithValue("@lr", Data.LenzoMovieRanking.ToString());
			CommandOutput.Parameters.AddWithValue("@jr", Data.JasonMovieRanking.ToString());
			CommandOutput.Parameters.AddWithValue("@sr", Data.ShaiMovieRanking.ToString());
			CommandOutput.Parameters.AddWithValue("@id", Data.MovieID.ToString());

			CommandOutput.ExecuteNonQuery();
		}
		
	}

	public void UpdateDataInDB(int movId, string rev)
	{
		CommandOutput.CommandText = @"UPDATE movies SET review = @rev WHERE movID = @id";
		CommandOutput.Parameters.AddWithValue("@rev", rev);
		CommandOutput.Parameters.AddWithValue("@id", movId);

		CommandOutput.ExecuteNonQuery();
	}

	public void UpdateDataInDB(int movID, int user, int rank)
	{
		string CurrentRank = String.Empty;
		
		if (user == (int) UsersEnum.Lenzo)
			CurrentRank = @"UPDATE movies SET lrank = @rev WHERE movID = @id";

		else if (user == (int) UsersEnum.Jason)
			CurrentRank = @"UPDATE movies SET jrank = @rev WHERE movID = @id";

		else if (user == (int) UsersEnum.Shai)
			CurrentRank = @"UPDATE movies SET srank = @rev WHERE movID = @id";

		else if (user == (int) UsersEnum.Dev)
			CurrentRank = @"UPDATE movies SET grank = @rev WHERE movID = @id";
		
		CommandOutput.CommandText = CurrentRank;

		CommandOutput.Parameters.AddWithValue("@rev", rank);
		CommandOutput.Parameters.AddWithValue("@id", movID);

		CommandOutput.ExecuteNonQuery();
	}
	
	public void UpdateDataInDB(int[] movIds, int user, int[] ranks)
	{
		// TODO: This function, but like correctly this time.
		
		string CurrentRank = String.Empty;
		
		if (user == (int) UsersEnum.Lenzo)
			CurrentRank = @"UPDATE movies SET lrank = @rev WHERE movID = @id";

		else if (user == (int) UsersEnum.Jason)
			CurrentRank = @"UPDATE movies SET jrank = @rev WHERE movID = @id";

		else if (user == (int) UsersEnum.Shai)
			CurrentRank = @"UPDATE movies SET srank = @rev WHERE movID = @id";

		else if (user == (int) UsersEnum.Dev)
			CurrentRank = @"UPDATE movies SET grank = @rev WHERE movID = @id";
		
		CommandOutput.CommandText = CurrentRank;

		int index = 0;
		foreach (int movId in movIds)
		{
			int rank = ranks[index];
			CommandOutput.Parameters.AddWithValue("@rev", rank.ToString());
			CommandOutput.Parameters.AddWithValue("@id", movId.ToString());
			CommandOutput.ExecuteNonQuery();
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
		
		CommandReader.Close();
	}
	
	protected void GetWatchedMovieList()
	{
		CommandOutput.CommandText = @"SELECT * FROM movies WHERE watched = 1 AND grank != 0 ORDER BY grank";
		CommandReader = CommandOutput.ExecuteReader();

		while (CommandReader.Read())
		{
			MovieEntryData DBEntry = CreateMovieEntryData();
			OutputArray.Add(DBEntry);
		}
		
		CommandReader.Close();
	}

	protected void GetWatchedUnrankedMovieList()
	{
		CommandOutput.CommandText = @"SELECT * FROM movies WHERE watched = 1 AND grank = 0";
		CommandReader = CommandOutput.ExecuteReader();

		while (CommandReader.Read())
		{
			MovieEntryData DBEntry = CreateMovieEntryData();
			OutputArray.Add(DBEntry);
		}
		
		CommandReader.Close();
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
