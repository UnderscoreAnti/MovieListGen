using System;
using Godot;
using Godot.Collections;

public partial class WatchedMoviesUI : VBoxContainer
{
    [Signal] public delegate void UpdateStatusBarEventHandler(string Message);

    private PackedScene MovieEntryScene = (PackedScene) ResourceLoader.Load("uid://dm05gjkbn0umm");
    private PackedScene ReviewMovieDialogueScene = (PackedScene) ResourceLoader.Load("uid://sd1qgh31nw03");

    private VBoxContainer PageList;
    
    private Dictionary<int, ActiveRankMovieEntry> MovieDict = new();

    private int CurrentUser = -1;
    private int TempId = -1;

    public void GenerateScreenContent(Array<MovieEntryData> RequestedList)
    {
        PageList = (VBoxContainer) GetNode("ScrollContainer/MainList");
        
        foreach (MovieEntryData ElementData in RequestedList)
        {
            ActiveRankMovieEntry NewEntry = (ActiveRankMovieEntry) MovieEntryScene.Instantiate();
            NewEntry.SetCurrentUser(CurrentUser);
            NewEntry.ProcessMovieData(ElementData);
            NewEntry.GenerateText();
            NewEntry.OpenReviewDialogue += OpenMovieReviewDialogue;
            
            MovieDict.Add(NewEntry.MovieID, NewEntry);
            PageList.AddChild(NewEntry);
        }
    }


    public void SetCurrentUser(int User)
    {
        CurrentUser = User;
    }

    public void OpenMovieReviewDialogue(int MovieId)
    {
        RejectMovieDialogue ReviewDialogue = (RejectMovieDialogue) ReviewMovieDialogueScene.Instantiate();
        ReviewDialogue.RejectMovieDialogueClosed += OnReviewMovieDialogueClosed;
        
        if(MovieDict[MovieId].MovieReview != "Movie not reviewed")
            ReviewDialogue.PlaceTextInTextbox(MovieDict[MovieId].MovieReview);
        
        AddChild(ReviewDialogue);

        TempId = MovieId;
    }

    public void OnReviewMovieDialogueClosed(string InText)
    {
        if (InText == String.Empty)
        {
            EmitSignal(SignalName.UpdateStatusBar, "Operation aborted!");
        }
        else
        {
            MovieDict[TempId].UpdateReview(InText);
            EmitSignal(SignalName.UpdateStatusBar, "DIMENSION REPORT SUBMITTED");
        }
    }

}