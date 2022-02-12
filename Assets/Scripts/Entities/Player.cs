
public static class Player
{
    public static int Level, StarsNumber, InfinityLevel, MaxLevel, Energy, TimeLeftToAddEnergy, Language;
    public static bool Volume, Vibration;
    public static short[] StarsInLevel = new short[100];
    public static long LastTimeClosed;

    public const int TIMEToAddEnergy = 150, MAXEnergy = 10, MAXLanguage = 2;
}
