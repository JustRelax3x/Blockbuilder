using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/EntityFactory")]
internal class EntityFactory : ScriptableObject
{
    [SerializeField]
    private GameObject _blockPrefab;

    [SerializeField]
    private GameObject _goalPrefab;

    [SerializeField]
    private GameObject _rightArrowPrefab;

    [SerializeField]
    private GameObject _leftArrowPrefab;

    [SerializeField]
    private GameObject _emptyPrefab;

    public GameObject GetEntity(Entity entity)
    {
        switch (entity)
        {
            case Entity.Block:
                return _blockPrefab;
            case Entity.Goal:
                return _goalPrefab;
            case Entity.Empty:
                return _emptyPrefab;
            case Entity.RightArrow:
                return _rightArrowPrefab;
            case Entity.LeftArrow:
                return _leftArrowPrefab;
            default:
                Debug.LogError("Factory Doesn't have prebaf for" + entity);
                return _blockPrefab;
        }
    }
    public enum Entity
    {
        Block,
        Goal,
        Empty,
        RightArrow,
        LeftArrow
    }
}

