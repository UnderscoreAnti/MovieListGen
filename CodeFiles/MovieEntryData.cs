using Godot;
using System;
using System.Collections.Concurrent;

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

	public bool IntToBool(long ConvertVal)
	{
		return ConvertVal == 1;
	}

	// public void ConvertFromDB(long movID, long isWatched, long isFindable,
	// 	string movRej, string movRev, long gRank, long lRank,
	// 	long jRank, long sRank, string movTitle)
	
	public void ConvertFromDB(long movID, long isWatched, long isFindable,
							string movRej, string movRev, long gRank, long lRank,
							long jRank, long sRank, string movTitle)
	{
		MovieID = Convert.ToInt32(movID);
		
		AlreadyWatched = Convert.ToInt32(isWatched);
		MovieRejectReason = movRej;
		MovieReview = movRev;
		IsFinable = Convert.ToInt32(isFindable);
		GeneralMovieRanking = Convert.ToInt32(gRank);
		LenzoMovieRanking = Convert.ToInt32(lRank);
		JasonMovieRanking = Convert.ToInt32(jRank);
		SharMovieRanking = Convert.ToInt32(sRank);
	
		MovieTitle = movTitle;
	}
}
