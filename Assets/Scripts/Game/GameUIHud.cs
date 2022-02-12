using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using Assets.SimpleLocalization;

public class GameUIHud : MonoBehaviour
{
    [SerializeField]
    private SpriteAtlas _atlas;
    [SerializeField]
    private GameObject _handPressToDrop;
    [SerializeField]
    private TextMeshProUGUI _levelNumber;
    [SerializeField]
    private TextMeshProUGUI _levelCompleted;
    [SerializeField]
    private GameObject _mainCanvas;
    [SerializeField]
    private GameObject _ggPanel;
    [SerializeField]
    private GameObject _pausePanel;
    [SerializeField]
    private GameObject[] _stars;
    [SerializeField]
    private Image _vibration;
    [SerializeField]
    private Image _volume;
    [SerializeField]
    private GameObject[] _popUpScreens;
    [SerializeField]
    private Window_Confetti _confetti;


    private Coroutine _turnOffOnClick;
    private Coroutine _vibroSrars;
    public void ActivateUI(int level, bool tutorial, System.Action action) {
        _mainCanvas.SetActive(true);
        _levelNumber.text = (level+1).ToString();
        _ggPanel.SetActive(false);
        if (tutorial)
        {
            bool flag = false;
            switch (level)
            {
                case 0:
                    _handPressToDrop.SetActive(true);
                    _turnOffOnClick = StartCoroutine(TurnOffOnClick(_handPressToDrop));
                    break;
                case 1:
                    flag = true;
                    level = 0;
                    break;
                case 10:
                    flag = true;
                    level = 1;
                    break;
                case 20:
                    flag = true;
                    level = 2;
                    break;
                default:
                    break;
            }
            if (flag)
            {
                _popUpScreens[level].transform.parent.gameObject.SetActive(true);
                _popUpScreens[level].SetActive(true);
                _turnOffOnClick = StartCoroutine(TurnOffOnClick(_popUpScreens[level].transform.parent.gameObject));
            }
        }
        UpdateChangeableUI(Player.Vibration, Player.Volume);
        foreach (var stars in _stars) stars.SetActive(false);
        _confetti.gameObject.SetActive(false);
        _confetti.Clear();
        action?.Invoke();
    }

    public void Pause(bool paused)
    {
        _pausePanel.SetActive(paused);
    }

    public void OnVolumeClicked()
    {
        Player.Volume = !Player.Volume;
        ChangeVolumeSprite(Player.Volume);
    }

    public void OnVibroClicked()
    {
        Player.Vibration = !Player.Vibration;
        ChangeVibrationSprite(Player.Vibration);
    }

    private void UpdateChangeableUI(bool VibroIsActive, bool VolumeIsActive)
    {
        ChangeVibrationSprite(VibroIsActive);
        ChangeVolumeSprite(VolumeIsActive);
    }

    private void ChangeVibrationSprite(bool VibroIsActive) => _vibration.sprite = VibroIsActive ? _atlas.GetSprite("haptic") : _atlas.GetSprite("phone");
    private void ChangeVolumeSprite(bool VolumeIsActive) => _volume.sprite = VolumeIsActive ? _atlas.GetSprite("audioOn") : _atlas.GetSprite("audioOff");

    public void ActivateGGUI(int stars)
    {
        _ggPanel.SetActive(true);
        _levelCompleted.text = stars switch
        {
            0 => LocalizationManager.Localize("failed"),
            1 => LocalizationManager.Localize("1star"),
            2 => LocalizationManager.Localize("2stars"),
            3 => LocalizationManager.Localize("3stars"),
            _ => LocalizationManager.Localize("3stars"),
        };
        float clipLength = _ggPanel.GetComponent<Animation>().GetClip("GG").length;
        StartCoroutine(AnimationGG(clipLength, stars));
    }

    private IEnumerator VibroStars(int seconds)
    {
        while (seconds > 0)
        {
            long[] pattern = { 0, 25, 15, 5};
            Vibration.Vibrate(pattern, -1);
            seconds--;
            yield return new WaitForSeconds(0.75f);
        }
        if (_vibroSrars != null)
        {
            _confetti.gameObject.SetActive(true);
            StopCoroutine(_vibroSrars);
        }
    }
    private IEnumerator AnimationGG(float seconds, int stars)
    {
        yield return new WaitForSeconds(seconds);
        _mainCanvas.SetActive(false);
        for (int i = 0; i < stars; i++)
        {
            _stars[i].SetActive(true);
        }
        _vibroSrars = StartCoroutine(VibroStars(stars));
    }

    private IEnumerator TurnOffOnClick(GameObject gameObject)
    {
        yield return new WaitUntil(() => Input.touchCount > 0);
        gameObject.SetActive(false);
        StopCoroutine(_turnOffOnClick);
    }

}
