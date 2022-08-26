using GoogleMobileAds.Api;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoader : MonoBehaviour
{
    private SaveSystem _save = new SaveSystem();

    private void Start()
    {
        MobileAds.Initialize(initStatus => { });
        Assets.SimpleLocalization.LocalizationManager.Read();
        StartCoroutine(LoadAsync());
    }

    private IEnumerator LoadAsync()
    {
        string scene = "Game";
        if (_save.LoadLastSave()) scene = "Menu";
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        asyncLoad.allowSceneActivation = false;
        while (!asyncLoad.isDone)
        {
            //_bar.value = asyncLoad.progress;
            if (!asyncLoad.allowSceneActivation && Time.timeSinceLevelLoad > 1f)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}