using Godot;
using System;

public partial class RejectMovieDialogue : Window
{
	[Signal] public delegate void RejectMovieDialogueClosedEventHandler(string OutText);
	
	private TextEdit TextBox;
	public override void _Ready()
	{
		TextBox = (TextEdit) GetNode("MarginContainer/TextEdit");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void CloseRejectMovieDialogue()
	{
		if (TextBox.Text == String.Empty)
		{
			TextBox.PlaceholderText = "Please enter a reason to reject the movie";
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
}
