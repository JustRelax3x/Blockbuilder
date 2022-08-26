using Assets.Scripts.Entities;
using Assets.Scripts.Game.LevelModel;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelPresenter : MonoBehaviour
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

    [SerializeField]
    private GameObject _crane;

    [SerializeField]
    private GameObject _craneSlot;

    [SerializeField]
    private Transform _canvas;

    [SerializeField]
    private TextAsset[] _levels;

    [SerializeField]
    private Transform[] _slots;

    [SerializeField]
    private Transform _upSlot;

    [SerializeField]
    private ParticleHandler _particleHandler;

    private Vector2Int _sizeXY = new Vector2Int(Constants.SizeX, Constants.SizeY);
    private bool[] _blocks;
    private bool[] _activeGoals;
    private bool[] _arrows;
    private BlockColor[] _colors;
    private List<BlockColor> _blockToBeDropped;

    private short _goalCounter;
    private short _goalCompleted;

    private readonly short _maxHp = Constants.MaxHp;
    private short _hp;
    private float _upperSlot;

    private Animator _craneAnimator;
    private bool _dropped;

    private Vector3 _startcranePos;
    private short _moved;

    private LevelReader _levelReader = new LevelReader();
    private LevelGenerator _levelGenerator = new LevelGenerator();

    private List<GameObject> _spawnedObjects = new List<GameObject>();
    private List<GameObject> _usedObjects = new List<GameObject>();

    public int LevelLength => _levels.Length;

    public event Action<int, bool> GameOver;

    public void GetLevelData(int level)
    {
        _blocks = new bool[_sizeXY.x * _sizeXY.y];
        _activeGoals = new bool[_sizeXY.x * _sizeXY.y];
        _arrows = new bool[_sizeXY.x * _sizeXY.y];
        _colors = new BlockColor[_sizeXY.x * _sizeXY.y];
        _blockToBeDropped = new List<BlockColor>();
        if (level < 0)
        {
            _levelGenerator.Generate(_sizeXY, _blocks, _activeGoals, _colors, _arrows);
        }
        else
        {
            _levelReader.Read(_levels, level, _sizeXY, _blocks, _activeGoals, _colors, _arrows);
        }
    }

    public void BuildLevel()
    {
        BeginNewGame();
        _goalCounter = 0;
        _upperSlot = _upSlot.position.y - _slots[0].position.y;
        Vector3 slot;
        for (int y = 0; y < _sizeXY.y; y++)
        {
            for (int i = 0; i < _sizeXY.x; i++)
            {
                int slotIndex = i + y * _sizeXY.x;
                if (!_blocks[slotIndex] && !_activeGoals[slotIndex] && !_arrows[slotIndex])
                    continue;
                slot = _slots[i].position;
                slot.y += y * _upperSlot;
                if (_blocks[slotIndex])
                {
                    Instantiate(_blockPrefab, slot, Quaternion.identity, _canvas).GetComponent<Block>().BlockColor = _colors[slotIndex];
                    if (y != _sizeXY.y - 1 && (_activeGoals[i + (y + 1) * _sizeXY.x] || _blocks[i + (y + 1) * _sizeXY.x]))
                        continue;
                    slot.y += _upperSlot;
                    Instantiate(_emptyPrefab, slot, Quaternion.identity, _canvas);
                }
                else if (_activeGoals[slotIndex])
                {
                    if (y > 0 && (!_blocks[i + (y - 1) * _sizeXY.x]))
                    {
                        continue;
                    }
                    SpawnGoal(slot, slotIndex);
                }
                else if (_arrows[slotIndex])
                {
                    if (i > _sizeXY.x / 2f)
                    {
                        int index = _sizeXY.x - i - 1;
                        slot.x += (_slots[1].position.x - _slots[0].position.x) / 2f * index;
                        slot.y += 0.1f; //left arrows always should be higher
                        Instantiate(_leftArrowPrefab, slot, Quaternion.identity, _canvas).GetComponent<Block>().BlockColor = _colors[slotIndex];
                    }
                    else
                    {
                        slot.x -= (_slots[1].position.x - _slots[0].position.x) / 2f * (i);
                        Instantiate(_rightArrowPrefab, slot, Quaternion.identity, _canvas).GetComponent<Block>().BlockColor = _colors[slotIndex];
                    }
                }
            }
        }
        for (int i = 0; i < _sizeXY.x; i++)
        {
            if (_blocks[i] || _activeGoals[i]) continue;
            slot = _slots[i].position;
            Instantiate(_emptyPrefab, slot, Quaternion.identity, _canvas);
        }
        _craneSlot.GetComponent<Block>().BlockColor = _blockToBeDropped[_goalCompleted];
        _craneAnimator = _crane.GetComponent<Animator>();
        _startcranePos = _crane.transform.localPosition;
        _craneAnimator.SetTrigger("Take");
        _dropped = false;
    }

    public void Drop()
    {
        if (_dropped) return;
        _moved = 0;
        float slot = _craneSlot.transform.position.x;
        float minDistance = Mathf.Abs(slot - _slots[0].position.x);
        int index = 0;
        for (int i = 1; i < _sizeXY.x; i++)
        {
            if (minDistance > Mathf.Abs(slot - _slots[i].position.x))
            {
                minDistance = Mathf.Abs(slot - _slots[i].position.x);
                index = i;
            }
        }
        _dropped = true;
        _crane.transform.position = new Vector3(_slots[index].position.x, _crane.transform.position.y);
        _craneSlot.transform.position = new Vector3(_slots[index].position.x, _craneSlot.transform.position.y);
        _craneSlot.GetComponent<Image>().enabled = false;
        _craneAnimator.SetTrigger("Drop");
        Block b = Instantiate(_blockPrefab, _craneSlot.transform.position, Quaternion.identity, _canvas).GetComponent<Block>();
        b.BlockColor = _blockToBeDropped[_goalCompleted];
        b.ReachedGoal += BlockReachedGoal;
        b.ReachedEmpty += BlockReachedEmpty;
        b.Dash += BlockDash;
        _spawnedObjects.Add(b.gameObject);
    }

    public void RestartLevel()
    {
        Recycle();
        BeginNewGame();
        CraneRefill();
    }

    private void BeginNewGame()
    {
        _goalCompleted = 0;
        _hp = _maxHp;
    }

    public void Recycle()
    {
        foreach (GameObject v in _spawnedObjects)
        {
            Destroy(v);
        }
        _spawnedObjects.Clear();
        foreach (GameObject v in _usedObjects) v.SetActive(true);
        _usedObjects.Clear();
    }

    private void BlockReachedGoal(Goal goal, Block block)
    {
        _goalCompleted++;
        _usedObjects.Add(goal.gameObject);
        goal.gameObject.SetActive(false);
        if (block.BlockColor != goal.GetComponent<Block>().BlockColor)
        {
            _hp--;
            if (GameOverCheck()) return;
            Vibration.VibratePeek();
        }
        Unsubscribe(block);
        if (GameOverCheck()) return;
        int index = goal.GetIndex + _sizeXY.x;
        if (_activeGoals[index])
        {
            SpawnGoal(goal.gameObject.transform.position + new Vector3(0f, _upperSlot, 0f), index, true);
        }
        else
        {
            SpawnEmpty(goal.gameObject.transform.position + new Vector3(0f, _upperSlot, 0f));
        }
        _particleHandler.SetSuccessParticle(goal.transform.localPosition, goal.GetComponent<Image>().color);
        CraneRefill();
    }

    private void BlockReachedEmpty(GameObject empty, Block block)
    {
        _usedObjects.Add(empty);
        empty.SetActive(false);
        Unsubscribe(block);
        Vibration.VibratePeek();
        _hp -= 2;
        if (GameOverCheck()) return;
        _particleHandler.SetFailedParticle(empty.transform.localPosition);
        SpawnEmpty(empty.transform.position + new Vector3(0f, _upperSlot, 0f));
        CraneRefill();
    }

    private void SpawnGoal(Vector3 position, int index, bool lateSpawn = false)
    {
        GameObject ob = Instantiate(_goalPrefab, position, Quaternion.identity, _canvas);
        ob.GetComponent<Goal>().Index(index);
        ob.GetComponent<Block>().BlockColor = _colors[index];
        _blockToBeDropped.Add(_colors[index]);
        _goalCounter++;
        if (lateSpawn) _spawnedObjects.Add(ob);
    }

    private void SpawnEmpty(Vector3 position)
    {
        GameObject ob = Instantiate(_emptyPrefab, position, Quaternion.identity, _canvas);
        _spawnedObjects.Add(ob);
    }

    private bool GameOverCheck()
    {
        if (_hp > 0 && _goalCompleted != _goalCounter) return false;
        GameOver?.Invoke(_maxHp - _hp, _goalCompleted == _goalCounter);
        return true;
    }

    private void CraneRefill()
    {
        _crane.transform.localPosition = _startcranePos;
        _craneAnimator.SetTrigger("Take");
        _craneSlot.GetComponent<Image>().enabled = true;
        _craneSlot.GetComponent<Block>().BlockColor = _blockToBeDropped[_goalCompleted];
        _dropped = false;
    }

    private void Unsubscribe(Block block)
    {
        block.ReachedGoal -= BlockReachedGoal;
        block.ReachedEmpty -= BlockReachedEmpty;
        block.Dash -= BlockDash;
    }

    private void BlockDash(GameObject block, bool right)
    {
        float dash = _slots[1].position.x - _slots[0].position.x;
        if (right)
        {
            if (block.transform.position.x >= _slots[_sizeXY.x - 1].position.x - dash / 2) return;
            if (_moved == 0)
            {
                block.transform.position += new Vector3(dash, 0f);
            }
            else
            {
                _moved--;
            }
            return;
        }
        if (block.transform.position.x > _slots[0].position.x + dash / 2)
        {
            block.transform.position -= new Vector3(dash, 0f);
        }
        else
        {
            _moved++;
        }
    }
}