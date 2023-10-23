using System;
using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;

public partial class Main : Control
{
	// Windows
	private PackedScene SettingsConfigDialogueScene = (PackedScene) ResourceLoader.Load("uid://cuib88i5i8xew");
	private PackedScene LoadingErrorDialogueScene = (PackedScene) ResourceLoader.Load("uid://c6gvhe23srgto");

	// Tabs
	private PackedScene LoaderScene = (PackedScene) ResourceLoader.Load("uid://b3mp0mbv2prex");
	private PackedScene ViewRankScene = (PackedScene) ResourceLoader.Load("uid://bj7wonw0qpeqr"); 
	private PackedScene UnwatchedMovieScene = (PackedScene) ResourceLoader.Load("uid://dietw504dc5w3");
	private PackedScene WatchedMovieScene = (PackedScene) ResourceLoader.Load("uid://d350d0od5ay13");
	
	// NeonNexus Matrix
	private static Action GetUnwatchedAction;
	private static Action GetWatchedAction;
	private static Action GetRankAction;

	private System.Collections.Generic.Dictionary<int, Action> UIDict;
	private Array<PackedScene> TabSceneArray;
	private Dictionary<string, Variant> SettingsDict = new ();

	private MovieEntry CachedData; 

	private SaveSystem DB;
	private UnwatchedMoviesUI UnwatchedUI;
	private WatchedMoviesUI WatchedUI;
	private ViewRanksUI RankUI;
	private SettingsConfig SettingsDialogue;
	private Window LoadingDialogue;

	private Label StatusBar;
	private VBoxContainer MainUINode;

	public enum UIEnum
	{
		Default = -1,
		Unwatched,
		Watched,
		Rank,
	}

	public override void _EnterTree()
	{
		GetUnwatchedAction = GetUnwatchedUI;
		GetWatchedAction = GetWatchedUI;
		GetRankAction = GetRankedUI;
		
		UIDict = new()
		{
			{0, GetUnwatchedAction},
			{1, GetWatchedAction},
			{2, GetRankAction}
		};
	}

	public override void _Ready()
	{
		MainUINode = (VBoxContainer) GetNode("MainUI");

		StatusBar = (Label) GetNode("MainUI/TabContainer/StatusBar");
		UpdateSB("Status Bar is in the correct dimension.");
		
		DB = (SaveSystem) GetNode("SaveSystem");
		DB.CreateSettingsDialogue += OnSettingsPressed;
		DB.ForceCreateSettingsDialogue += CreateStartConfig;

		UpdateSB("Entering the Spider-Verse...");
		
		UpdateSB("We are going into the Spider-Verse...");
		UpdateSettings(DB.LoadSettings());
		
		if((int) SettingsDict["User"] == 3)
			DB.GetDataFromDB(true);
		
		else
			DB.GetDataFromDB();
		
		ChangeContainer(UIEnum.Unwatched);
	}

	public void ChangeContainer(UIEnum NextTab = UIEnum.Default)
	{
		MarginContainer LoaderUI = (MarginContainer) LoaderScene.Instantiate();
		MainUINode.AddChild(LoaderUI);
		
		if (UIDict.ContainsKey((int) NextTab))
		{
			Array<Node> MainChildren = MainUINode.GetChildren();
			
			foreach (Node child in MainChildren)
			{
				if (child.Name != "TabContainer" && child.Name != "SettingsButton")
				{
					child.QueueFree();
				}
			}
			UIDict[(int) NextTab]();
		}
		
		else if (NextTab == UIEnum.Default)
			UpdateSB("Canon Event Disrupted! Container couldn't be changed.");

		else
			UpdateSB("Canon Event Disrupted! Container not found!");
		
	}


	public void UpdateSB(string Message)
	{
		StatusBar.Text = Message;
	}

	public void UpdateSettings(Array<Variant> NewSettings, bool WriteToFileCheck=false)
	{
		if(SettingsDict == new Dictionary<string, Variant>() || SettingsDict.Count == 0)
		{
			SettingsDict.Add("User", NewSettings[0]);
			SettingsDict.Add("AutoSave", NewSettings[1]);
			SettingsDict.Add("Online", NewSettings[2]);
		}
		else
		{
			SettingsDict["User"] = NewSettings[0];
			SettingsDict["AutoSave"] = NewSettings[1];
			SettingsDict["Online"] = NewSettings[2];
		}
		
		if (WriteToFileCheck)
		{
			DB.CreateConfigFile(SettingsDict);
		}

		Random RNGesus = new();
		int Randy = RNGesus.Next(999999);
		
		UpdateSB($"Dimension locked in... Hello Dimension Number: {Randy}");

	}

	public void CommitSettings(Dictionary<string, Variant> Out)
	{
		DB.CreateConfigFile(Out);
	}

	public void GetUnwatchedUI()
	{
		UpdateSB("Into the Unwatched Verse...");
		DB.DBActionIO(SaveSystem.DbActionsEnum.LoadUnWatchedMovieList);
		Array<MovieEntryData> UIElementData = DB.ReturnIO();
		
		UnwatchedUI = (UnwatchedMoviesUI) UnwatchedMovieScene.Instantiate();
		MainUINode.AddChild(UnwatchedUI);
		UnwatchedUI.UpdateStatusBar += UpdateSB;
		UnwatchedUI.OpenRankUI += OnTabChanged;
		
		UnwatchedUI.GenerateScreenContent(UIElementData);
		UpdateSB("Across the Unwatched Verse...");
	}

	public void GetWatchedUI()
	{
		UpdateSB("Into the Watched Verse...");
		DB.DBActionIO(SaveSystem.DbActionsEnum.LoadWatchedMovieList);
		Array<MovieEntryData> UIElementData = DB.ReturnIO();
		
		WatchedUI = (WatchedMoviesUI) WatchedMovieScene.Instantiate();
		MainUINode.AddChild(WatchedUI);

		WatchedUI.GroupSendDatatoDB += SendToDB;
		WatchedUI.GroupSendRankToDB += SendToDB;
		WatchedUI.SendReviewToDB += SendToDB;
		WatchedUI.SendRankToDB += SendToDB;
		WatchedUI.UpdateStatusBar += UpdateSB;
		
		WatchedUI.SetCurrentUser((int) SettingsDict["User"]);
		WatchedUI.GenerateScreenContent(UIElementData);
		UpdateSB("Across the Watched Verse...");
	}

	public void GetRankedUI()
	{
		UpdateSB("Into the Ranked Verse...");
		DB.DBActionIO(SaveSystem.DbActionsEnum.LoadWatchedMovieList);
		RankUI = (ViewRanksUI) ViewRankScene.Instantiate();
		MainUINode.AddChild(RankUI);
		UpdateSB("Across the Ranked Verse...");
	}

	public void OnTabChanged(int TabNumber)
	{
		ChangeContainer((UIEnum) TabNumber);
	}

	public void CreateStartConfig()
	{
		UpdateSB("Into the Settings Verse...");
		SettingsDialogue = (SettingsConfig) SettingsConfigDialogueScene.Instantiate();
		
		SettingsDialogue.SettingsConfigDialogueClosed += UpdateSettings;
		AddChild(SettingsDialogue);
	}

	public void OnSettingsPressed()
	{
		UpdateSB("Into the Settings Verse...");
		SettingsDialogue = (SettingsConfig) SettingsConfigDialogueScene.Instantiate();

		Array<Variant> Sett = DB.LoadSettings();
		
		SettingsDialogue.SettingsConfigDialogueClosed += SettingsSubmitted;
		AddChild(SettingsDialogue);
		
		if (Sett.Count != 0)
		{
			SettingsDialogue.UpdateUI((int) Sett[0]);
			SettingsDialogue.UpdateUI((bool) Sett[1], true);
			SettingsDialogue.UpdateUI((bool) Sett[2], false);
		}
		UpdateSB("Across the Settings Verse...");
	}

	public void SettingsSubmitted(Array<Variant> NewSettings, bool WriteToFileCheck=false)
	{
		DB.CloseDBConnection();
		UpdateSettings(NewSettings, WriteToFileCheck);
		
		if((int) SettingsDict["User"] == 3)
			DB.GetDataFromDB(true);
		
		else
			DB.GetDataFromDB();
		
		ChangeContainer(UIEnum.Unwatched);
	}

	private void SendToDB(Array<MovieEntryData> Group)
	{
		DB.UpdateDataInDB(Group);
		UpdateSB("Adding new dimension description to the list...");
	}

	private void SendToDB(int movId, string rev)
	{
		DB.UpdateDataInDB(movId, rev);
		UpdateSB("This Spider has not been contacted yet.");
	}
	
	private void SendToDB(int movId, int user, int rank)
	{
		DB.UpdateDataInDB(movId, user, rank);
		UpdateSB("This Spider has not been contacted yet.");
	}
	
	private void SendToDB(int[] movIds, int user, int[] ranks)
	{
		DB.UpdateDataInDB(movIds, user, ranks);
		UpdateSB("This Spider has not been contacted yet.");
	}
}
