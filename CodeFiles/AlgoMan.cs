using System;
using Godot;


public class AlgoMan
{
    public void AdjustMovieEntryAlgo(int Cache, int Swap)
    {
        // Dictionary<int, string> studs; // TODO: Plug this in
        
        int Mult = Cache > Swap ? -1 : 1;
        int[] TempCache = {Cache, Swap};
        int Limit = (Swap - Cache) * Mult;

        int InternalX = Cache;
        for (int x = 0; x < Limit; x++)
        {
            InternalX += Mult;
        }
    }
}