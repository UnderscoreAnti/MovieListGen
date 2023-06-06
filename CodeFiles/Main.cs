using System;
using Godot;
using Godot.Collections;

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

		//Array<MovieEntry> Test = DB.GetUnwatchedMovieList();
		
		CreateListUI();
	}

	public void CreateListUI()
	{
		PageList = (VBoxContainer) GetNode("VBoxContainer/ScrollContainer/MainList");
		
		string LoadFile = FileAccess.GetFileAsString(MovieListIn);
		string[] NDString = LoadFile.Split("\n");

		foreach (string title in NDString)
		{
			if (title == String.Empty)
			{
				GD.Print("End of TextFile");
			}

			else
			{
				MovieEntry NewEntry = (MovieEntry) MovieEntryScene.Instantiate();
				NewEntry.UpdateName(title);
				MovieList.Add(NewEntry);
				PageList.AddChild(NewEntry);
			}
			
		}
	}

	public void PickNewMovie()
	{
		Random RNGesus = new();
		int ListCount = MovieList.Count;
		int MovieIndex = RNGesus.Next(ListCount);
		MovieEntry Entry = MovieList[MovieIndex];
		CurrentMovie = Entry;

		CurrentMovieLabel.Text = $"Now watching: {Entry.Text}";
	}

	public void PickNewMovieAndReplace()
	{
		Random RNGesus = new();
		int ListCount = MovieList.Count;
		int MovieIndex = RNGesus.Next(ListCount);
		MovieEntry Entry = MovieList[MovieIndex];
		CurrentMovie = Entry;
		CurrentMovieLabel.Text = $"Now watching: {Entry.Text} (And Replace)";
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

	public void SaveDataToFile()
	{
		
	}
}
