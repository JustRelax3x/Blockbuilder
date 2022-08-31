using Assets.Scripts.Entities;
using Assets.Scripts.Game.LevelModel;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

internal class LevelPresenter
{
    private EntityFactory _entityFactory;

    private IInstantiator _instantiator;
        
    private TextAsset[] _levels;

    private CraneHandler _craneHandler;
        
    private ParticleHandler _particleHandler;

    private Vector2Int _sizeXY = new Vector2Int(Constants.SizeX, Constants.SizeY);
    private bool[] _blocks = new bool[Constants.SizeX * Constants.SizeY];
    private bool[] _activeGoals = new bool[Constants.SizeX * Constants.SizeY];
    private bool[] _arrows = new bool[Constants.SizeX * Constants.SizeY];
    private BlockColor[] _colors = new BlockColor[Constants.SizeX * Constants.SizeY];
    private List<BlockColor> _blockToBeDropped = new List<BlockColor>();

    private short _goalCounter, _goalCompleted, _hp;
    private float _upperSlot;
    private bool _dropped;

    private readonly short _maxHp = Constants.MaxHp;
    private readonly Transform[] _slots;
    private readonly Transform _upSlot;

    private LevelReader _levelReader = new LevelReader();
    private LevelGenerator _levelGenerator = new LevelGenerator();

    private List<GameObject> _spawnedObjects = new List<GameObject>();
    private List<GameObject> _usedObjects = new List<GameObject>();

    public int LevelLength => _levels.Length;

    public event Action<int, bool> GameOver;

    public LevelPresenter(EntityFactory entityFactory, IInstantiator instantiator, GameObject crane, GameObject craneSlot, TextAsset[] levels, Transform[] slots, Transform upSlot, ParticleHandler particleHandler)
    {
        _entityFactory = entityFactory;
        _instantiator = instantiator;
        _levels = levels;
        _slots = slots;
        _upSlot = upSlot;
        _craneHandler = new CraneHandler(crane,craneSlot);
        _particleHandler = particleHandler;
    }
    public void GetLevelData(int level)
    {
        if (level < 0) _levelGenerator.Generate(_sizeXY, _blocks, _activeGoals, _colors, _arrows);
        else _levelReader.Read(_levels, level, _sizeXY, _blocks, _activeGoals, _colors, _arrows);
    }

    public void BuildLevel()
    {
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
                    _instantiator.Instantiate(_entityFactory.GetEntity(EntityFactory.Entity.Block), slot, Quaternion.identity).GetComponent<Block>().BlockColor = _colors[slotIndex];
                    if (y != _sizeXY.y - 1 && (_activeGoals[i + (y + 1) * _sizeXY.x] || _blocks[i + (y + 1) * _sizeXY.x]))
                        continue;
                    slot.y += _upperSlot;
                    _instantiator.Instantiate(_entityFactory.GetEntity(EntityFactory.Entity.Empty), slot, Quaternion.identity);
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
                    float dx = _slots[1].position.x - _slots[0].position.x;
                    if (i > _sizeXY.x / 2f)
                    {
                        int index = _sizeXY.x - i - 1;
                        slot.x += dx / 2f * index;
                        slot.y += 0.1f; //left arrows always should be higher
                        _instantiator.Instantiate(_entityFactory.GetEntity(EntityFactory.Entity.LeftArrow), slot, Quaternion.identity).GetComponent<Block>().BlockColor = _colors[slotIndex];
                    }
                    else
                    {
                        slot.x -= dx / 2f * (i);
                        _instantiator.Instantiate(_entityFactory.GetEntity(EntityFactory.Entity.RightArrow), slot, Quaternion.identity).GetComponent<Block>().BlockColor = _colors[slotIndex];
                    }
                }
            }
        }
        for (int i = 0; i < _sizeXY.x; i++)
        {
            if (_blocks[i] || _activeGoals[i]) continue;
            slot = _slots[i].position;
            _instantiator.Instantiate(_entityFactory.GetEntity(EntityFactory.Entity.Empty), slot, Quaternion.identity);
        }
        BeginNewGame();
    }

    public void Drop()
    {
        if (_dropped) return;
        _dropped = true;
        _craneHandler.SetDropPosX(_slots[GetClosestXSlotIndex(_craneHandler.GetCraneSlotPos().x)].position.x);
        SpawnBlock(_craneHandler.GetCraneSlotPos());
    }

    public void BeginNewGame()
    {
        Recycle();
        _goalCompleted = 0;
        _hp = _maxHp;
        SetNextBlock();
    }

    public void Recycle()
    {
        foreach (GameObject v in _spawnedObjects)
        {
            _instantiator.DestroyObject(v);
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
        int index = goal.GetIndex + _sizeXY.x;
        if (_activeGoals[index])
        {
            SpawnGoal(goal.gameObject.transform.position + new Vector3(0f, _upperSlot, 0f), index, true);
        }
        else
        {
            SpawnEmpty(goal.gameObject.transform.position + new Vector3(0f, _upperSlot, 0f));
        }
        if (GameOverCheck()) return;
        _particleHandler.SetSuccessParticle(goal.transform.localPosition, goal.GetComponent<Image>().color);
        SetNextBlock();
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
        SetNextBlock();
    }

    private void SpawnGoal(Vector3 position, int index, bool lateSpawn = false)
    {
        GameObject ob = _instantiator.Instantiate(_entityFactory.GetEntity(EntityFactory.Entity.Goal), position, Quaternion.identity);
        ob.GetComponent<Goal>().Index(index);
        ob.GetComponent<Block>().BlockColor = _colors[index];
        _blockToBeDropped.Add(_colors[index]);
        _goalCounter++;
        if (lateSpawn) _spawnedObjects.Add(ob);
    }

    private void SpawnEmpty(Vector3 position)
    {
        GameObject ob = _instantiator.Instantiate(_entityFactory.GetEntity(EntityFactory.Entity.Empty), position, Quaternion.identity);
        _spawnedObjects.Add(ob);
    }

    private void SpawnBlock(Vector3 position)
    {
        Block b = _instantiator.Instantiate(_entityFactory.GetEntity(EntityFactory.Entity.Block), position, Quaternion.identity).GetComponent<Block>();
        b.BlockColor = _blockToBeDropped[_goalCompleted];
        b.moved = 0;
        b.ReachedGoal += BlockReachedGoal;
        b.ReachedEmpty += BlockReachedEmpty;
        b.Dash += BlockDash;
        _spawnedObjects.Add(b.gameObject);
    }

    private bool GameOverCheck()
    {
        if (_hp > 0 && _goalCompleted != _goalCounter) return false;
        GameOver?.Invoke(_maxHp - _hp, _goalCompleted == _goalCounter);
        return true;
    }

    private void SetNextBlock()
    {
        _craneHandler.SetStartPos(_blockToBeDropped[_goalCompleted]);
        _dropped = false;
    }

    private void Unsubscribe(Block block)
    {
        block.ReachedGoal -= BlockReachedGoal;
        block.ReachedEmpty -= BlockReachedEmpty;
        block.Dash -= BlockDash;
    }

    private void BlockDash(Block block, bool right)
    {
        float dash = _slots[1].position.x - _slots[0].position.x;
        if (right)
        {
            if (block.transform.position.x >= _slots[_sizeXY.x - 1].position.x - dash / 2) return;
            if (block.moved == 0) block.transform.position += new Vector3(dash, 0f);
            else block.moved--;
            return;
        }
        if (block.transform.position.x > _slots[0].position.x + dash / 2)
            block.transform.position -= new Vector3(dash, 0f);
        else block.moved++;
    }

    private int GetClosestXSlotIndex(float posX)
    {
        float minDistance = Mathf.Abs(posX - _slots[0].position.x);
        int index = 0;
        for (int i = 1; i < _sizeXY.x; i++)
        {
            if (minDistance > Mathf.Abs(posX - _slots[i].position.x))
            {
                minDistance = Mathf.Abs(posX - _slots[i].position.x);
                index = i;
            }
        }
        return index;
    }
}