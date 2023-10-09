using Godot;
using System;
using Godot.Collections;

public partial class SettingsConfig : Window
{
	[Signal] public delegate void SettingsConfigDialogueClosedEventHandler(Array<Variant> Settings, bool Write=false);

	private int User = -1;
	private bool AutoSaveToggle = false;
	private bool isOnlineToggle = false;

	public void UpdateUI(Array<Variant> SettingsArr)
	{
		OptionButton UserOptBtn = (OptionButton) GetNode("VBoxContainer/UserConfig/OptionButton");
		CheckButton SaveCheckBtn = (CheckButton) GetNode("VBoxContainer/AutosaveConfig/CheckButton");
		CheckButton OnlineCheckBtn = (CheckButton) GetNode("VBoxContainer/EnableOnlineConfig/CheckButton");

		UserOptBtn.Selected = (int) SettingsArr[0];
		SaveCheckBtn.Disabled = (bool) SettingsArr[1];
		OnlineCheckBtn.Disabled = (bool) SettingsArr[2];
		
		User = (int) SettingsArr[0];
		AutoSaveToggle = (bool) SettingsArr[1];
		isOnlineToggle = (bool) SettingsArr[2];
	}

	public void UpdateUI(int UserOpt)
	{
		OptionButton UserOptBtn = (OptionButton) GetNode("VBoxContainer/UserConfig/OptionButton");
		UserOptBtn.Selected = UserOpt;

		User = UserOpt;
	}
	
	public void UpdateUI(bool UserOpt, bool isSaveButton)
	{
		if (isSaveButton)
		{
			AutoSaveToggle = UserOpt;
			CheckButton SaveCheckBtn = (CheckButton) GetNode("VBoxContainer/AutosaveConfig/CheckButton");
			SaveCheckBtn.ButtonPressed = AutoSaveToggle;
		}
		else
		{
			isOnlineToggle = UserOpt;
			CheckButton OnlineCheckBtn = (CheckButton) GetNode("VBoxContainer/EnableOnlineConfig/CheckButton");
			OnlineCheckBtn.ButtonPressed = isOnlineToggle;
		}
	}

	public void CloseRequest()
	{
		Array<Variant> Settings = new();
		Settings.Add(User);
		Settings.Add(AutoSaveToggle);
		Settings.Add(isOnlineToggle);
		
		EmitSignal(SignalName.SettingsConfigDialogueClosed, Settings, true);
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
