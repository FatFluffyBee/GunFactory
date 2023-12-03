using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class TestingScript : MonoBehaviour
{
   /* [SerializeField] private HeatMapVisual heatMapVisual;
    [SerializeField] private HeatMapBoolVisual heatMapBoolVisual;
    [SerializeField] private HeatMapGenericVisual heatMapGenericVisual;
    private Grid<HeatMapGridObject>grid;

    private void Start()
    {
        grid = new Grid<HeatMapGridObject>(20, 10, 5f, Vector3.zero, (Grid<HeatMapGridObject> g, int v, int i, int j) => new HeatMapGridObject(g, v, i, j));

        //heatMapVisual.SetGrid(grid);
        //heatMapBoolVisual.SetGrid(grid);
        heatMapGenericVisual.SetGrid(grid);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 position = UtilsClass.GetMouseWorldPosition();
            HeatMapGridObject heatMapGridObject = grid.GetGridObject(position);
            if(heatMapGridObject != null) 
            {
                heatMapGridObject.AddValue(5);
            }
        }
    }
}

public class HeatMapGridObject
{
    private const int MIN = 0;
    private const int MAX = 100;

    private Grid<HeatMapGridObject> grid;
    private int value;
    private int i;
    private int j;

    public HeatMapGridObject(Grid<HeatMapGridObject> grid, int value, int i, int j)
    {
        this.grid = grid;
        this.value = value;
        this.i = i;
        this.j = j;
    }

    public void AddValue(int addValue)
    {
        value += addValue;
        Mathf.Clamp(value, MIN, MAX);
        grid.TriggerGridObjectChanged(i, j);
    }

    public float GetValueNormalized()
    {
        return (float)value / MAX;
    }

    public override string ToString()
    {
        return value.ToString();
    }*/
}
