using Godot;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;


public partial class SaveSystem : Control
{
	private IDbConnection SQLiteConnection;
	private IDbCommand CommandOutput;

	public override void _Ready()
	{
		if (File.Exists("SaveFile.db"))
		{
			SQLiteConnection = new SqlConnection("Data Source=SaveFile.db");
			CommandOutput = SQLiteConnection.CreateCommand();
		}

		else
		{
			File.Create("SaveFile.db").Dispose();

			SQLiteConnection = new SqlConnection("Data Source=SaveFile.db");
			CommandOutput = SQLiteConnection.CreateCommand();
		}
	}

	public override void _ExitTree()
	{
		SQLiteConnection.Dispose();
	}

	public void AddMovieEntryToDB()
	{
		//
	}
	
	public override void _Process(double delta)
	{
	}
}
