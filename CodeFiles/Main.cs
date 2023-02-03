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

	private string SaveFilePath = "user://MovieListData.save";
	private Godot.Collections.Array<MovieEntry> MovieList = new();
	private VBoxContainer PageList;
	private MovieEntry CurrentMovie;
	private Label CurrentMovieLabel;

	private partial class FilePacket : Resource
	{
		public Array<MovieEntryData> PleaseGodPlease = new();
	}
	
	public override void _Ready()
	{
		CurrentMovieLabel = (Label) GetNode("VBoxContainer/HBoxContainer/CurrentMovieTitle");
		GenerateDB();
		
		SaveDataToFile();
		ClearScreen();
		ReadDataFromFile();
	}

	public void GenerateDB()
	{
		PageList = (VBoxContainer) GetNode("VBoxContainer/ScrollContainer/MainList");
		
		string LoadFile = FileAccess.GetFileAsString(MovieListIn);
		string[] NDString = LoadFile.Split("\n");

		foreach (string title in NDString)
		{
			if (title == String.Empty)
			{
				GD.Print("End of List");
			}

			else
			{
				MovieEntry NewEntry = (MovieEntry) MovieEntryScene.Instantiate();
				NewEntry.UpdateName(title);
				MovieList.Add(NewEntry);
				PageList.AddChild(NewEntry);
				
				//GD.Print($"{NewEntry.MovieName} Loaded!");
			}
			
		}
	}

	public void PickNewMovie()
	{
		Random RNGesus = new();
		int ListCount = MovieList.Count;
		int MovieIndex = RNGesus.Next(ListCount);
		MovieEntry Entry = MovieList[MovieIndex];
		

		CurrentMovieLabel.Text = $"Now watching: {Entry.Text}";
	}

	public void PickNewMovieAndReplace()
	{
		Random RNGesus = new();
		int ListCount = MovieList.Count;
		int MovieIndex = RNGesus.Next(ListCount);
		MovieEntry Entry = MovieList[MovieIndex];
		

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

	private void SaveDataToFileOld()
	{
		var File = FileAccess.Open(SaveFilePath, FileAccess.ModeFlags.Write);
		string StoreContent = JSON.Stringify(MovieList, "\t", true, false);

		File.StoreString(StoreContent);
		File.Dispose();
	}

	private void SaveDataToFile()
	{
		var File = FileAccess.Open(SaveFilePath, FileAccess.ModeFlags.Write);
		FilePacket FP = new();
		
		foreach (MovieEntry movie in MovieList)
		{
			var temp = movie.CreateSaveData();
			FP.PleaseGodPlease.Add(temp);
			GD.Print($"{movie.Text} committed >:(");
		}
		
		File.StoreVar(FP);
		File.Dispose();
		
	}

	private void ReadDataFromFile()
	{
		var File = FileAccess.Open(SaveFilePath, FileAccess.ModeFlags.Read);
		FilePacket Content = (FilePacket) File.GetVar();
		FilePacket FP = new();

		MovieEntry demo = new();
		demo.ProcessSaveData(Content.PleaseGodPlease[1]);

		AddChild(demo);
		// foreach (MovieEntryData entrydata in Content.PleaseGodPlease)
		// {
		// 	MovieEntry temp = new();
		// 	temp.ProcessSaveData(entrydata);
		// 	
		// 	PageList.AddChild(temp);
		// 	GD.Print($"{temp.Text}");
		// }
		
		File.Dispose();
	}

	private void ReadDataFromFileOld()
	{
		string Content = FileAccess.GetFileAsString(SaveFilePath);
		Array<MovieEntry> ParsedContent = (Array<MovieEntry>) JSON.ParseString(Content);

		foreach (MovieEntry movie in ParsedContent)
		{
			GD.Print($"{movie.Text} UNLOADED");
		}
	}
}
