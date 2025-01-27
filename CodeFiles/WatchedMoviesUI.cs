using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
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
    [Signal] public delegate void UnrankedCacheClearedEventHandler();
    [Signal] public delegate void EditRankButtonPressedEventHandler();
    
    private PackedScene MovieEntryScene = (PackedScene) ResourceLoader.Load("uid://dm05gjkbn0umm");
    private PackedScene ReviewMovieDialogueScene = (PackedScene) ResourceLoader.Load("uid://sd1qgh31nw03");

    private VBoxContainer PageList;
    private Timer AutoSaveTimer;
    private Button SetModeButton;
    private Button SaveButton;
    private Label CurrentMovieTitleCache;
    private int CurrentMovieIDCache;
    
    private Godot.Collections.Dictionary<int, ActiveRankMovieEntry> MovieDict = new();
    private Godot.Collections.Dictionary<int, ActiveRankMovieEntry> EditedEntries = new();
    private Godot.Collections.Dictionary<int, MovieEntryData> UnrankedMovies = new();

    private int CurrentUser = -1;
    private int TempId = -1;
    private bool isReviewMode = true;

    private MovieEntryData MovieCache = new();
    private ActiveRankMovieEntry HiddenEntry = new();

    public void GenerateScreenContent(Array<MovieEntryData> RequestedList, bool isFirstSetup=true)
    {
        
        if (isFirstSetup)
        {
            PageList = (VBoxContainer) GetNode("ScrollContainer/MainList");
            SetModeButton = (Button) GetNode("HBoxContainer/Rank");
            SaveButton = (Button) GetNode("HBoxContainer/Save");

            UnrankedCacheCleared += ChangeCacheTitle;
        }

        int LastEntryCheck = 0;
        foreach (MovieEntryData ElementData in RequestedList)
        {
            ActiveRankMovieEntry NewEntry = PrepNode(ElementData);
            
            if(MovieDict.ContainsKey(NewEntry.MovieID))
                MovieDict[NewEntry.MovieID] = NewEntry;

            else
                MovieDict.Add(NewEntry.MovieID, NewEntry);
            
            
            NewEntry.UpdateColor();
            PageList.AddChild(NewEntry);
            LastEntryCheck++;
            
            if(LastEntryCheck >= RequestedList.Count)
                NewEntry.FinalRankEntryToggle();
        }
    }

    private ActiveRankMovieEntry PrepNode(MovieEntryData InData)
    {
        ActiveRankMovieEntry TempEntry = (ActiveRankMovieEntry) MovieEntryScene.Instantiate();
        
        TempEntry.SetCurrentUser(CurrentUser);
        TempEntry.ProcessMovieData(InData);
        TempEntry.GenerateText();
        
        TempEntry.OpenReviewDialogue += OpenMovieReviewDialogue;
        TempEntry.SendRankData += MoveRankItem;
        TempEntry.RankAbovePressed += OnRankAbovePressed;
        TempEntry.RankBelowPressed += OnRankBelowPressed;
        TempEntry.EditRankPressed += EditRankPressed;
        UIModeToggled += TempEntry.OnModeToggled;
        UnrankedCacheCleared += TempEntry.RankCacheCleared;
        EditRankButtonPressed += TempEntry.EditRankSwitchToRank;

        if(isReviewMode == false)
            TempEntry.OnModeToggled(isReviewMode);
        
        return TempEntry;
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
        CurrentMovieTitleCache = (Label) GetNode("HBoxContainer/CacheTitle");
        CurrentMovieTitleCache.Text = InData.MovieTitle;
        CurrentMovieIDCache = InData.MovieID;
    }

    private void ChangeCacheTitle()
    {
        CurrentMovieTitleCache = (Label) GetNode("HBoxContainer/CacheTitle");
        CurrentMovieTitleCache.Text = "No More Movies to Rank!";
        CurrentMovieIDCache = -1;
    }

    private void UnrankedWatchedMovieCheck()
    {
        // Lord forgive me for what follows but I am just so lazy 
        int[] DictKee = UnrankedMovies.Keys.ToArray();
        if (UnrankedMovies.Count > 0)
            ChangeCacheTitle(UnrankedMovies[DictKee[0]]);

        else
            EmitSignal(SignalName.UnrankedCacheCleared);
    }

    private void OnRankModeEnabled()
    {
        SetModeButton.Text = "REVIEW MODE";
        isReviewMode = false;
        UnrankedWatchedMovieCheck();
    }

    private void OnRankAbovePressed(MovieEntryData Data)
    {
        if (HiddenEntry.MovieID == 0)
            RankAboveNewRankMode(Data); 

        else
            RankAboveEditRankMode(Data);
    }
    
    public void RankAboveEditRankMode(MovieEntryData Data)
    {
        EmitSignal(SignalName.UpdateStatusBar, "UPDATING TEMPORARY DIMENSIONAL RANKS...");
        
        Array<Node> RawPageList = PageList.GetChildren();
        int RankThreshold = Data.Ranks[CurrentUser] - 1;
        
        Array<Node> PreppedPageList = RawPageList.Slice(RankThreshold);
        Array<ActiveRankMovieEntry> CookedPageList = new();
        
        foreach (Node PreppedNode in PreppedPageList)
        {
            ActiveRankMovieEntry CookedEntry = (ActiveRankMovieEntry) PreppedNode;
            
            if(CookedEntry.MovieID != HiddenEntry.MovieID)
                CookedPageList.Add(CookedEntry);
        }

        ActiveRankMovieEntry CookedCache = new();
        UnrankedMovies[CurrentMovieIDCache].Ranks[CurrentUser] = Data.Ranks[CurrentUser];
        
        int RankInterator = 0;
        CookedCache = PrepNode(UnrankedMovies[CurrentMovieIDCache]);
        PageList.AddChild(CookedCache);
        PageList.MoveChild(CookedCache, RankThreshold);
        CookedCache = HiddenEntry;
        CookedCache.Ranks[CurrentUser] = Data.Ranks[CurrentUser];
        CookedCache.GenerateText();
        CookedCache.UpdateColor();
        CookedCache.Visible = true;
        
        int LastEntryCheck = 0;
        foreach (Node SlicedEntry in PreppedPageList)
        {
            ActiveRankMovieEntry QuickSwitch = (ActiveRankMovieEntry) SlicedEntry;
            if (QuickSwitch == HiddenEntry)
            {
                GD.Print("This is a flag!");
                continue;
            }
            
            QuickSwitch.Ranks[CurrentUser] += 1;
            QuickSwitch.UpdateColor();
            
            QuickSwitch.GenerateText();
            
            if(LastEntryCheck >= PreppedPageList.Count)
                QuickSwitch.FinalRankEntryToggle();
        } 
        
        
        EmitSignal(SignalName.UpdateStatusBar, "DIMENSIONAL RANKS UPDATED!");
        UnrankedMovies.Remove(CurrentMovieIDCache);
        UnrankedWatchedMovieCheck();
        
        GD.Print("Poopoo");
    }
    
    public void RankAboveNewRankMode(MovieEntryData Data)
    {
        EmitSignal(SignalName.UpdateStatusBar, "UPDATING TEMPORARY DIMENSIONAL RANKS...");
        
        Array<Node> RawPageList = PageList.GetChildren();
        UnrankedMovies[CurrentMovieIDCache].Ranks[CurrentUser] = Data.Ranks[CurrentUser];
        
        int RankThreshold = Data.Ranks[CurrentUser] - 1;
        int NewRank = Data.Ranks[CurrentUser];

        Array<Node> PreppedPageList = RawPageList.Slice(RankThreshold);

        ActiveRankMovieEntry CookedCache = new();
        
        int RankInterator = 0;
        CookedCache = PrepNode(UnrankedMovies[CurrentMovieIDCache]);
        PageList.AddChild(CookedCache);
        
        int LastEntryCheck = 0;
        foreach (Node SlicedEntry in PreppedPageList)
        {
            ActiveRankMovieEntry QuickSwitch = (ActiveRankMovieEntry) SlicedEntry;
            QuickSwitch.Ranks[CurrentUser] += 1;
            QuickSwitch.UpdateColor();
            
            QuickSwitch.GenerateText();
            
            if(LastEntryCheck >= PreppedPageList.Count)
                QuickSwitch.FinalRankEntryToggle();
            
        } 
        
        PageList.MoveChild(CookedCache, RankThreshold);
        CookedCache.UpdateColor();
        
        EmitSignal(SignalName.UpdateStatusBar, "DIMENSIONAL RANKS UPDATED!");
        UnrankedMovies.Remove(CurrentMovieIDCache);
        UnrankedWatchedMovieCheck();
    }
    
    private void EditRankPressed(MovieEntryData Data)
    {
        UnrankedMovies.Add(Data.MovieID, Data);
        UnrankedWatchedMovieCheck();
        
        Array<Node> EntryList = PageList.GetChildren();
        
        int RankThreshold = Data.Ranks[CurrentUser] - 1;
        Array<Node> CookedEntryList = EntryList.Slice(RankThreshold);
        Node SelectedNode = EntryList[RankThreshold];
        ActiveRankMovieEntry SelectedEntry = (ActiveRankMovieEntry) SelectedNode;
        SelectedEntry.Visible = false;
        HiddenEntry = SelectedEntry;


        int LastEntryCheck = 0;
        foreach (Node SlicedEntry in CookedEntryList)
        {
            ActiveRankMovieEntry QuickSwitch = (ActiveRankMovieEntry) SlicedEntry;
            QuickSwitch.Ranks[CurrentUser] -= 1;
            
            QuickSwitch.UpdateColor();
            QuickSwitch.GenerateText();
            if(LastEntryCheck >= CookedEntryList.Count)
                QuickSwitch.FinalRankEntryToggle();
        }

        EmitSignal(SignalName.EditRankButtonPressed);
    }
    
    private void OnRankBelowPressed(MovieEntryData Data)
    {
        EmitSignal(SignalName.UpdateStatusBar, "UPDATING TEMPORARY DIMENSIONAL RANKS...");
        PageList = (VBoxContainer) GetNode("ScrollContainer/MainList");
        
        MovieDict[Data.MovieID].FinalRankEntryToggle();

        ActiveRankMovieEntry Entry = new();
        bool isNotActiveHiddenEntry = HiddenEntry.Visible;
        
        if (isNotActiveHiddenEntry)
            Entry = PrepNode(UnrankedMovies[CurrentMovieIDCache]);

        else
            Entry = HiddenEntry;

        Entry.Ranks[CurrentUser] = Data.Ranks[CurrentUser];
        Entry.GenerateText();
        Entry.UpdateColor();
        Entry.OnModeToggled(false);
        Entry.FinalRankEntryToggle();
        
        PageList.AddChild(Entry);
        UnrankedMovies.Remove(CurrentMovieIDCache);
        UnrankedWatchedMovieCheck();
        EmitSignal(SignalName.UpdateStatusBar, "DIMENSIONAL RANKS UPDATED!");
        
        ClearHiddenEntry();
    }

    private void UpdateEditedEntries(ActiveRankMovieEntry Entry)
    {
            if(EditedEntries.ContainsKey(Entry.MovieID))
                EditedEntries[Entry.MovieID] = Entry;

            else
                EditedEntries.Add(Entry.MovieID, Entry);
    }
    
    private void ClearHiddenEntry()
    {
        HiddenEntry = new ActiveRankMovieEntry();
    }
}
