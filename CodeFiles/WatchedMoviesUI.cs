using System;
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
    
    private Dictionary<int, ActiveRankMovieEntry> MovieDict = new();
    private Dictionary<int, ActiveRankMovieEntry> EditedEntries = new();

    private int CurrentUser = -1;
    private int TempId = -1;
    private bool isReviewMode = true;

    private MovieEntryData MovieCache = new();

    public void GenerateScreenContent(Array<MovieEntryData> RequestedList)
    {
        PageList = (VBoxContainer) GetNode("ScrollContainer/MainList");
        SetModeButton = (Button) GetNode("HBoxContainer/Rank");
        SaveButton = (Button) GetNode("HBoxContainer/Save");
        
        
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
        }
    }

    public void GetAutoSave()
    {
        AutoSaveTimer = (Timer) GetNode("Timer");
    }

    public void OnModeChanged()
    {
        string[] Modes = {"RANK MODE", "REVIEW MODE"};
        bool Check = SetModeButton.Text == Modes[0];

        if (Check)
            SetModeButton.Text = Modes[1];
        else
            SetModeButton.Text = Modes[0];
        
        // Check needs to be opposite of the button mode
        EmitSignal(SignalName.UIModeToggled, !Check);
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
    
    // TODO: Put that function back lmaooooooooo

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

    public Dictionary<int, ActiveRankMovieEntry> GetEditedNodes()
    {
        return EditedEntries;
    }

}