using Godot;
using System;
using System.Collections.Concurrent;

public partial class MovieEntryData : Resource
{
	public int MovieID;
	
	public int AlreadyWatched;
	public int IsFinable;
	public int[] Ranks = new int[4];
	
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
	
	public void ConvertFromDB(long movID, long isWatched, long isFindable,
							string movRej, string movRev, long gRank, long lRank,
							long jRank, long sRank, string movTitle)
	{
		MovieID = Convert.ToInt32(movID);
		
		AlreadyWatched = Convert.ToInt32(isWatched);
		MovieRejectReason = movRej;
		MovieReview = movRev;
		IsFinable = Convert.ToInt32(isFindable); 
		Ranks[(int) SaveSystem.UsersEnum.Dev] = Convert.ToInt32(gRank);
		Ranks[(int) SaveSystem.UsersEnum.Lenzo] = Convert.ToInt32(lRank);
		Ranks[(int) SaveSystem.UsersEnum.Jason] = Convert.ToInt32(jRank);
		Ranks[(int) SaveSystem.UsersEnum.Shai] = Convert.ToInt32(sRank);
	
		MovieTitle = movTitle;
	}

	public void QuickAddRank(int Rank, int User=-1)
	{
		if (User == -1)
		{
			Ranks[(int) SaveSystem.UsersEnum.Dev] = Rank;
			Ranks[(int) SaveSystem.UsersEnum.Lenzo] = Rank;
			Ranks[(int) SaveSystem.UsersEnum.Jason] = Rank;
			Ranks[(int) SaveSystem.UsersEnum.Shai] = Rank;
		}

		else
			Ranks[User] = Rank;
	}
	
}
