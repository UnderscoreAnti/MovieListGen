using Godot;
using System;

public partial class MovieEntry : MarginContainer
{
	[Export()] public Godot.Collections.Array<Label> TextNodes = new();
	
	public int MovieID;

	public bool AlreadyWatched = false;
	public bool IsFindable = true;

	public string MovieRejectReason;
	public string MovieReview = "Movie not reviewed";
	public string MovieTitle;

	public int[] Ranks = new int[4];
	
	public virtual void GenerateText()
	{
		GD.Print($"Empty Generate Text Function in Type: {GetType().ToString()}");
	}
	
	public void UpdateName(string NewName)
	{
		MovieTitle = NewName;
	}

	public MovieEntryData GenerateEntryData()
	{
		MovieEntryData NewData = new();

		NewData.MovieTitle = MovieTitle;
		NewData.MovieRejectReason = MovieRejectReason;
		NewData.AlreadyWatched = NewData.BoolToInt(AlreadyWatched);
		NewData.IsFinable = NewData.BoolToInt(IsFindable);
		NewData.MovieReview = MovieReview;
		
		NewData.LenzoMovieRanking = Ranks[(int) SaveSystem.UsersEnum.Lenzo];
		NewData.JasonMovieRanking = Ranks[(int) SaveSystem.UsersEnum.Jason];
		NewData.ShaiMovieRanking = Ranks[(int) SaveSystem.UsersEnum.Shai];
		NewData.GeneralMovieRanking = Ranks[(int) SaveSystem.UsersEnum.Lenzo];

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

		Ranks[(int) SaveSystem.UsersEnum.Shai] = Data.ShaiMovieRanking;
		Ranks[(int) SaveSystem.UsersEnum.Lenzo] = Data.LenzoMovieRanking;
		Ranks[(int) SaveSystem.UsersEnum.Jason] = Data.JasonMovieRanking;

		Ranks[(int) SaveSystem.UsersEnum.Dev] = GenerateGeneralRank(Ranks[(int) SaveSystem.UsersEnum.Jason], 
			Ranks[(int) SaveSystem.UsersEnum.Lenzo], Ranks[(int) SaveSystem.UsersEnum.Shai]);
		
		MovieID = Data.MovieID;
	}

	private int GenerateGeneralRank(int JR, int LR, int SR)
	{
		return (JR + LR + SR) / 3;
	}

	public void GeneralRankUpdated(int JR, int LR, int SR)
	{
		Ranks[(int) SaveSystem.UsersEnum.Dev] = GenerateGeneralRank(JR, LR, SR);
	}

}
