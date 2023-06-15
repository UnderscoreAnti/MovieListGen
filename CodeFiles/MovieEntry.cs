using Godot;
using System;

public partial class MovieEntry : Label
{
	public int MovieID;

	public bool AlreadyWatched = false;
	public bool IsFindable = true;

	public string MovieRejectReason;
	public string MovieReview = "Movie not reviewed";
	public string MovieTitle;
	
	public int GeneralRanking;
	public int SharRank;
	public int LenzoRank;
	public int JasonRank;
	
	public void UpdateName(string NewName)
	{
		MovieTitle = NewName;
	}

	public void GenerateText()
	{
		Text = $"{MovieTitle}";

		if (AlreadyWatched)
		{
			Text += $" | RANK: {GeneralRanking}";
		}
	}
	

	public MovieEntryData GenerateEntryData()
	{
		MovieEntryData NewData = new();

		NewData.MovieTitle = MovieTitle;
		NewData.MovieRejectReason = MovieRejectReason;
		NewData.AlreadyWatched = NewData.BoolToInt(AlreadyWatched);
		NewData.IsFinable = NewData.BoolToInt(IsFindable);
		NewData.MovieReview = MovieReview;
		
		NewData.GeneralMovieRanking = GeneralRanking;
		NewData.SharMovieRanking = SharRank;
		NewData.LenzoMovieRanking = LenzoRank;
		NewData.JasonMovieRanking = JasonRank;

		NewData.MovieID = MovieID;
		
		return NewData;
	}

	public void ProcessMovieData(MovieEntryData Data)
	{
		MovieTitle = Data.MovieTitle;
		MovieRejectReason = Data.MovieRejectReason;
		AlreadyWatched = Data.IntToBool(Data.AlreadyWatched);
		IsFindable = Data.IntToBool(Data.IsFinable);
		MovieReview = Data.MovieReview;

		SharRank = Data.SharMovieRanking;
		LenzoRank = Data.LenzoMovieRanking;
		JasonRank = Data.JasonMovieRanking;

		GeneralRanking = GenerateGeneralRank(JasonRank, LenzoRank, SharRank);
		
		MovieID = Data.MovieID;
	}

	private int GenerateGeneralRank(int JR, int LR, int SR)
	{
		return (JR + LR + SR) / 3;
	}

	public void GeneralRankUpdated(int JR, int LR, int SR)
	{
		GeneralRanking = GenerateGeneralRank(JR, LR, SR);
	}

}
