using System;
using UnityEngine;
using UnityEngine.UI;

public class ImageAnimator : MonoBehaviour
{
    public event EventHandler OnAnimationLoopedFirstTime;

    public event EventHandler OnAnimationLooped;

    [SerializeField] private Sprite[] frameArray;
    [SerializeField] private float framerate = .1f;
    [SerializeField] private bool loop;

    private int _currentFrame;
    private float _timer;
    private Image _image;
    private bool _isPlaying;
    private int _loopCounter;

    private void Awake()
    {
        _image = gameObject.GetComponent<Image>();

        if (frameArray != null)
        {
            PlayAnimation(frameArray, framerate);
        }
        else
        {
            _isPlaying = false;
        }
    }

    private void Update()
    {
        if (!_isPlaying)
        {
            return;
        }

        _timer += Time.deltaTime;

        if (_timer >= framerate)
        {
            _timer -= framerate;
            _currentFrame = (_currentFrame + 1) % frameArray.Length;
            if (!loop && _currentFrame == 0)
            {
                StopPlaying();
            }
            else
            {
                _image.sprite = frameArray[_currentFrame];
            }

            if (_currentFrame == 0)
            {
                _loopCounter++;
                if (_loopCounter == 1)
                {
                    if (OnAnimationLoopedFirstTime != null) OnAnimationLoopedFirstTime(this, EventArgs.Empty);
                }
                if (OnAnimationLooped != null) OnAnimationLooped(this, EventArgs.Empty);
            }
        }
    }

    public void StopPlaying()
    {
        _isPlaying = false;
    }

    public void PlayAnimation(Sprite[] frameArray, float framerate, bool loop = true)
    {
        this.frameArray = frameArray;
        this.framerate = framerate;
        _isPlaying = true;
        _currentFrame = 0;
        _timer = 0f;
        _loopCounter = 0;
        _image.sprite = frameArray[_currentFrame];
    }
}