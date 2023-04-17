using Godot;
using System;

public partial class MovieEntry : Label
{
	public bool AlreadyWatched = false;
	public bool IsFindable = true;

	public string MovieRejectReason;
	public string MovieReview = "Movie not reviewed";
	
	public int GeneralRanking;
	public int SharRank;
	public int LenzoRank;
	public int JasonRank;
	
	public void UpdateName(string NewName)
	{
		Text = NewName;
	}

	public MovieEntryData GenerateEntryData()
	{
		MovieEntryData NewData = new();

		NewData.MovieTitle = Text;
		NewData.MovieRejectReason = MovieRejectReason;
		NewData.AlreadyWatched = NewData.BoolToInt(AlreadyWatched);
		NewData.IsFinable = NewData.BoolToInt(IsFindable);
		NewData.MovieReview = MovieReview;
		
		NewData.GeneralMovieRanking = GeneralRanking;
		NewData.SharMovieRanking = SharRank;
		NewData.LenzoMovieRanking = LenzoRank;
		NewData.JasonMovieRanking = JasonRank;
		
		return NewData;
	}

	public void ProcessMovieData(MovieEntryData Data)
	{
		Text = Data.MovieTitle;
		MovieRejectReason = Data.MovieRejectReason;
		AlreadyWatched = Data.IntToBool(Data.AlreadyWatched);
		IsFindable = Data.IntToBool(Data.IsFinable);
		MovieReview = Data.MovieReview;

		GeneralRanking = Data.GeneralMovieRanking;
		SharRank = Data.SharMovieRanking;
		LenzoRank = Data.LenzoMovieRanking;
		JasonRank = Data.JasonMovieRanking;
	}

}
