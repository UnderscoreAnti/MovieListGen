using Godot;
using System;
using Godot.Collections;

public partial class UnwatchedMovieEntry : MovieEntry
{
    public override void GenerateText()
    {
        if (TextNodes != new Array<Label>())
        {
            Label EntryText = (Label) GetNode("Label");
            EntryText.Text = $"{MovieTitle}";

            if (AlreadyWatched)
            {
                EntryText.Text += $" | RANK: {GeneralRanking}";
            }
        }

        else
        {
            GD.Print($"No Labels in Array in NodeType: {GetType()} | Node: {Name}");
        }
    }

}