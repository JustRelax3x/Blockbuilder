using TMPro;
using UnityEngine;
using System;
using Assets.SimpleLocalization;

public class EnergyUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _energy;
    [SerializeField]
    private TextMeshProUGUI _timeLeft;
    [SerializeField]
    private RewardedAds _adRewarded10;

    public void Energy(int energy, int maxEnergy)
    {
        _energy.text = $"{energy}/{maxEnergy}";
    }
    public void Time(int timeLeft)
    {
        if(timeLeft < 0)
        {
            _timeLeft.text = LocalizationManager.Localize("full");
            return;
        }
        _timeLeft.text = string.Format("{0:00}:{1:00}", timeLeft/60, timeLeft%60);
    }
    public void ActivateAd(Action EarnedReward)
    {
        _adRewarded10.gameObject.SetActive(true);
        _adRewarded10.Earned += EarnedReward;
    }

    public void DisableAd()
    {
        _adRewarded10.Earned = null;
        _adRewarded10.gameObject.SetActive(false);
    }

}