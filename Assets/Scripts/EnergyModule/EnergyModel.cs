using System;
using UnityEngine;

namespace Assets.Scripts
{
    internal class EnergyModel
    {
        private int _currentEnergy, _maxEnergy;

        private readonly int _timeToAddEnergy;
        
        private State _state;
        private enum State
        {
            Adding,
            Full
        }

        public int _currentEnergyTimeLeft { get; private set; } 

        public int Energy => _currentEnergy;
        public int MaxEnergy => _maxEnergy;

        public event Action _energyChanged;

        public event Action<int> _timeLeftChanged;

        public bool IsFull => _state == State.Full;

        public EnergyModel(int energy, int timeToAddEnergy, int maxEnergy, int timeLeftToAddEnergy) 
        {
            _state = State.Adding;
            _maxEnergy = maxEnergy;
            if (energy >= _maxEnergy)
            {
                energy = _maxEnergy;
                _state = State.Full;
            }
            if (energy < 0) energy = 2;
            if (timeToAddEnergy <= 0)
            {
                Debug.LogError("Time to add energy <= 0" + timeToAddEnergy);
                timeToAddEnergy = 150;
            }
            _currentEnergy = energy;
            _timeToAddEnergy = timeToAddEnergy;
            while (timeLeftToAddEnergy > _timeToAddEnergy)
            {
                timeLeftToAddEnergy -= _timeToAddEnergy;
                AddEnergy(1);
            }
            _currentEnergyTimeLeft = timeLeftToAddEnergy;
        }

        public void Ticked(int sec = 1)
        {
            if (_state == State.Full)
            {
                _timeLeftChanged?.Invoke(-1);
                return;
            }
            _currentEnergyTimeLeft -= sec; 
            _timeLeftChanged?.Invoke(_currentEnergyTimeLeft);
            if (_currentEnergyTimeLeft <= 0)
            {
                AddEnergy(1);
                _currentEnergyTimeLeft = _timeToAddEnergy;
            }
           
        }

        public void AddEnergy(int amount)
        {
            if (amount <= 0 || _state==State.Full) return;
            _currentEnergy += amount;
            if (_currentEnergy >= _maxEnergy)
            {
                _currentEnergy = _maxEnergy;
                _state = State.Full;
            }
             _energyChanged?.Invoke();
        }
        public bool TryUseEnergy(int amount) 
        {
            if (amount <= 0 || _currentEnergy < amount) return false;
            _currentEnergy -= amount;
            if (_state == State.Full) _state = State.Adding;
            _energyChanged?.Invoke();
     
            return true;
        }

    }
}