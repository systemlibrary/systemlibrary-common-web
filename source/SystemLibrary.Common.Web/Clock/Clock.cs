using System;
using System.Diagnostics;

namespace SystemLibrary.Common.Web.Extensions;

public static class Clock
{
    public static void Measure(Action method, string message = null)
    {
        var sw = new Stopwatch();

        sw.Start();
        sw.Stop();
        sw.Reset();
        sw.Start();

        method();
        sw.Stop();
        Log.Debug(message + " executed in total time: " + sw.ElapsedTicks + " ticks, " + sw.ElapsedMilliseconds + "ms");
        sw.Reset();
    }
}
