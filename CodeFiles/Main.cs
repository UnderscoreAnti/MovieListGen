using System;
using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;

public partial class Main : Control
{
	[Export(PropertyHint.File)] private PackedScene SettingsConfigDialogueScene;
	[Export(PropertyHint.File)] private PackedScene LoadingErrorDialogueScene;
	[Export(PropertyHint.File)] private PackedScene UnwatchedMovieScene;
	[Export(PropertyHint.File)] private PackedScene WatchedMovieScene;
	[Export(PropertyHint.File)] private PackedScene ViewRankScene;

	private Array Settings = new();
	
	private Label StatusBar;
	
	private SaveSystem DB;

	private UnwatchedMoviesUI UnwatchedUI;
	private WatchedMoviesUI WatchedUI;
	private ViewRanksUI RankUI;

	private Control CurrentUINode;

	public override void _Ready()
	{
		
		StatusBar = (Label) GetNode("MainUI/TabContainer/StatusBar");
		UpdateSB("Status Bar is in the correct dimension.");
		
		DB = (SaveSystem) GetNode("SaveSystem");
		DB.CreateSettingsDialogue += OpenSettingsConfigWindow;

		UpdateSB("Entering the Spider-Verse...");
		
		UnwatchedUI = (UnwatchedMoviesUI) UnwatchedMovieScene.Instantiate();
		UnwatchedUI.GenerateScreenContent(DB, SaveSystem.DbActionsEnum.LoadUnWatchedMovieList);

		UpdateSB("We are going into the Spider-Verse...");
		UpdateSettings(DB.LoadSettings());
	}

	public void OpenSettingsConfigWindow()
	{
		SettingsConfig ConfigDialogue = (SettingsConfig) SettingsConfigDialogueScene.Instantiate();
		ConfigDialogue.SettingsConfigDialogueClosed += SettingsConfigDialogueClosed;
		AddChild(ConfigDialogue);
	}

	public void SettingsConfigDialogueClosed(int User, bool AutoSave, bool Online)
	{
		DB.CreateConfigFile(User, AutoSave, Online);
		UpdateSB("Dimension locked in... Hello 42");
	}

	public void UpdateSB(string Message)
	{
		StatusBar.Text = Message;
	}

	public void UpdateSettings(Array NewSettings)
	{
		Settings = NewSettings;
	}
}
