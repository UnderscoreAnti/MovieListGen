using Godot;
using System;
using Godot.Collections;

public partial class ActiveRankMovieEntry : MovieEntry
{
	[Signal] public delegate void OpenReviewDialogueEventHandler(int Id);
	[Signal] public delegate void SendRankDataEventHandler(MovieEntryData Data);
	[Signal] public delegate void MouseReleasedEventHandler();
	
	private Label RankNumberNode;
	private Label MovieTitleNode;
	private Label MovieRePreNode;
	private Button RankMovieButton;
	private Timer AutoSaveTimer;

	private int CurrentUser = -1;

	public void SetCurrentUser(int Cu)
	{
		CurrentUser = Cu;
	}

	public override void GenerateText()
	{
		Dictionary<int, int> ProfileDict = new() {{0, Ranks[(int) SaveSystem.UsersEnum.Lenzo]}, 
			{1, Ranks[(int) SaveSystem.UsersEnum.Jason]}, 
			{2, Ranks[(int) SaveSystem.UsersEnum.Shai]}, 
			{3, Ranks[(int) SaveSystem.UsersEnum.Dev]}};
		
		RankNumberNode = (Label) GetNode("HBoxContainer/RankNumber");
		MovieTitleNode = (Label) GetNode("HBoxContainer/MovieTitle");
		MovieRePreNode = (Label) GetNode("HBoxContainer/MovieRePreview");
		RankMovieButton = (Button) GetNode("HBoxContainer/ReviewMovieButton");
		
		RankNumberNode.Text = GenerateMovieRank(ProfileDict[CurrentUser]);
		MovieTitleNode.Text = MovieTitle;
		MovieRePreNode.Text = GenerateMovieReviewPreview(MovieReview);

	}

	private string GenerateMovieRank(int Rating)
	{
		string Out = String.Empty;
		if(Rating < 10)
			Out = "00" + Rating;
		
		else if (Rating < 100)
			Out = "0" + Rating;

		else
			Out = Rating.ToString();
		
		return Out;
	}

	private string GenerateMovieReviewPreview(string Rev)
	{
		int ThreeLssThn32 = 29;
		string Out;
		
		if (Rev.Length > ThreeLssThn32)
		{
			string Shorty = Rev.Remove(0, ThreeLssThn32);
			Rev = Shorty;
		}

		Out = Rev + "...";
		return Out;
	}

	public void UpdateReview(string InText)
	{
		MovieRePreNode.Text = GenerateMovieReviewPreview(InText);
		MovieReview = InText;
	}

	public void OnModeToggled(bool isReviewMode)
	{
		if (isReviewMode)
			SetReviewMode();

		else
			SetRankMode();
	}

	public void SetReviewMode()
	{
		RankMovieButton.Text = "Review Movie";
		// RankMovieButton.ActionMode = BaseButton.ActionModeEnum.Release;
	}

	public void SetRankMode()
	{
		RankMovieButton.Text = "Rank Movie";
		// RankMovieButton.ActionMode = BaseButton.ActionModeEnum.Press;
	}

	public void OnMouseReleased()
	{
		// TODO: This sorry but *I* have more important things to do.
	}
	public void OnReviewMovieButtonPressed()
	{
		if(RankMovieButton.Text == "Review Movie")
			EmitSignal(SignalName.OpenReviewDialogue, MovieID);

		else
		{
			EmitSignal(SignalName.SendRankData, GenerateEntryData());
		}
	}

	public ActiveRankMovieEntry _GetDragData(Vector2 atPosition)
	{
		GD.Print($"Getting position of node: {Name}");
		ActiveRankMovieEntry Prev = (ActiveRankMovieEntry) Duplicate();

		Prev.Modulate = Colors.Transparent;
		SetDragPreview(Prev);

		return this; 
	}
}
