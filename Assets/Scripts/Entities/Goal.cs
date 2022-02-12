using UnityEngine;

public class Goal : MonoBehaviour
{
    private int _index;
    public void Index(int index)
    {
        _index = index;
    }

    public int GetIndex => _index;
}
