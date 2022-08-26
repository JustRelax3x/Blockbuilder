using Assets.Scripts;
using Assets.Scripts.Entities;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    [SerializeField]
    private SceneFader _fader;

    [SerializeField]
    private Button[] _levelButtons;

    private MenuScreenPresenter _screenPresenter;

    private EnergySystemManager _energyManager;

    public void Initialize(EnergySystemManager energySystemManager, MenuScreenPresenter screenPresenter)
    {
        int levelReached = Player.MaxLevel;

        for (int i = 0; i <= levelReached; i++)
        {
            _levelButtons[i].GetComponent<LevelSelectorButton>().EnableButton(i + 1, Player.StarsInLevel[i]);
            AddListener(_levelButtons[i], i);
        }
        _energyManager = energySystemManager;
        _screenPresenter = screenPresenter;
    }

    private void AddListener(Button b, int value)
    {
        b.onClick.AddListener(() => Select(value));
    }

    public void Select(int level)
    {
        if (_energyManager.TryUseEnergy())
        {
            _energyManager.Recycle();
            Player.Level = level;
            _fader.FadeTo(Constants.GameScene);
        }
        else
        {
            _energyManager.ActivateUI();
            _screenPresenter.ChangeScreen(_screenPresenter.EnergyScreen);
        }
    }
}