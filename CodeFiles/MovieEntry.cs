using Godot;
using System;

public partial class MovieEntry : Label
{
	public bool AlreadyWatched = false;
	public bool IsFindable = true;
	public int MovieRating;

	public string MovieRejectReason;
	public string MovieReview = "Movie not reviewed";
	
	public void UpdateName(string NewName)
	{
		Text = NewName;
	}

	public MovieEntryData CreateSaveData()
	{
		MovieEntryData Save = new(Text, MovieRating, IsFindable, AlreadyWatched, MovieReview, MovieRejectReason);
		return Save;
	}

	public void ProcessSaveData(MovieEntryData InData)
	{
		Text = InData.MovieName;
		MovieRating = InData.MovieRating;
		IsFindable = InData.IsFindable;
		AlreadyWatched = InData.AlreadyWatched;
		MovieReview = InData.MovieReview;

		if (InData.MovieRejectReason != String.Empty)
		{
			MovieRejectReason = InData.MovieRejectReason;
		}
	}

}
