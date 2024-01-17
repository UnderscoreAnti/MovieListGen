using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class WatchedMoviesUI : VBoxContainer
{
    [Signal] public delegate void UpdateStatusBarEventHandler(string Message);
    [Signal] public delegate void GroupSendDatatoDBEventHandler(Array<MovieEntryData> Group);
    [Signal] public delegate void GroupSendRankToDBEventHandler(int[] movIds, int user, int[] ranks);
    [Signal] public delegate void SendReviewToDBEventHandler(int movId, string rev);
    [Signal] public delegate void SendRankToDBEventHandler(int movId, int user, int rank);
    [Signal] public delegate void UIModeToggledEventHandler(bool isReviewMode);

    private PackedScene MovieEntryScene = (PackedScene) ResourceLoader.Load("uid://dm05gjkbn0umm");
    private PackedScene ReviewMovieDialogueScene = (PackedScene) ResourceLoader.Load("uid://sd1qgh31nw03");

    private VBoxContainer PageList;
    private Timer AutoSaveTimer;
    private Button SetModeButton;
    private Button SaveButton;
    private Label CurrentMovieCache;
    
    private Godot.Collections.Dictionary<int, ActiveRankMovieEntry> MovieDict = new();
    private Godot.Collections.Dictionary<int, ActiveRankMovieEntry> EditedEntries = new();
    private Godot.Collections.Dictionary<int, MovieEntryData> UnrankedMovies = new();

    private int CurrentUser = -1;
    private int TempId = -1;
    private bool isReviewMode = true;

    private MovieEntryData MovieCache = new();

    public void GenerateScreenContent(Array<MovieEntryData> RequestedList)
    {
        PageList = (VBoxContainer) GetNode("ScrollContainer/MainList");
        SetModeButton = (Button) GetNode("HBoxContainer/Rank");
        SaveButton = (Button) GetNode("HBoxContainer/Save");

        int LastEntryCheck = 0;
        foreach (MovieEntryData ElementData in RequestedList)
        {
            ActiveRankMovieEntry NewEntry = (ActiveRankMovieEntry) MovieEntryScene.Instantiate();
            NewEntry.SetCurrentUser(CurrentUser);
            NewEntry.ProcessMovieData(ElementData);
            NewEntry.GenerateText();
            NewEntry.OpenReviewDialogue += OpenMovieReviewDialogue;
            NewEntry.SendRankData += MoveRankItem;
            UIModeToggled += NewEntry.OnModeToggled;
            
            MovieDict.Add(NewEntry.MovieID, NewEntry);
            PageList.AddChild(NewEntry);
            LastEntryCheck++;
            
            if(LastEntryCheck >= RequestedList.Count)
                NewEntry.FinalRankEntryToggle();
        }
    }

    public void GetAutoSave()
    {
        AutoSaveTimer = (Timer) GetNode("Timer");
    }

    public void OnModeChanged()
    {
        Label MovCache = (Label) GetNode("HBoxContainer/CacheTitle");
        string[] Modes = {"RANK MODE", "REVIEW MODE"};
        bool Check = SetModeButton.Text == Modes[0];

        if (Check)
            OnRankModeEnabled();
        
        else
            SetModeButton.Text = Modes[0];

        // Check needs to be opposite of the button mode
        EmitSignal(SignalName.UIModeToggled, !Check);
        
        MovCache.Visible = Check;
    }

    public void OnSave()
    {
        int EntryCount = EditedEntries.Count;
        
        Array<MovieEntryData> Out = new();
        foreach (ActiveRankMovieEntry Entry in EditedEntries.Values)
        {
            Out.Add(Entry.GenerateEntryData());
        }

        EmitSignal(SignalName.GroupSendDatatoDB, Out);
    
        // int[] Ids = System.Array.Empty<int>();
        // int[] Ranks = System.Array.Empty<int>();
        //
        // foreach (ActiveRankMovieEntry Entry in EditedEntries.Values)
        // {
        //     MovieEntryData Nue = Entry.GenerateEntryData();
        //     Ids.Append(Entry.MovieID);
        //     Ranks.Append(Entry.Ranks[CurrentUser]);
        // }
        //
        // EmitSignal(SignalName.GroupSendRankToDB, Ids, CurrentUser, Ranks);
        
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

        if (AutoSaveTimer == (Timer) GetNode("Timer"))
        {
            AutoSaveTimer.Start();
        }
        
        EditedEntries.Add(TempId, MovieDict[TempId]);
    }

    public void MoveRankItem(MovieEntryData Data)
    {
        if(MovieCache == new MovieEntryData() || MovieCache == null)
            MovieCache = Data;
    }

    public void UpdateEditedNode(int Id)
    {
        if (EditedEntries.ContainsKey(Id))
        {
            EditedEntries[Id] = MovieDict[Id];
        }

        else
        {
            EditedEntries.Add(Id, MovieDict[Id]);
        }
    }

    public Godot.Collections.Dictionary<int, ActiveRankMovieEntry> GetEditedNodes()
    {
        return EditedEntries;
    }

    public void GetWatchedUnrankedMovies(Array<MovieEntryData> InData)
    {
        foreach (MovieEntryData MovData in InData)
        {
            UnrankedMovies.Add(MovData.MovieID, MovData);
        }
    }

    private void ChangeCacheTitle(MovieEntryData InData)
    {
        CurrentMovieCache = (Label) GetNode("HBoxContainer/CacheTitle");
        CurrentMovieCache.Text = InData.MovieTitle;
    }

    private void UnrankedWatchedMovieCheck()
    {
        // Lord forgive me for what follows but I am just so lazy 
        int[] DictKee = UnrankedMovies.Keys.ToArray();
        if (UnrankedMovies.Count > 0)
            ChangeCacheTitle(UnrankedMovies[DictKee[0]]);
    }

    private void OnRankModeEnabled()
    {
        SetModeButton.Text = "REVIEW MODE";
        UnrankedWatchedMovieCheck();
    }
}