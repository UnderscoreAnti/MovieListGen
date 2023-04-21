using Godot;
using System;

public partial class MovieEntryData : Resource
{
	public int MovieID;
	
	public int AlreadyWatched;
	public int IsFinable;
	public int GeneralMovieRanking;
	public int SharMovieRanking;
	public int LenzoMovieRanking;
	public int JasonMovieRanking;

	public string MovieTitle;
	public string MovieRejectReason;
	public string MovieReview;

	public int BoolToInt(bool ConvertVal)
	{
		if (ConvertVal == true)
		{
			return 1;
		}
		else
		{
			return 0;
		}
	}

	public bool IntToBool(int ConvertVal)
	{
		return ConvertVal == 1;
	}
}
