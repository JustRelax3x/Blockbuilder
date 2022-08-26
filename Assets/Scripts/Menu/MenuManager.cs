using UnityEngine;
using Assets.Scripts;
using Assets.Scripts.Entities;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private EnergySystemManager _energyManager;
    [SerializeField]
    private LevelSelector _levelSelector;
    [SerializeField]
    private MenuScreenPresenter _screenPresenter;

    private SaveSystem _save = new SaveSystem();

    private void Start()
    {
        _energyManager.Initialize(Player.Energy, Constants.TimeToAddEnergy, Constants.MaxEnergy, Player.TimeLeftToAddEnergy);
        _levelSelector.Initialize(_energyManager,_screenPresenter);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            _energyManager.Recycle();
            _save.SaveData();
        }
        else
        {
            _save.LoadLastSave();
            _energyManager.Initialize(Player.Energy, Constants.TimeToAddEnergy, Constants.MaxEnergy, Player.TimeLeftToAddEnergy);
        }
    }
    private void OnApplicationQuit()
    {
        _save.SaveData();
    }
}
