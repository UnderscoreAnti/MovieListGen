using Godot;
using System;
using Godot.Collections;

public partial class UnwatchedMoviesUI : VBoxContainer
{
	[Signal] public delegate void UpdateStatusBarEventHandler(string Message);
	[Signal] public delegate void OpenRankUIEventHandler(int Menu);
	
	private PackedScene MovieEntryScene = (PackedScene) ResourceLoader.Load("uid://cqjklud3wl1hl");
	private PackedScene RejectMovieDialogueScene = (PackedScene) ResourceLoader.Load("uid://blp75sr6qskvp");
	
	private Array<MovieEntry> MovieList = new();
	
	private Button NewMovieButton;
	private Button NewAndReplaceButton;
	private Button RejectButton;
	private VBoxContainer PageList;
	private Label CurrentMovieLabel;
	
	private MovieEntry CurrentMovie;

	public void GenerateScreenContent(Array<MovieEntryData> RequestedList)
	{
		NewMovieButton = (Button) GetNode("PickMovieUI/PickNewMovie");
		NewAndReplaceButton = (Button) GetNode("PickMovieUI/PickNewMovieReplace");
		RejectButton = (Button) GetNode("PickMovieUI/RejectMovie");
		
		CurrentMovieLabel = (Label) GetNode("PickMovieUI/CurrentMovieTitle");
		
		NewAndReplaceButton.Disabled = true;
		RejectButton.Disabled = true;
		
		PageList = (VBoxContainer) GetNode("UwatchedListUI/MainList");
		foreach (MovieEntryData ElementData in RequestedList)
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
			EmitSignal(SignalName.OpenRankUI, (int) Main.UIEnum.Rank);
			return;
		}
		
		Random RNGesus = new();
		int ListCount = MovieList.Count;
		int MovieIndex = RNGesus.Next(ListCount);
		MovieEntry Entry = MovieList[MovieIndex];
		CurrentMovie = Entry;
		
		EmitSignal(SignalName.UpdateStatusBar, "Versed...");

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
		
		EmitSignal(SignalName.UpdateStatusBar, "Re-Versed...");
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
			EmitSignal(SignalName.UpdateStatusBar, "Operation aborted!");
		}
		else
		{
			CurrentMovie.MovieRejectReason = InText;
			NewMovieButton.Text = "NEW MOVIE";
			
			NewAndReplaceButton.Disabled = true;
			RejectButton.Disabled = true;
			EmitSignal(SignalName.UpdateStatusBar, "An alternate universe 'you' is watching that movie right now...");

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
}
