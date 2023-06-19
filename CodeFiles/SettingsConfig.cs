using Godot;
using System;

public partial class SettingsConfig : Window
{
	[Signal] public delegate void SettingsConfigDialogueClosedEventHandler();

	[Signal] public delegate void UserSelectedEventHandler(int UserIndex);
	
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void CloseRequest()
	{
		EmitSignal(SignalName.SettingsConfigDialogueClosed);
		QueueFree();
	}

	public void OnUserSelect(int Index)
	{
		EmitSignal(SignalName.UserSelected, Index);
	}
}
