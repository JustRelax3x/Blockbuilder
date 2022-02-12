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
    private Vector2Int _sizeXY;
    [SerializeField]
    private ParticleSystem _smoke;
    [SerializeField]
    private ParticleSystem _star;

    private bool[] _blocks;
    private bool[] _activeGoals;
    private bool[] _arrows;
    private BlockColor[] _colors;
    private List<BlockColor> _blockToBeDropped;

    private short _goalCounter;
    private short _goalCompleted = 0;

    [SerializeField]
    private short _maxHp = 10;
    private short _hp;
    private float _upperSlot;

    private Animator _craneAnimator;
    private bool _dropped;

    private Vector3 _startcranePos;
    private short _moved;   

    private List<GameObject> _spawnedObjects = new List<GameObject>();
    private List<GameObject> _usedObjects = new List<GameObject>();

    public int LevelLength => _levels.Length;

    public event Action<int,bool> GameOver;


    public void LevelReader(int level)
    {
        _blocks = new bool[_sizeXY.x * _sizeXY.y];
        _activeGoals = new bool[_sizeXY.x * _sizeXY.y];
        _arrows = new bool[_sizeXY.x * _sizeXY.y];
        _colors = new BlockColor[_sizeXY.x * _sizeXY.y];
        _blockToBeDropped = new List<BlockColor>();
        if (level < 0)
        {
            GenerateLevel();
            return;
        }
        string s;
        try
        {
            s = _levels[level].text;
        }
        catch
        {
            s = _levels[0].text;
        }
        string text = s.Trim().Replace(" ", "").Replace("\n", "");
        int temp = text.Length - _sizeXY.x;

        for (int i = 0; i < _sizeXY.x * _sizeXY.y; i++, temp++)
        {
            if (text[temp] == 'x')
            {
                if (((i + 1) % _sizeXY.x) == 0)
                {
                    temp = temp - _sizeXY.x - _sizeXY.x - 1;
                }
                continue;
            }
            switch (text[temp])
            {
                case 'b':
                    _blocks[i] = true;
                    _colors[i] = BlockColor.Black;
                    break;
                case 's':
                    _blocks[i] = true;
                    _colors[i] = BlockColor.Blue;
                    break;
                case 'r':
                    _blocks[i] = true;
                    _colors[i] = BlockColor.Red;
                    break;
                case 'y':
                    _blocks[i] = true;
                    _colors[i] = BlockColor.Yellow;
                    break;
                case 'p':
                    _blocks[i] = true;
                    _colors[i] = BlockColor.Red;
                    break;
                case 'B':
                    _activeGoals[i] = true;
                    _colors[i] = BlockColor.Black;
                    break;
                case 'S':
                    _activeGoals[i] = true;
                    _colors[i] = BlockColor.Blue;
                    break;
                case 'R':
                    _activeGoals[i] = true;
                    _colors[i] = BlockColor.Red;
                    break;
                case 'Y':
                    _activeGoals[i] = true;
                    _colors[i] = BlockColor.Yellow;
                    break;
                case 'P':
                    _activeGoals[i] = true;
                    _colors[i] = BlockColor.Purple;
                    break;
                case '1':
                    _arrows[i] = true;
                    _colors[i] = BlockColor.Black;
                    break;
                case '2':
                    _arrows[i] = true;
                    _colors[i] = BlockColor.Blue;
                    break;
                case '3':
                    _arrows[i] = true;
                    _colors[i] = BlockColor.Purple;
                    break;
                case '4':
                    _arrows[i] = true;
                    _colors[i] = BlockColor.Red;
                    break;
                case '5':
                    _arrows[i] = true;
                    _colors[i] = BlockColor.Yellow;
                    break;

            }
            if (((i + 1) % _sizeXY.x) == 0)
            {
                temp = temp - _sizeXY.x - _sizeXY.x - 1;
            }
        }
    }

    private void GenerateLevel()
    {
        bool lineHasBlock;
        int random, goalsNumber = 1, blackArrowsMoveToRight = 0, arrowsLeftInLine, arrowsRightInLine;
        int[] rightExtremeSlotByColor = new int[(int)BlockColor.Sizeof];
        int[] leftExtremeSlotByColor = new int[(int)BlockColor.Sizeof];
        for (int i = 0; i < rightExtremeSlotByColor.Length; i++)
        {
            rightExtremeSlotByColor[i] = _sizeXY.x;
            leftExtremeSlotByColor[i] = _sizeXY.x;
        }
        for (int y = 0; y < _sizeXY.y; y++)
        {
            arrowsRightInLine = 0;
            arrowsLeftInLine = 0;
            if (y <= _sizeXY.y / 2)
            {
                lineHasBlock = false;
                for (int x = 0; x < _sizeXY.x; x++)
                {
                    random = UnityEngine.Random.Range(0, y + 4);
                    if (random > 1) //add last chance spawn
                    {
                        if (goalsNumber == 0) continue;
                        if (y > 0)
                        {
                            if (!_blocks[x + (y - 1) * _sizeXY.x] || !_activeGoals[x + (y - 1) * _sizeXY.x])
                            {
                                continue;
                            }
                        }
                        random = UnityEngine.Random.Range(0, goalsNumber);
                        _activeGoals[x + y * _sizeXY.x] = random <= 1; // chance  100 100 66 50 40 
                        goalsNumber = _activeGoals[x + y * _sizeXY.x] ? goalsNumber + 1 : 0;
                        if (goalsNumber == 0) continue;
                        random = UnityEngine.Random.Range(1, (int)BlockColor.Sizeof);
                        _colors[x + y * _sizeXY.x] = (BlockColor)random;
                        rightExtremeSlotByColor[random] = Mathf.Min(_sizeXY.x - 1 - x, rightExtremeSlotByColor[random]);
                        leftExtremeSlotByColor[random] = Mathf.Min(x, leftExtremeSlotByColor[random]);
                        continue;
                    }
                    if (y > 0)
                    {
                        if (!_blocks[x + (y - 1) * _sizeXY.x] || y == _sizeXY.y / 2)
                        {
                            continue;
                        }
                    }
                    _blocks[x + y * _sizeXY.x] = true;
                    random = UnityEngine.Random.Range(0, 5);
                    _colors[x + y * _sizeXY.x] = (BlockColor)random;
                    lineHasBlock = true;
                }
                if (!lineHasBlock && y < _sizeXY.y / 2) y = _sizeXY.y / 2;
            }
            else
            {
                random = UnityEngine.Random.Range(0, 16); //0 1 2 3 - 1arrow; 4 5 - 2 arrows; 6 7 - 3arrows; 8 - 1a 1a; 9 - 2a 1a; 10 - 1a 2a; 11 - 2a 2a, 12 - 3a 2a 13 - 3a 3a; 
                switch (random)
                {
                    case 7:
                        BuildArrowToTheRight(y);
                        goto case 5;
                    case 5:
                        BuildArrowToTheRight(y);
                        goto case 1;
                    case 3:
                    case 1:
                        BuildArrowToTheRight(y);
                        break;

                    case 6:
                        BuildArrowToTheLeft(y);
                        goto case 4;
                    case 4:
                        BuildArrowToTheLeft(y);
                        goto case 0;
                    case 2:
                    case 0:
                        BuildArrowToTheLeft(y);
                        break;

                    case 8:
                        BuildArrowToTheRight(y);
                        goto case 0;

                    case 9:
                        BuildArrowToTheRight(y);
                        goto case 4;

                    case 13:
                        BuildArrowToTheRight(y);
                        goto case 12;
                    case 12:
                        BuildArrowToTheLeft(y);
                        goto case 11;
                    case 11:
                        BuildArrowToTheLeft(y);
                        goto case 10;
                    case 10:
                        BuildArrowToTheLeft(y);
                        goto case 5;

                    default:
                        break;
                }
            }
        }
        void BuildArrowToTheRight(int line)
        {
            random = UnityEngine.Random.Range(0, (int)BlockColor.Sizeof);
            _colors[(line + 1) * _sizeXY.x - 1 - arrowsRightInLine] = (BlockColor)random;
            if (rightExtremeSlotByColor[random] + blackArrowsMoveToRight > 0 || random == 0)
            {
                if (random == 0)
                {
                    bool flag = false;
                    for (int i = 0; i < rightExtremeSlotByColor.Length; i++)
                    {
                        if (rightExtremeSlotByColor[i] + blackArrowsMoveToRight <= 0)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag) return;
                    blackArrowsMoveToRight--;
                }
                _arrows[(line + 1) * _sizeXY.x - 1 - arrowsRightInLine] = true;
                arrowsRightInLine++;
                leftExtremeSlotByColor[random]++;
                rightExtremeSlotByColor[random]--;
            }
        }
        void BuildArrowToTheLeft(int line)
        {
            random = UnityEngine.Random.Range(0, (int)BlockColor.Sizeof);
            _colors[line * _sizeXY.x + arrowsLeftInLine] = (BlockColor)random;
            if (leftExtremeSlotByColor[random] - blackArrowsMoveToRight > 0 || random == 0)
            {
                if (random == 0)
                {
                    bool flag = false;
                    for (int i = 0; i < leftExtremeSlotByColor.Length; i++)
                    {
                        if (leftExtremeSlotByColor[i] - blackArrowsMoveToRight <= 0)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag) { return; }
                    blackArrowsMoveToRight++;
                }
                _arrows[line * _sizeXY.x + arrowsLeftInLine] = true;
                arrowsLeftInLine++;
                rightExtremeSlotByColor[random]++;
                leftExtremeSlotByColor[random]--;
            }
        }
    }

    public void InitializeLevel()
    {
        InitLevel();
        _goalCounter = 0;
        _upperSlot = _upSlot.position.y - _slots[0].position.y;
        Vector3 slot;
        for (int y = 0; y < _sizeXY.y; y++)
        {
            for (int i = 0; i < _sizeXY.x; i++)
            {
                if (_blocks[i + y * _sizeXY.x])
                {
                    slot = _slots[i].position;
                    slot.y += y * _upperSlot;
                    Instantiate(_blockPrefab, slot, Quaternion.identity, _canvas).GetComponent<Block>().blockColor = _colors[i + y * _sizeXY.x];
                    if (y == _sizeXY.y - 1)
                    {
                        slot.y += _upperSlot;
                        Instantiate(_emptyPrefab, slot, Quaternion.identity, _canvas);
                    }
                    else
                    {
                        if (!_activeGoals[i + (y + 1) * _sizeXY.x] && !_blocks[i + (y + 1) * _sizeXY.x])
                        {
                            slot.y += _upperSlot;
                            Instantiate(_emptyPrefab, slot, Quaternion.identity, _canvas);
                        }
                    }
                }
                else if (_activeGoals[i + y * _sizeXY.x])
                {
                    if (y > 0)
                    {
                        if (_activeGoals[i + (y - 1) * _sizeXY.x] || !_blocks[i + (y - 1) * _sizeXY.x])
                        {
                            continue;
                        }
                    }
                    slot = _slots[i].position;
                    slot.y += y * _upperSlot;
                    GameObject ob = Instantiate(_goalPrefab, slot, Quaternion.identity, _canvas).gameObject;
                    ob.GetComponent<Block>().blockColor = _colors[i + y * _sizeXY.x];
                    ob.GetComponent<Goal>().Index(i + y * _sizeXY.x);
                    _blockToBeDropped.Add(_colors[i + y * _sizeXY.x]);
                    _goalCounter++;
                }
                else if (_arrows[i + y * _sizeXY.x])
                {
                    slot = _slots[i].position;
                    slot.y += y * _upperSlot;
                    if (i > _sizeXY.x / 2f)
                    {
                        int index = _sizeXY.x - i - 1;
                        slot.x += (_slots[1].position.x - _slots[0].position.x) / 2f * index;
                        slot.y += 0.1f; //left arrows always should be higher
                        Instantiate(_leftArrowPrefab, slot, Quaternion.identity, _canvas).GetComponent<Block>().blockColor = _colors[i + y * _sizeXY.x];
                    }
                    else
                    {
                        slot.x -= (_slots[1].position.x - _slots[0].position.x) / 2f * (i);
                        Instantiate(_rightArrowPrefab, slot, Quaternion.identity, _canvas).GetComponent<Block>().blockColor = _colors[i + y * _sizeXY.x];
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
        _craneSlot.GetComponent<Block>().blockColor =_blockToBeDropped[_goalCompleted];
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
        b.blockColor = _blockToBeDropped[_goalCompleted];
        b.ReachedGoal += BlockReachedGoal;
        b.ReachedEmpty += BlockReachedEmpty;
        b.Dash += BlockDash;
        _spawnedObjects.Add(b.gameObject);
    }

    private void InitLevel() {
        _goalCompleted = 0;
        _hp = _maxHp;
    }
    
    public void RestartLevel()
    {
        Recycle();
        InitLevel();       
        _crane.transform.localPosition = _startcranePos;
        _craneSlot.GetComponent<Image>().enabled = true;
        _craneSlot.GetComponent<Block>().blockColor = _blockToBeDropped[_goalCompleted];
        _craneAnimator.SetTrigger("Take");
        _dropped = false;
    }

    public void Recycle()
    {
        foreach (GameObject v in _spawnedObjects) {
            if (v.CompareTag("Block"))
            {
                v.GetComponent<Block>().ReachedGoal -= BlockReachedGoal;
                v.GetComponent<Block>().ReachedEmpty -= BlockReachedEmpty;
                v.GetComponent<Block>().Dash -= BlockDash;
            }
            Destroy(v);
        }
        _spawnedObjects.Clear();
        foreach (GameObject v in _usedObjects) v.SetActive(true);
        _usedObjects.Clear();
    }

    private void BlockReachedGoal(GameObject goal, BlockColor blockColor, BlockColor goalColor, int index)
    {
        _goalCompleted++;
        _usedObjects.Add(goal);
        goal.SetActive(false);
        if (blockColor != goalColor)
        {
            _hp--;
            if (GameOverCheck()) return;
            Vibration.VibratePeek();
        }
        index += _sizeXY.x;
        if (_activeGoals[index])
        {
            Vector3 position = goal.transform.position + new Vector3(0f, _upperSlot, 0f);
            GameObject ob = Instantiate(_goalPrefab, position, Quaternion.identity, _canvas);
            ob.GetComponent<Goal>().Index(index);
            _goalCounter++;
            ob.GetComponent<Block>().blockColor =_colors[index];
            _blockToBeDropped.Add(_colors[index]);
        }
        else
        {
            Vector3 position = goal.transform.position + new Vector3(0f, _upperSlot, 0f);
            GameObject ob = Instantiate(_emptyPrefab, position, Quaternion.identity, _canvas);
            _spawnedObjects.Add(ob);
        }
        if (_goalCompleted == _goalCounter)
        {
            GameOver?.Invoke(_maxHp - _hp, true);
            return;
        }
        _star.gameObject.transform.localPosition = goal.transform.localPosition;
        _star.startColor = goal.GetComponent<Image>().color;
        _star.Play();
        CraneRefill();
    }
    private void BlockReachedEmpty(GameObject empty)
    {
        _hp -= 2;
        if (GameOverCheck()) return;
        _usedObjects.Add(empty);
        empty.SetActive(false);
        _smoke.gameObject.transform.localPosition = empty.transform.localPosition;
        _smoke.Play();
        Vector3 position = empty.transform.position + new Vector3(0f, _upperSlot, 0f);
        GameObject ob = Instantiate(_emptyPrefab, position, Quaternion.identity,_canvas);
        _spawnedObjects.Add(ob);
        CraneRefill();
    }
    private bool GameOverCheck()
    {
        if (_hp <= 0)
        {
            GameOver?.Invoke(_maxHp - _hp, false);
            return true;
        }
        return false;
    }
    private void CraneRefill()
    {
        _crane.transform.localPosition = _startcranePos;
        _craneAnimator.SetTrigger("Take");
        _craneSlot.GetComponent<Image>().enabled = true;
        _craneSlot.GetComponent<Block>().blockColor = _blockToBeDropped[_goalCompleted];
        _dropped = false;
    }

    private void BlockDash(GameObject block, bool right)
    {
        float dash = _slots[1].position.x - _slots[0].position.x;
        if (right)
        {
            if (block.transform.position.x < _slots[_sizeXY.x - 1].position.x - dash / 2)
            {
                if (_moved == 0)
                {
                    block.transform.position += new Vector3(dash, 0f);
                }
                else
                {
                    _moved--;
                }
            }
        }
        else
        {
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



    
}
