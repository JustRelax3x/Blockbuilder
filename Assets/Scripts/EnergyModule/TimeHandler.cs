using System;
using UnityEngine;

public class TimeHandler
{

    public int[] MeasureEnergyGap(long lastTime)
    {
        int timegap = MeasureTimeGap(lastTime);
        int timeToAddEnergy = Player.TIMEToAddEnergy;
        return new int[2] {timegap / timeToAddEnergy, timegap%timeToAddEnergy};
    }
    public int MeasureTimeGap(long lastTime)
    {
        long gap = GetRealTime() - lastTime;
        if (gap < 0) //phone was restarted
        {
            gap = Player.TIMEToAddEnergy * Player.MAXEnergy;
        }
        return (int)gap;
    }

    public long GetRealTime()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaObject jo = new AndroidJavaObject("android.os.SystemClock");
        long time = jo.CallStatic<long>("elapsedRealtime");
        long timeSec = time / 1000;
#elif UNITY_STANDALONE || UNITY_EDITOR
        int time = Environment.TickCount;

        if (time < 0)
            time = int.MaxValue + Environment.TickCount;

        int timeSec = time / 1000;
#endif
        return timeSec;
    }
}
