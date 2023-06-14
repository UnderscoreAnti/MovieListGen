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

	private SaveSystem DB;

	public override void _Ready()
	{
		DB = (SaveSystem) GetNode("SaveSystem");
		CurrentMovieLabel = (Label) GetNode("VBoxContainer/HBoxContainer/CurrentMovieTitle");
		
		CreateListUI(SaveSystem.DbActionsEnum.LoadUnWatchedMovieList);
	}

	public void CreateListUI(SaveSystem.DbActionsEnum RequestedList)
	{
		PageList = (VBoxContainer) GetNode("VBoxContainer/ScrollContainer/MainList");

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
		Random RNGesus = new();
		int ListCount = MovieList.Count;
		int MovieIndex = RNGesus.Next(ListCount);
		MovieEntry Entry = MovieList[MovieIndex];
		CurrentMovie = Entry;

		CurrentMovieLabel.Text = $"Now watching: {Entry.MovieTitle}";
	}

	public void PickNewMovieAndReplace()
	{
		Random RNGesus = new();
		int ListCount = MovieList.Count;
		int MovieIndex = RNGesus.Next(ListCount);
		MovieEntry Entry = MovieList[MovieIndex];
		CurrentMovie = Entry;
		CurrentMovieLabel.Text = $"Now watching: {Entry.MovieTitle} (And Replace)";
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
			GD.Print("Operation aborted!");
		}
		else
		{
			CurrentMovie.MovieRejectReason = InText;
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
