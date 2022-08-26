using UnityEngine;

namespace Assets.Scripts.Game.LevelModel
{
    public class LevelGenerator
    {
        private int[] _rightExtremeSlotByColor = new int[(int)BlockColor.Sizeof];
        private int[] _leftExtremeSlotByColor = new int[(int)BlockColor.Sizeof];
        private bool _hasBlackGoal;

        public void Generate(Vector2Int sizeXY, bool[] blocks, bool[] activeGoals, BlockColor[] colors, bool[] arrows)
        {
            bool continueGenerate;
            int random, goalsNumber = 1;

            random = _rightExtremeSlotByColor.Length;
            for (int i = 0; i < random; i++) //initialize
            {
                _rightExtremeSlotByColor[i] = 0;
                _leftExtremeSlotByColor[i] = sizeXY.x - 1;
            }
            for (int y = 0; y <= sizeXY.y / 2; y++)
            {
                continueGenerate = false;
                for (int x = 0; x < sizeXY.x; x++)
                {
                    random = Random.Range(0, 5);
                    if (random >= 3 || (y == 1 && goalsNumber == 1))
                    {
                        if (goalsNumber == 0) continue;
                        if (y > 0)
                        {
                            if (!blocks[x + (y - 1) * sizeXY.x] && !activeGoals[x + (y - 1) * sizeXY.x])
                            {
                                continue;
                            }
                        }
                        random = Random.Range(0, goalsNumber);
                        activeGoals[x + y * sizeXY.x] = random <= 2; // chance  100 100 100 75 60 50...
                        goalsNumber = activeGoals[x + y * sizeXY.x] ? goalsNumber + 1 : 0;
                        if (goalsNumber == 0) continue;
                        random = Random.Range(0, (int)BlockColor.Sizeof);
                        colors[x + y * sizeXY.x] = (BlockColor)random;
                        if (random == (int)BlockColor.Black) _hasBlackGoal = true;
                        _rightExtremeSlotByColor[random] = Mathf.Max(x, _rightExtremeSlotByColor[random]);
                        _leftExtremeSlotByColor[random] = Mathf.Min(x, _leftExtremeSlotByColor[random]);
                        continueGenerate = true;
                        continue;
                    }
                    if (y > 0)
                    {
                        if (!blocks[x + (y - 1) * sizeXY.x] || y == sizeXY.y / 2)
                        {
                            continue;
                        }
                    }
                    blocks[x + y * sizeXY.x] = true;
                    random = Random.Range(0, 5);
                    colors[x + y * sizeXY.x] = (BlockColor)random;
                    continueGenerate = true;
                }
                if (!continueGenerate) break;
            }
            GenerateArrows(sizeXY, colors, arrows);
        }

        public void GenerateArrows(Vector2Int sizeXY, BlockColor[] colors, bool[] arrows)
        {
            int random, totalArrowPowerRightDir = 0, arrowsLeftInLine, arrowsRightInLine;
            int[] arrowsPowerRigthDir = new int[(int)BlockColor.Sizeof];
            for (int y = sizeXY.y / 2 + 1; y < sizeXY.y; y++)
            {
                arrowsRightInLine = 0;
                arrowsLeftInLine = 0;
                random = Random.Range(6, 15); //0 1 - 1arrow; 2 3 4 5 - 2 arrows; 6 7 - 3arrows; 8 - 1a 1a; 9 - 2a 1a; 10 - 1a 2a; 11 - 2a 2a, 12 - 3a 2a 13 - 3a 3a;
                switch (random)
                {
                    case 7:
                        GenerateArrowToTheRight(y);
                        goto case 5;
                    case 5:
                    case 3:
                        GenerateArrowToTheRight(y);
                        goto case 1;
                    case 1:
                        GenerateArrowToTheRight(y);
                        break;

                    case 6:
                        GenerateArrowToTheLeft(y);
                        goto case 4;
                    case 4:
                    case 2:
                        GenerateArrowToTheLeft(y);
                        goto case 0;
                    case 0:
                        GenerateArrowToTheLeft(y);
                        break;

                    case 8:
                        GenerateArrowToTheRight(y);
                        goto case 0;

                    case 9:
                        GenerateArrowToTheRight(y);
                        goto case 4;

                    case 13:
                        GenerateArrowToTheRight(y);
                        goto case 12;
                    case 12:
                        GenerateArrowToTheLeft(y);
                        goto case 11;
                    case 11:
                        GenerateArrowToTheLeft(y);
                        goto case 10;
                    case 10:
                        GenerateArrowToTheLeft(y);
                        goto case 5;

                    default:
                        break;
                }
            }
            void GenerateArrowToTheRight(int line)
            {
                int black = (int)BlockColor.Black;
                if (_hasBlackGoal && _rightExtremeSlotByColor[black] - totalArrowPowerRightDir >= sizeXY.x - 1) return;

                random = Random.Range(0, (int)BlockColor.Sizeof);
                if (_rightExtremeSlotByColor[random] - arrowsPowerRigthDir[black] - arrowsPowerRigthDir[random] >= sizeXY.x - 1 && random != black) return;
                if (random == black)
                {
                    int length = _rightExtremeSlotByColor.Length;
                    for (int i = 0; i < length; i++)
                    {
                        if (i == black) continue;
                        if (_rightExtremeSlotByColor[i] - arrowsPowerRigthDir[i] - arrowsPowerRigthDir[random] >= sizeXY.x - 1)
                        {
                            return;
                        }
                    }
                }
                colors[((line + 1) * sizeXY.x) - 1 - arrowsRightInLine] = (BlockColor)random;
                arrows[((line + 1) * sizeXY.x) - 1 - arrowsRightInLine] = true;
                arrowsRightInLine++;
                arrowsPowerRigthDir[random]--;
                totalArrowPowerRightDir--;
            }
            void GenerateArrowToTheLeft(int line)
            {
                int black = (int)BlockColor.Black;
                if (_hasBlackGoal && _leftExtremeSlotByColor[black] - totalArrowPowerRightDir <= 0) return;
                random = Random.Range(0, (int)BlockColor.Sizeof);

                if (_leftExtremeSlotByColor[random] - arrowsPowerRigthDir[black] - arrowsPowerRigthDir[random] <= 0 && random != black) return;
                if (random == black)
                {
                    int length = _leftExtremeSlotByColor.Length;
                    for (int i = 0; i < length; i++)
                    {
                        if (i == black) continue;
                        if (_leftExtremeSlotByColor[i] - arrowsPowerRigthDir[black] - arrowsPowerRigthDir[i] <= 0)
                        {
                            return;
                        }
                    }
                }
                arrows[line * sizeXY.x + arrowsLeftInLine] = true;
                colors[line * sizeXY.x + arrowsLeftInLine] = (BlockColor)random;
                arrowsLeftInLine++;
                arrowsPowerRigthDir[random]++;
                totalArrowPowerRightDir++;
            }
        }
    }
}