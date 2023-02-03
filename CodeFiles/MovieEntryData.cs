using Godot;
using System;

public partial class MovieEntryData : Resource
{
    public bool AlreadyWatched = false;
    public bool IsFindable = true;
    public string MovieName;
    public int MovieRating;

    public string MovieRejectReason;
    public string MovieReview = "Movie not reviewed";

    public MovieEntryData(string name, int rating, bool findable, bool watched, string review, string rejection="")
    {
        MovieName = name;
        MovieRating = rating;
        IsFindable = findable;
        AlreadyWatched = watched;
        MovieReview = review;
        MovieRejectReason = rejection;
    }
}
