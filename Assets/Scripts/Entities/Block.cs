using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    private BlockColor _blockColor;
    private Image _image;
    public event System.Action<GameObject, BlockColor, BlockColor, int> ReachedGoal;
    public event System.Action<GameObject> ReachedEmpty;
    public event System.Action<GameObject,bool> Dash;

    public BlockColor blockColor
    {
        get => _blockColor;
        set => ChangeColor(value);
    }

    private void ChangeColor(BlockColor blockColor)
    {
        _blockColor = blockColor;
        _image = GetComponent<Image>();
        Color color = new Color(0.2038324f, 0.1521894f, 0.5660378f);
        switch (blockColor)
        { 
            case BlockColor.Blue:
                break;
            case BlockColor.Red:
                color = new Color(0.745283f, 0.08654777f, 0.08085619f);
                break;
            case BlockColor.Yellow:
                color = new Color(0.9245283f, 0.860714f, 0.06541473f);
                break; 
            case BlockColor.Purple:
                color = new Color(0.3238445f, 0.8867924f, 0.2133321f);
                break;
            case BlockColor.Black:
                color = new Color(0f, 0f, 0f);
                break;
        }
        _image.color = color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CompareTag("Block"))
        {
            
            if (collision.CompareTag("Right") || collision.CompareTag("Left"))
            {
                BlockColor collisionColor = collision.GetComponent<Block>().blockColor;
                if (collisionColor == BlockColor.Black || blockColor == BlockColor.Black || collisionColor == blockColor )
                Dash?.Invoke(gameObject, collision.CompareTag("Right"));
            }
            else if (collision.CompareTag("Goal"))
            {
                BlockColor collisionColor = collision.GetComponent<Block>().blockColor;
                ReachedGoal?.Invoke(collision.gameObject, blockColor, collisionColor, collision.GetComponent<Goal>().GetIndex);
            }
            else if (collision.CompareTag("Empty"))
            {
                ReachedEmpty?.Invoke(collision.gameObject); 
                Vibration.VibratePeek();
            }
        }
    }
}


public enum BlockColor {
Black = 0,
Blue = 1,
Purple = 2,
Red = 3,
Yellow = 4,
Sizeof = 5
}
