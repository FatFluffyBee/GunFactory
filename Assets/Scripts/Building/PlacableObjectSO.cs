using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "ScriptableObjects/CreateNewBuildingType", order = 1)]

public class PlacableObjectSO : ScriptableObject
{
    public string buildingName;
    public Transform prefab;
    public Transform phantomVisual;
    public int width;
    public int height;

    public enum Dir { Down, Left, Up, Right } 

    public List<Vector2Int> GetGridPositionList(Vector2Int offset, Dir dir) //modify if building are not rectangulars
    {
        List<Vector2Int> gridPositions = new List<Vector2Int>();

        switch(dir)
        {
            case Dir.Down:
            case Dir.Up:
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < height; z++)
                    {
                        gridPositions.Add(new Vector2Int(x, z) + offset);
                    }
                }
                break;

            case Dir.Left:
            case Dir.Right:
                for (int x = 0; x < height; x++)
                {
                    for (int z = 0; z < width; z++)
                    {
                        gridPositions.Add(new Vector2Int(x, z) + offset);
                    }
                }
                break;
        }

       

        return gridPositions;
    }

    public static Dir GetNextDir(Dir dir, bool scrollUp) 
    {
        switch(dir)
        {
            default:
                case Dir.Down: return scrollUp ? Dir.Right : Dir.Left;
                case Dir.Left: return scrollUp ? Dir.Down : Dir.Up;
                case Dir.Up: return scrollUp ? Dir.Left : Dir.Right;
                case Dir.Right: return scrollUp ? Dir.Up : Dir.Down;

        }
    }

    public static Dir GetOppositeDir(Dir dir)
    {
        switch (dir)
        {
            default:
            case Dir.Down: return Dir.Up;
            case Dir.Left: return Dir.Right;
            case Dir.Up: return Dir.Down;
            case Dir.Right: return Dir.Left;

        }
    }

    public static int GetRotationAngle(Dir dir)
    {
        switch (dir)
        {
            default:
            case Dir.Down: return 0;
            case Dir.Left: return 90;
            case Dir.Up: return 180;
            case Dir.Right: return 270;
        }
    }

    public Vector2Int GetRotationOffset(Dir dir)
    {
        switch (dir) 
        {
            default:
            case Dir.Down: return new Vector2Int(0, 0);
            case Dir.Left: return new Vector2Int(0, width);
            case Dir.Up: return new Vector2Int(width, height);
            case Dir.Right: return new Vector2Int(height, 0);
        }
    }

    public static Vector2Int GetDirForwardVector(Dir dir)
    {
        switch (dir) 
        {
            default:
            case Dir.Down: return new Vector2Int(0, -1); 
            case Dir.Left: return new Vector2Int(-1, 0);
            case Dir.Up: return new Vector2Int(0, 1);
            case Dir.Right: return new Vector2Int(1, 0);
        }
    }
}
