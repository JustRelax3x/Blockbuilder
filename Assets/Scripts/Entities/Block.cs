using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    private BlockColor _blockColor;
    private Image _image;
    public event System.Action<Goal, Block> ReachedGoal;
    public event System.Action<GameObject,Block> ReachedEmpty;
    public event System.Action<GameObject,bool> Dash;

    public BlockColor BlockColor
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
                BlockColor collisionColor = collision.GetComponent<Block>().BlockColor;
                if (collisionColor == BlockColor.Black || BlockColor == BlockColor.Black || collisionColor == BlockColor )
                Dash?.Invoke(gameObject, collision.CompareTag("Right"));
            }
            else if (collision.CompareTag("Goal"))
            {
                ReachedGoal?.Invoke(collision.GetComponent<Goal>(), this);
            }
            else if (collision.CompareTag("Empty"))
            {
                ReachedEmpty?.Invoke(collision.gameObject, this); 
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
