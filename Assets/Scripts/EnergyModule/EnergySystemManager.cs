using Assets.Scripts.Entities;
using UnityEngine;

namespace Assets.Scripts
{
    public class EnergySystemManager : MonoBehaviour
    {
        [SerializeField]
        private EnergyUI _energyUI;

        private EnergyModel _energyModel;

        private float _unscaledTime = 0;

        private bool _adIsActivated = false;


        public void Initialize(int energy, int timeToAddEnergy, int maxEnergy, int timeLeft)
        {
            int extraEnergy = timeLeft / timeToAddEnergy + energy;
            if (_energyModel == null)
            {
                _energyModel = new EnergyModel(extraEnergy, timeToAddEnergy, maxEnergy, timeLeft % timeToAddEnergy);
            }
            else
            {
                _energyModel.AddEnergy(extraEnergy);
            } 
            _energyModel._energyChanged += EnergyValueUpdater;
            _energyModel._timeLeftChanged += TimeLeftUpdater;
            ActivateUI();
        }

        public bool TryUseEnergy(int amount = 1)
        {
            if (amount <= 0) return false;
            if (_energyModel.TryUseEnergy(amount))
            {
                return true;
            }
            _energyUI.ActivateAd(EarnedEnergyReward);
            return false;
        }

        public void ActivateUI()
        {
            EnergyValueUpdater();
            TimeLeftUpdater();
            if (_energyModel.Energy == 0 && !_adIsActivated)
            {
                _energyUI.ActivateAd(EarnedEnergyReward);
                _adIsActivated = true;
            }
        }

        private void Update()
        {
            if (_energyModel.IsFull) return;
            _unscaledTime += Time.unscaledDeltaTime;
            if (_unscaledTime > 1)
            {
                _energyModel.Ticked((int)_unscaledTime);
                _unscaledTime = 0;
            }
        }
        
        private void EarnedEnergyReward()
        {
            _energyModel.AddEnergy(_energyModel.MaxEnergy);
            _energyUI.DisableAd();
            TimeLeftUpdater();
            _adIsActivated = false;
        }

        private void EnergyValueUpdater() 
        {
            _energyUI.Energy(_energyModel.Energy,_energyModel.MaxEnergy);
            Player.Energy = _energyModel.Energy;
        }

        private short counter = 0;
        private void TimeLeftUpdater(int timeLeft=-1)
        {
            if (!_energyModel.IsFull)
            {
                _energyUI.Time(timeLeft);
                counter++;
                if(counter == 5)
                {
                    Player.TimeLeftToAddEnergy = timeLeft;
                    counter = 0;
                }
            }
            else
            {
                _energyUI.Time(-1);
                Player.TimeLeftToAddEnergy = Constants.TimeToAddEnergy-1;
            }
        }

        public void Recycle()
        {
            _energyModel._energyChanged -= EnergyValueUpdater;
            _energyModel._timeLeftChanged -= TimeLeftUpdater;
        }

    }
}
