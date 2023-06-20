using Godot;
using System;

public partial class SettingsConfig : Window
{
	[Signal] public delegate void SettingsConfigDialogueClosedEventHandler(int User, bool AutoSave, bool IsOnline);

	private int User;
	private bool AutoSaveToggle;
	private bool isOnlineToggle;

	public void CloseRequest()
	{
		EmitSignal(SignalName.SettingsConfigDialogueClosed, User, AutoSaveToggle, isOnlineToggle);
		QueueFree();
	}

	public void OnUserSelect(int Index)
	{
		User = Index;
	}

	public void OnAutoSaveToggled(bool Toggle)
	{ 
		AutoSaveToggle = Toggle;
	}

	public void OnOnlineToggled(bool Toggle)
	{
		isOnlineToggle = Toggle;
	}
}
