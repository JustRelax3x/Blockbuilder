using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(Image))]
public class SceneFader : MonoBehaviour {

	private Image _image;
	public AnimationCurve curve;

	void Start ()
	{
		_image = GetComponent<Image>();
		StartCoroutine(FadeIn());
	}

	public void FadeTo (int scene)
	{
		StartCoroutine(FadeOut(scene));
	}

	public void Fade()
	{
		StartCoroutine(FakeFade());
	}

	IEnumerator FadeIn ()
	{
		float t = 1f;

		while (t > 0f)
		{
			t -= Time.deltaTime;
			float a = curve.Evaluate(t);
			_image.color = new Color (0f, 0f, 0f, a);
			yield return 0;
		}
		StopAllCoroutines();
	}

	IEnumerator FakeFade()
    {
		float t = 0f;
		while (t < 1f)
		{
			t += Time.deltaTime;
			float a = curve.Evaluate(t);
			_image.color = new Color(0f, 0f, 0f, a);
			yield return 0;
		}
		StartCoroutine(FadeIn());
	}

	IEnumerator FadeOut(int scene)
	{
		float t = 0f;
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);
		asyncLoad.allowSceneActivation = false;
		while (t < 1f)
		{
			t += Time.deltaTime;
			float a = curve.Evaluate(t);
			_image.color = new Color(0f, 0f, 0f, a);
			yield return 0;
		}
		asyncLoad.allowSceneActivation = true;
	}

}
