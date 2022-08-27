using Assets.SimpleLocalization;
using System;
using TMPro;
using UnityEngine;

public class EnergyUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _energy;

    [SerializeField]
    private TextMeshProUGUI _timeLeft;

    [SerializeField]
    private RewardedAds _adRewarded10;

    [SerializeField]
    private GameObject _energyParent;

    //PausePanel
    [SerializeField]
    private TextMeshProUGUI _energyPause;

    [SerializeField]
    private TextMeshProUGUI _timeLeftPause;

    [SerializeField]
    private RewardedAds _adRewarded10Pause;

    [SerializeField]
    private GameObject _energyParentPause;    

    public void Energy(int energy, int maxEnergy)
    {
        _energy.text = $"{energy}/{maxEnergy}";
        _energyPause.text = $"{energy}/{maxEnergy}";
    }

    public void Time(int timeLeft)
    {
        if (timeLeft < 0)
        {
            string text = LocalizationManager.Localize("full");
            _timeLeft.text = text;
            _timeLeftPause.text = text;
            return;
        }
        if (HasNoActiveUI()) return;
        var time = GetTime();
        time.text = string.Format("{0:00}:{1:00}", timeLeft / 60, timeLeft % 60);
    }

    public void ActivateAd(Action EarnedReward)
    {
        if (!_adRewarded10.gameObject.activeInHierarchy)
        {
            _adRewarded10.gameObject.SetActive(true);
            _adRewarded10.Earned += EarnedReward;
        }
        if (_adRewarded10Pause.gameObject.activeInHierarchy) return;
        _adRewarded10Pause.gameObject.SetActive(true);
        _adRewarded10Pause.Earned += EarnedReward;
    }

    public void DisableAd()
    {
        var ad = GetAds();
        ad.Earned = null;
        ad.gameObject.SetActive(false);
    }

private bool HasNoActiveUI() 
    {
        return !(_energyParent.activeInHierarchy || _energyParentPause.activeInHierarchy);
    } 

    private TextMeshProUGUI GetTime()
    {
        if (_energyParent.activeInHierarchy) return _timeLeft;
        return _timeLeftPause;
    }
    
    private RewardedAds GetAds()
    {
        if (_energyParent.activeInHierarchy) return _adRewarded10;
        return _adRewarded10Pause;
    }
}