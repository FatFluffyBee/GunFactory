using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid <TGridObject>
{
    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
    public class OnGridValueChangedEventArgs : EventArgs
    {
        public int x;
        public int z;
    }

    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private TGridObject[,] gridArray;
    private TextMesh[,] debugTextArray;

    public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<Grid<TGridObject>, int, int, TGridObject> createGridObject) //we use it to import constructor of whatever the object will be
    {
        this.width = width; //x
        this.height = height; //y
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new TGridObject[width, height];

        for (int x = 0; x < width; x++)
            for (int z = 0; z < height; z++)
            {
                gridArray[x, z] = createGridObject(this, x, z);
            }

        bool debugEnabled = true;
        if(debugEnabled)
        {
            debugTextArray = new TextMesh[width, height];

            for (int x = 0; x < width; x++)
                for (int z = 0; z < height; z++)
                {
                    debugTextArray[x, z] = CreateWorldText(null, gridArray[x, z]?.ToString(), GetWorldPosition(x, z) + new Vector3(cellSize, 0, cellSize) * 0.5f, 20, Color.white, TextAnchor.MiddleCenter, TextAlignment.Center, 1);
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.white, 100);
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.white, 100);

                }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100);

            OnGridValueChanged += (object sender, OnGridValueChangedEventArgs eventArgs) =>
            { debugTextArray[eventArgs.x, eventArgs.z].text = gridArray[eventArgs.x, eventArgs.z].ToString(); };
        }
    }

    public Vector3 GetWorldPosition(int x, int z)
    {
        return new Vector3(x, 0, z) * cellSize + originPosition;
    }

    public void GetXZ(Vector3 worldPos, out int x, out int z)
    {
        x = Mathf.FloorToInt((worldPos.x - originPosition.x) / cellSize);
        z = Mathf.FloorToInt((worldPos.z - originPosition.z) / cellSize);
    }

    public Vector2Int GetXZ(Vector3 worldPos)
    {
        GetXZ(worldPos, out int x, out int z);

        return new Vector2Int(x, z);
    }

    public Vector3 GetWorldPositionOfCell (Vector3 worldPos)//
    {
        int x = Mathf.FloorToInt((worldPos.x - originPosition.x) / cellSize);
        int z = Mathf.FloorToInt((worldPos.z - originPosition.z) / cellSize);

        return GetWorldPosition(x, z);
    }

    public bool IsInGrid (Vector2Int pos)
    {
        if(pos.x < 0 || pos.x >= width || pos.y < 0 || pos.y >= height)
        {
            return false;
        }

        return true;
    }

    public bool IsInGrid(List<Vector2Int> listPos)
    {
        foreach (Vector2Int pos in listPos)
        {
            if (!IsInGrid(pos))
                return false;
        }
            
        return true;
    }


    public void TriggerGridObjectChanged(int x, int z)
    {
        if (OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, z = z });
    }

    /*public void SetGridObject(Vector3 pos, TGridObject value)
    {
        int i, j;
        GetXZ(pos, out i, out j);
        SetGridObject(i, j, value);
    }*/

    /* public void SetGridObject(int x, int z, TGridObject value)
   {
       if (x >= 0 && x >= 0 && z < width && z < height)
       {
           gridArray[x, z] = value;
           if (OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, z = z });
       }
   }*/


    public TGridObject GetGridObject(Vector2Int pos)
    {
        return GetGridObject(pos.x, pos.y);
;    }
    public TGridObject GetGridObject(int x, int z)
    {
        if (x >= 0 && z >= 0 && x < width && z < height)
            return gridArray[x, z];
        else
            return default;
    }

    public TGridObject GetGridObject(Vector3 pos)
    {
        int x, z;
        GetXZ(pos, out x, out z);
        return GetGridObject(x, z);
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public float GetCellSize()
    {
        return cellSize;
    }
    TextMesh CreateWorldText(Transform parent, string text, Vector3 localPos, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignement, int sortingOrder)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPos;
        transform.Rotate(90, 0, 0);
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignement;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.text = text;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }


}
