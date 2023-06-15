using System;
using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;

public partial class Main : Control
{
	[Export(PropertyHint.File)] private string MovieListIn;
	[Export(PropertyHint.File)] private string MovieListOut;
	[Export(PropertyHint.File)] private PackedScene MovieEntryScene;
	[Export(PropertyHint.File)] private PackedScene RejectMovieDialogueScene;
	[Export(PropertyHint.File)] private PackedScene LoadingErrorDialogueScene;

	private Array<MovieEntry> MovieList = new();
	private VBoxContainer PageList;
	private MovieEntry CurrentMovie;
	
	private Label CurrentMovieLabel;
	private Label StatusBar;
	private Button NewMovieButton;
	private Button NewAndReplaceButton;
	private Button RejectButton;

	private SaveSystem DB;

	public override void _Ready()
	{
		StatusBar = (Label) GetNode("MainUI/TabContainer/StatusBar");
		UpdateSB("Status Bar is in the correct dimension.");
		
		DB = (SaveSystem) GetNode("SaveSystem");
		
		NewMovieButton = (Button)GetNode("MainUI/UnwatchedMoviesUI/PickMovieUI/PickNewMovie");
		NewAndReplaceButton = (Button)GetNode("MainUI/UnwatchedMoviesUI/PickMovieUI/PickNewMovieReplace");
		RejectButton = (Button)GetNode("MainUI/UnwatchedMoviesUI/PickMovieUI/RejectMovie");
		
		CurrentMovieLabel = (Label) GetNode("MainUI/UnwatchedMoviesUI/PickMovieUI/CurrentMovieTitle");
		
		UpdateSB("Entering the Spider-Verse...");
		CreateListUI(SaveSystem.DbActionsEnum.LoadUnWatchedMovieList);

		NewAndReplaceButton.Disabled = true;
		RejectButton.Disabled = true;
		
		UpdateSB("We are going into the Spider-Verse...");
		
		// TEST 
		// TEST 
		// MAKE SURE YOU DELETE IT NIGGA
		DB.LoadSettings();
	}

	public void CreateListUI(SaveSystem.DbActionsEnum RequestedList)
	{
		PageList = (VBoxContainer) GetNode("MainUI/UnwatchedMoviesUI/UwatchedListUI/MainList");

		DB.DBActionIO(RequestedList);
		
		Array<MovieEntryData> UIElementData = DB.ReturnIO();
		foreach (MovieEntryData ElementData in UIElementData)
		{
			MovieEntry NewEntry = (MovieEntry) MovieEntryScene.Instantiate();
			NewEntry.ProcessMovieData(ElementData);
			NewEntry.GenerateText();
			MovieList.Add(NewEntry);
			PageList.AddChild(NewEntry);
		}
	}

	public void PickNewMovie()
	{
		if (NewMovieButton.Text == "RANK MOVIE")
		{
			// FunctionCall
			return;
		}
		
		Random RNGesus = new();
		int ListCount = MovieList.Count;
		int MovieIndex = RNGesus.Next(ListCount);
		MovieEntry Entry = MovieList[MovieIndex];
		CurrentMovie = Entry;
		
		UpdateSB("Versed...");

		CurrentMovieLabel.Text = $"Now watching: {Entry.MovieTitle}";
		NewAndReplaceButton.Disabled = false;
		RejectButton.Disabled = false;
		
		NewMovieButton.Text = "RANK MOVIE";
	}

	public void PickNewMovieAndReplace()
	{
		Random RNGesus = new();
		int ListCount = MovieList.Count;
		int MovieIndex = RNGesus.Next(ListCount);
		MovieEntry Entry = MovieList[MovieIndex];
		CurrentMovie = Entry;
		
		UpdateSB("Re-Versed...");
		CurrentMovieLabel.Text = $"Now watching: {Entry.MovieTitle}";
		NewAndReplaceButton.Disabled = true;
	}

	public void RejectMovie()
	{
		RejectMovieDialogue RejectDialogue = (RejectMovieDialogue) RejectMovieDialogueScene.Instantiate();
		RejectDialogue.RejectMovieDialogueClosed += RejectMovieDialogueClosed;
		AddChild(RejectDialogue);
	}

	public void RejectMovieDialogueClosed(string InText)
	{
		if (InText == String.Empty)
		{
			UpdateSB("Operation aborted!");
		}
		else
		{
			CurrentMovie.MovieRejectReason = InText;
			NewMovieButton.Text = "NEW MOVIE";
			
			NewAndReplaceButton.Disabled = true;
			RejectButton.Disabled = true;
			UpdateSB("An alternate universe 'you' is watching that movie right now...");

			CurrentMovieLabel.Text = "No Movie Picked";
		}
	}

	public void ClearScreen()
	{
		foreach (var child in PageList.GetChildren())
		{
			child.QueueFree();
		}
	}

	public void UpdateSB(string Message)
	{
		StatusBar.Text = Message;
	}
}
