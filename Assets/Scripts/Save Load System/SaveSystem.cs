using Assets.Scripts.Entities;
using System;
using UnityEngine;

public class SaveSystem
{
    private SaveData _save = new SaveData();
    private TimeHandler _time = new TimeHandler();
    public bool LoadLastSave()
    {
        if (PlayerPrefs.HasKey("Save"))
        {
            try
            {
                _save = JsonUtility.FromJson<SaveData>(Helper.Decrypt(PlayerPrefs.GetString("Save")));
                PushSavedData();
            }
            catch (Exception)
            { //some code
            }
            return true;
        }
            StartGame();
            return false;
    }
    private void PushSavedData()
    {
        Player.MaxLevel = _save.MaxLevel;
        Player.InfinityLevel = _save.InfinityLevel;
        Player.StarsNumber = _save.StarsNumber;
        Player.Vibration = _save.Vibration;
        Player.Volume = _save.Volume;
        Player.Language = _save.Language;
        Assets.SimpleLocalization.LocalizationManager.ChangeLanguage(Player.Language);
        _save.Stars.CopyTo(Player.StarsInLevel, 0);
        int[] gap =_time.MeasureEnergyGap(_save.Time);
        Player.Energy = _save.Energy + gap[0];
        Player.TimeLeftToAddEnergy = _save.TimeLeft + gap[1];
        Player.LastTimeClosed = _save.Time; 
    }

    public void SaveData()
    {
        _save.MaxLevel = Player.MaxLevel;
        _save.InfinityLevel = Player.InfinityLevel;
        _save.StarsNumber =  Player.StarsNumber;
        _save.Vibration = Player.Vibration;
        _save.Volume = Player.Volume;
        _save.Energy = Player.Energy;
        _save.TimeLeft = Player.TimeLeftToAddEnergy;
        _save.Time = _time.GetRealTime();
        _save.Language = Player.Language;
        Player.StarsInLevel.CopyTo(_save.Stars, 0);
        PlayerPrefs.SetString("Save", Helper.Encrypt(JsonUtility.ToJson(_save)));
    }

    private void StartGame()
    {
        Player.MaxLevel = 0;
        Player.InfinityLevel = 0;
        Player.StarsNumber = 0;
        Player.Volume = false;
        Player.Vibration = false;
        Player.StarsInLevel[0] = 0;
        Player.Energy = Constants.MaxEnergy;
        Player.TimeLeftToAddEnergy = Constants.TimeToAddEnergy;
        Player.LastTimeClosed = 0;
        Player.Language = 0;
    }


}
