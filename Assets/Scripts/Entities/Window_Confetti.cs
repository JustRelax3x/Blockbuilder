using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Window_Confetti : MonoBehaviour
{
    [SerializeField] private Transform pfConfetti;
    [SerializeField] private Color[] colorArray;

    private LinkedList<Confetti> _confettiList = new LinkedList<Confetti>();
    private float spawnTimer;
    private const float SPAWN_TIMER_MAX = 0.4f;

    private void Update()
    {
        CheckConfetti();
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            spawnTimer += SPAWN_TIMER_MAX;
            SpawnConfetti();
        }
    }

    private void SpawnConfetti()
    {
        float width = transform.GetComponent<RectTransform>().rect.width;
        float height = transform.GetComponent<RectTransform>().rect.height;
        Vector2 anchoredPosition = new Vector2(Random.Range(-width / 2f, width / 2f), height / 2f);
        Color color = colorArray[Random.Range(0, colorArray.Length)];
        Confetti confetti = new Confetti(pfConfetti, transform, anchoredPosition, color, -height / 2f);
        _confettiList.AddLast(confetti);
    }

    public void CheckConfetti()
    {
        int count =_confettiList.Count;
        if (count == 0) return;
        var temp = _confettiList.First;
        var tempNext = _confettiList.First;
        for (int i=0; i < count;i++)
        { 
            if (i != count - 1) 
                tempNext = temp.Next;
            if(temp.Value.Update())
            {
                temp.Value.Die();
                _confettiList.Remove(temp);
            }
            temp = tempNext;
        }
    }

    public void Clear()
    {
        int count = _confettiList.Count;
        var temp = _confettiList.First;
        for (int i = 0; i < count; i++)
        {
            temp.Value.Die();
            temp = temp.Next;
            _confettiList.RemoveFirst();
        }
    }

    private class Confetti
    {
        private Transform transform;
        private RectTransform rectTransform;
        private Vector2 anchoredPosition;
        private Vector3 euler;
        private float eulerSpeed;
        private Vector2 moveAmount;
        private float minimumY;

        public void Die()
        { Destroy(transform.gameObject); }

        public Confetti(Transform prefab, Transform container, Vector2 anchoredPosition, Color color, float minimumY)
        {
            this.anchoredPosition = anchoredPosition;
            this.minimumY = minimumY;
            transform = Instantiate(prefab, container);
            rectTransform = transform.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = anchoredPosition;

            rectTransform.sizeDelta *= Random.Range(.8f, 1.2f);

            euler = new Vector3(0, 0, Random.Range(0, 360f));
            eulerSpeed = Random.Range(100f, 200f);
            eulerSpeed *= Random.Range(0, 2) == 0 ? 1f : -1f;
            transform.localEulerAngles = euler;

            moveAmount = new Vector2(0, Random.Range(-450f, -650f));

            transform.GetComponent<Image>().color = color;
        }

        public bool Update()
        {
            anchoredPosition += moveAmount * Time.deltaTime;
            rectTransform.anchoredPosition = anchoredPosition;

            euler.z += eulerSpeed * Time.deltaTime;
            transform.localEulerAngles = euler;

            if (anchoredPosition.y < minimumY)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}