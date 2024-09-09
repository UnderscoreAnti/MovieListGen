using System;
using Godot;

public class DebugDB
{
    private bool WillFlush = false;
    private string FlushPath = string.Empty; 
    
    public DebugDB(bool FlushToFile = false, string FlushFilePath = "")
    {
        WillFlush = FlushToFile;
        FlushPath = FlushFilePath;
    }
    
    public void PrintToLog(MovieEntryData CurrData, bool WriteToConsole = false)
    {
        
    }

    public void PrintToFile(MovieEntryData CurrData, bool WriteToFile = false)
    {
        
    }
}