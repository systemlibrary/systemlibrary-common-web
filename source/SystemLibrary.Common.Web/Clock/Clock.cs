using System;
using System.Diagnostics;

namespace SystemLibrary.Common.Web.Extensions;

public static class Clock
{
    public static long Measure(Action method, string message = null)
    {
        var sw = new Stopwatch();

        sw.Start();
        sw.Stop();
        sw.Reset();
        sw.Start();

        method();

        sw.Stop();

        if(message != null)
            Log.Debug(message + " executed in total time: " + sw.ElapsedTicks + " ticks, " + sw.ElapsedMilliseconds + "ms");
        
        var ms = sw.ElapsedMilliseconds;
        
        sw.Reset();
        
        return ms;
    }
}
