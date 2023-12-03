using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridClass <TGridObject>
{
   /* public const int HEAT_MAP_MAX_VALUE = 100;
    public const int HEAT_MAP_MIN_VALUE = 0;

    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
    public class OnGridValueChangedEventArgs : EventArgs
    {
        public int i;
        public int j;
    }

    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private TGridObject[,] gridArray;

    private TextMesh[,] debugTextArray;

    public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<Grid<TGridObject>, int, int, int, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new TGridObject[width, height];

        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                gridArray[i, j] = createGridObject(this, 0, i, j);
            }

                bool showDebug = true;
        if(showDebug)
        {
            debugTextArray = new TextMesh[width, height];

            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    debugTextArray[i, j] = CreateWorldText(null, gridArray[i, j]?.ToString(), GetWorldPosition(i, j) + new Vector3(cellSize, cellSize) * 0.5f, 20, Color.white, TextAnchor.MiddleCenter, TextAlignment.Center, 1);
                    Debug.DrawLine(GetWorldPosition(i, j), GetWorldPosition(i, j + 1), Color.white, 100);
                    Debug.DrawLine(GetWorldPosition(i, j), GetWorldPosition(i + 1, j), Color.white, 100);

                }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100);

            OnGridValueChanged += (object sender, OnGridValueChangedEventArgs eventArgs) =>
            { debugTextArray[eventArgs.i, eventArgs.j].text = gridArray[eventArgs.i, eventArgs.j].ToString(); };
        }
    }

    public Vector3 GetWorldPosition(int i, int j)
    {
        return new Vector3(i, j) * cellSize + originPosition;
    }

    private void GetIJ(Vector3 worldPos, out int i, out int j)
    {
        i = Mathf.FloorToInt((worldPos.x - originPosition.x) / cellSize);
        j = Mathf.FloorToInt((worldPos.y - originPosition.y) / cellSize);
    }

    public void SetGridObject(int i, int j, TGridObject value)
    {
        if(i >= 0 &&  j >= 0 && i < width && j < height)
        {
            gridArray[i, j] = value;
            if (OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedEventArgs { i = i, j = j });
        }
    }

    public void TriggerGridObjectChanged(int i, int j)
    {
        if (OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedEventArgs { i = i, j = j });
    }

    public void SetGridObject(Vector3 pos, TGridObject value)
    {
        int i, j;
        GetIJ(pos, out i, out j);
        SetGridObject(i, j, value);
    }

    public TGridObject GetGridObject(int i, int j)
    {
        if (i >= 0 && j >= 0 && i < width && j < height)
            return gridArray[i, j];
        else
            return default;
    }

    public TGridObject GetGridObject(Vector3 pos)
    {
        int i, j;
        GetIJ(pos, out i, out j);
        return GetGridObject(i, j);
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

    /*public void AddValue(int i, int j, int value) //FOR HEAT MAP ONLY
    {
        SetValue(i, j, GetValue(i, j) + value);
    }

    public void AddValue(Vector3 worldPosition, int value, int range)
    {
        Debug.Log("Clic Ok");
        GetIJ(worldPosition, out int xOrigin, out int yOrigin);

        for(int i = 0; i < range; i++)
            for(int j = 0; j < range - i; j++)
            {
                AddValue(xOrigin + i, yOrigin + j, value);
                if(i != 0)
                    AddValue(xOrigin - i, yOrigin + j, value);
                if(j != 0)
                {
                    AddValue(xOrigin + i, yOrigin - j, value);
                    if (i != 0)
                        AddValue(xOrigin - i, yOrigin - j, value);
                }
            }
    }*/


    TextMesh CreateWorldText(Transform parent, string text, Vector3 localPos, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignement, int sortingOrder)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPos;
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
