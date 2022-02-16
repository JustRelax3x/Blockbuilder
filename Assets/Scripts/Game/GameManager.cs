using UnityEngine;
using Assets.Scripts;
using Assets.Scripts.Entities;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private SceneFader _fader;
    [SerializeField]
    private GameUIHud _gameUIHud;
    [SerializeField]
    private LevelPresenter _levelPresenter;
    [SerializeField]
    private EnergySystemManager _energyManager;

    private SaveSystem _save = new SaveSystem();

    private int _playerLevel;

    public static GameManager Instance;
    private bool _pause;
    private bool _infinityMode = false;

    private System.Action _uiReady; 


    private void Awake()
    {
        _playerLevel = Player.Level;
        _levelPresenter.GetLevelData(_playerLevel);
        if (_playerLevel < 0)
        {
            _playerLevel = Player.InfinityLevel;
            _infinityMode = true;
        }
    }
   
    private void Start()
    {
        _pause = false;
        Vibration.Init();
        _gameUIHud.ActivateUI(_playerLevel, !_infinityMode, null);
        _levelPresenter.BuildLevel();
        _uiReady += RestartLevel;
        _levelPresenter.GameOver += GameOver;
        _energyManager.Initialize(Player.Energy, Player.TIMEToAddEnergy, Player.MAXEnergy, Player.TimeLeftToAddEnergy);
    }


    private void GameOver(int score, bool completed)
    {
        short stars = 0;
        if (!completed)
        {
            _gameUIHud.ActivateGGUI(stars);
            return;
        }
        switch (score)
        {
            case 0:
                stars = 3;
                break;
            case 1:
            case 2:
            case 3:
                stars = 2;
                break;
            default:
                stars = 1;
                break;
        }
        _gameUIHud.ActivateGGUI(stars);
        _energyManager.ActivateUI();
        if (!_infinityMode && stars > Player.StarsInLevel[_playerLevel])
        {
            Player.StarsInLevel[_playerLevel] = stars;
        }
        if (_playerLevel+1 > Player.MaxLevel && !_infinityMode)
        {
            Player.MaxLevel = _playerLevel+1;
        }
        else if(_playerLevel+1 > Player.InfinityLevel && _infinityMode)
        {
            Player.InfinityLevel = _playerLevel+1;
        }
    }
    public void RestartPreviousLevel()
    {
        if (_energyManager.TryUseEnergy())
        {
            if (_pause) OnPause();
            _gameUIHud.ActivateUI(_playerLevel, !_infinityMode,_uiReady);
        }
    }

    private void RestartLevel()
    {
        _levelPresenter.RestartLevel();
    }
    public void StartGame()
    {  
        if (_energyManager.TryUseEnergy())
        {
            if (++_playerLevel >= _levelPresenter.LevelLength && !_infinityMode)
            {
                _playerLevel = 0;
            }
            if (!_infinityMode)
            {
                Player.Level = _playerLevel;
            }
            ActivateScene(Constants.SceneGame);
        }
    }
    public void OnPause()
    {
        _pause = !_pause;
        _gameUIHud.Pause(_pause);
        Time.timeScale = _pause ? 0f : 1f;
    }
    public void Menu()
    {
        ActivateScene(Constants.SceneMenu);
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
            _energyManager.Initialize(Player.Energy, Player.TIMEToAddEnergy, Player.MAXEnergy, Player.TimeLeftToAddEnergy);
        }
    }

    private void ActivateScene(int scene)
    {
        Time.timeScale = 1f;
        Recycle();
        _fader.FadeTo(scene);
    }

    private void Recycle()
    {
        _uiReady -= RestartLevel;
        _levelPresenter.GameOver -= GameOver;
        _energyManager.Recycle();
        _levelPresenter.Recycle();
    }

    private void OnApplicationQuit()
    {
        _save.SaveData();
    }

}
