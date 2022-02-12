using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class LevelSelectorButton : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI _numberOfLevel;
    [SerializeField]
    private GameObject[] Stars;
    [SerializeField]
    private SpriteAtlas _atlas;
    public void EnableButton(int numberOfLevel,int stars)
    {
        GetComponent<Image>().sprite = _atlas.GetSprite("Button");
        _numberOfLevel.gameObject.SetActive(true);
        _numberOfLevel.text = numberOfLevel.ToString();
        for (int i=0; i<stars; i++)
        {
            Stars[i].SetActive(true);
        }
        gameObject.GetComponent<Button>().interactable = true;
    }
}
