using Godot;
using System;

public partial class RejectMovieDialogue : Window
{
	[Signal] public delegate void RejectMovieDialogueClosedEventHandler(string OutText);
	
	private TextEdit TextBox;
	private bool isRejectBox = false;
	public override void _Ready()
	{
		TextBox = (TextEdit) GetNode("MarginContainer/TextEdit");
		isRejectBox = Title == "Reject Movie?";
		
		if (TextBox.Text == String.Empty && isRejectBox)
		{
			TextBox.PlaceholderText = "Movie is rejected because...";
		}

		else
		{
			TextBox.PlaceholderText = "This movie was...";
		}
	}

	public void CloseRejectMovieDialogue()
	{
		if (TextBox.Text == String.Empty && isRejectBox)
		{
			TextBox.PlaceholderText = "Please enter a reason to reject the movie";
		}

		else if (TextBox.Text == String.Empty && !isRejectBox)
		{
			TextBox.PlaceholderText = "Please enter a review for the movie.";
		}

		else
		{
			CloseRequest();
		}
	}

	public void CloseRequest()
	{
		EmitSignal(SignalName.RejectMovieDialogueClosed, TextBox.Text);
		QueueFree();
	}

	public void PlaceTextInTextbox(string Text)
	{
		TextEdit TE = (TextEdit) GetNode("MarginContainer/TextEdit");
		TE.Text = Text;
	}
}
