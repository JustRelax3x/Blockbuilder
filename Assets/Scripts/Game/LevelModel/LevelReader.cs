using UnityEngine;
public class LevelReader
{
    public void Read(TextAsset[] levels,int level,Vector2Int sizeXY,bool[] blocks,bool[] activeGoals, BlockColor[] colors, bool[] arrows)
    {
        string s;
        try
        {
            s = levels[level].text;
        }
        catch
        {
            s = levels[0].text;
        }
        string text = s.Trim().Replace(" ", "").Replace("\n", "");
        int temp = text.Length - sizeXY.x;

        for (int i = 0; i < sizeXY.x * sizeXY.y; i++, temp++)
        {
            if (text[temp] == 'x')
            {
                if (((i + 1) % sizeXY.x) == 0)
                {
                    temp = temp - sizeXY.x - sizeXY.x - 1;
                }
                continue;
            }
            switch (text[temp])
            {
                case 'b':
                    blocks[i] = true;
                    colors[i] = BlockColor.Black;
                    break;
                case 's':
                    blocks[i] = true;
                    colors[i] = BlockColor.Blue;
                    break;
                case 'r':
                    blocks[i] = true;
                    colors[i] = BlockColor.Red;
                    break;
                case 'y':
                    blocks[i] = true;
                    colors[i] = BlockColor.Yellow;
                    break;
                case 'p':
                    blocks[i] = true;
                    colors[i] = BlockColor.Red;
                    break;
                case 'B':
                    activeGoals[i] = true;
                    colors[i] = BlockColor.Black;
                    break;
                case 'S':
                    activeGoals[i] = true;
                    colors[i] = BlockColor.Blue;
                    break;
                case 'R':
                    activeGoals[i] = true;
                    colors[i] = BlockColor.Red;
                    break;
                case 'Y':
                    activeGoals[i] = true;
                    colors[i] = BlockColor.Yellow;
                    break;
                case 'P':
                    activeGoals[i] = true;
                    colors[i] = BlockColor.Green;
                    break;
                case '1':
                    arrows[i] = true;
                    colors[i] = BlockColor.Black;
                    break;
                case '2':
                    arrows[i] = true;
                    colors[i] = BlockColor.Blue;
                    break;
                case '3':
                    arrows[i] = true;
                    colors[i] = BlockColor.Green;
                    break;
                case '4':
                    arrows[i] = true;
                    colors[i] = BlockColor.Red;
                    break;
                case '5':
                    arrows[i] = true;
                    colors[i] = BlockColor.Yellow;
                    break;

            }
            if (((i + 1) % sizeXY.x) == 0)
            {
                temp = temp - sizeXY.x - sizeXY.x - 1;
            }
        }
    }
}

