using Godot;
using System;
using Godot.Collections;

public partial class ActiveRankMovieEntry : MovieEntry
{
	[Signal] public delegate void OpenReviewDialogueEventHandler(int Id);
	
	private Label RankNumberNode;
	private Label MovieTitleNode;
	private Label MovieRePreNode;
	private Button RankMovieButton;

	private Dictionary<int, int> ProfileDict = new();

	private int CurrentUser = -1;

	public void SetCurrentUser(int Cu)
	{
		CurrentUser = Cu;
	}

	public override void GenerateText()
	{
		ProfileDict = new() {{0, LenzoRank}, {1, JasonRank}, {2, SharRank}, {3, GeneralRanking}};
		
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
		string Out = "Empty Str";
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
	
	public void OnReviewMovieButtonPressed()
	{
		EmitSignal(SignalName.OpenReviewDialogue, MovieID);
	}
}
