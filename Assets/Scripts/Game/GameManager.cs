using Assets.Scripts;
using Assets.Scripts.Entities;
using Assets.Scripts.Game.LevelModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IInstantiator
{
    [SerializeField]
    private GameUIHud _gameUIHud;

    [SerializeField]
    private EnergySystemManager _energyManager;

    [SerializeField]
    private GameObject _crane;

    [SerializeField]
    private GameObject _craneSlot;

    [SerializeField]
    private TextAsset[] _levels;

    [SerializeField]
    private Transform[] _slots;

    [SerializeField]
    private Transform _upSlot;

    [SerializeField]
    private EntityFactory _entityFactory;

    [SerializeField]
    private ParticleSystem _smoke;

    [SerializeField]
    private ParticleSystem _star;

    private ParticleHandler _particleHandler;

    private LevelPresenter _levelPresenter;

    private SaveSystem _save = new SaveSystem();

    private int _playerLevel;

    public static GameManager Instance;
    private bool _pause;
    private bool _infinityMode = false;

    private System.Action _uiReady;

    private void Awake()
    {
        _playerLevel = Player.Level;
        _particleHandler = new ParticleHandler(_smoke, _star);
        _levelPresenter = new LevelPresenter(_entityFactory, this, _crane, _craneSlot, _levels, _slots, _upSlot, _particleHandler);
        _levelPresenter.GetLevelData(_playerLevel);
        if (_playerLevel < 0)
        {
            _playerLevel = Player.InfinityLevel;
            _infinityMode = true;
        }
        _gameUIHud.SetPlayerInputListener(_levelPresenter.Drop);
    }

    private void Start()
    {
        _pause = false;
        Vibration.Init();
        _gameUIHud.ActivateUI(_playerLevel, !_infinityMode, null);
        _levelPresenter.BuildLevel();
        _uiReady += StartCurrentLevel;
        _levelPresenter.GameOver += GameOver;
        _energyManager.Initialize(Player.Energy, Constants.TimeToAddEnergy, Constants.MaxEnergy, Player.TimeLeftToAddEnergy);
    }

    private void GameOver(int fails, bool completed)
    {
        short stars = 0;
        _energyManager.ActivateUI();
        if (!completed)
        {
            _gameUIHud.ActivateGGUI(stars);
            return;
        }
        switch (fails)
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
        if (!_infinityMode && stars > Player.StarsInLevel[_playerLevel])
        {
            Player.StarsInLevel[_playerLevel] = stars;
        }
        if (_playerLevel + 1 > Player.MaxLevel && !_infinityMode)
        {
            Player.MaxLevel = _playerLevel + 1;
        }
        else if (_playerLevel + 1 > Player.InfinityLevel && _infinityMode)
        {
            Player.InfinityLevel = _playerLevel + 1;
        }
    }

    public void RestartCurrentLevel()
    {
        if (_energyManager.TryUseEnergy())
        {
            if (_pause) OnPause();
            _gameUIHud.ActivateUI(_playerLevel, !_infinityMode, _uiReady);
        }
        _energyManager.ActivateUI();
    }

    private void StartCurrentLevel()
    {
        _levelPresenter.RestartLevel();
    }

    public void StartNextLevel()
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
            ActivateScene(Constants.GameScene);
        }
        else _energyManager.ActivateUI();
    }

    public void OnPause()
    {
        _pause = !_pause;
        _gameUIHud.Pause(_pause);
        Time.timeScale = _pause ? 0f : 1f;
    }

    public void Menu()
    {
        ActivateScene(Constants.MenuScene);
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

    private void ActivateScene(int scene)
    {
        Time.timeScale = 1f;
        Recycle();
        SceneManager.LoadScene(scene);
    }

    private void Recycle()
    {
        _uiReady -= StartCurrentLevel;
        _levelPresenter.GameOver -= GameOver;
        _energyManager.Recycle();
        _levelPresenter.Recycle();
        _save.SaveData();
    }

    public GameObject Instantiate(GameObject gameObject, Vector3 position, Quaternion quaternion)
    {
        return Instantiate(gameObject, position, quaternion, _gameUIHud.GetDynamicCanvas());
    }

    public void DestroyObject(GameObject gameObject)
    {
        Destroy(gameObject);
    }
}