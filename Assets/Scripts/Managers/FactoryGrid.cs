using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryGrid : MonoBehaviour
{
    public GunBehavior GunBehavior;
    public static FactoryGrid instance;

    [SerializeField] private List<PlacableObjectSO> arrayBuildingSO;
    private PlacableObjectSO placableObjectSO;

    private GameObject buildingGhost;

    [SerializeField] private Material validMaterial, invalidMaterial;

    private Grid<FactoryGridObject> factoryGrid;
    private PlacableObjectSO.Dir dir = PlacableObjectSO.Dir.Down;
    private bool isBuilding = false;

    Vector2Int currentCaseHovered = new Vector2Int(-1, -1);
    Vector3 currentMouseLocation = new Vector3(0, 0);

    private PlacedObject selectedObject;

    private void Awake()
    {
        instance = this;

        int gridWidth = 10;
        int gridHeight = 10;
        float cellSize = 10f;
        factoryGrid = new Grid<FactoryGridObject>(gridWidth, gridHeight, cellSize, Vector3.zero, (Grid<FactoryGridObject> g, int x, int z) => new FactoryGridObject(g, x, z));

        placableObjectSO = arrayBuildingSO[0];
    }

    private void Update() ////A TERME GARDERR UNIQUEMENT LES FONCTIONS QUI SERONT APPELLEES PAR LES INPUTS DANS UN AUTRE SCRIPT
    {
        List<Vector2Int> gridPositions = new List<Vector2Int>();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 999f, LayerMask.GetMask("MouseLayerMask")))
        {
            currentMouseLocation = hit.point;
            currentCaseHovered = factoryGrid.GetXZ(hit.point);

            gridPositions = placableObjectSO.GetGridPositionList(new Vector2Int(currentCaseHovered.x, currentCaseHovered.y), dir);
        }

        bool canBuild = CanBuild(gridPositions);

        ////////////////////// Mini script pour selectionner un objet
        
        if(Input.GetMouseButtonDown(0) && !isBuilding)
        {
            if(factoryGrid.GetGridObject(currentCaseHovered)?.GetPlacedObject())
            {
                if(factoryGrid.GetGridObject(currentCaseHovered).GetPlacedObject() != selectedObject)
                {
                    selectedObject = factoryGrid.GetGridObject(currentCaseHovered).GetPlacedObject();
                    HUDSelectedObject.instance.SelectedObjectHaveBeenChanged(selectedObject);
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////

        if (isBuilding)
        {
            if (Input.GetMouseButtonDown(0)) //on recupère le grisd object touché et si il existe on essaie d'ajouter le batiment
            {
                if (canBuild)
                {
                    Vector2Int rotationOffset = placableObjectSO.GetRotationOffset(dir);
                    Vector3 buildingPosition = factoryGrid.GetWorldPosition(currentCaseHovered.x, currentCaseHovered.y) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * factoryGrid.GetCellSize();

                    PlacedObject instance = Instantiate(placableObjectSO.prefab, buildingPosition, Quaternion.Euler(0, PlacableObjectSO.GetRotationAngle(dir), 0)).GetComponent<PlacedObject>();

                    foreach (Vector2Int gridPos in gridPositions)
                    {
                        factoryGrid.GetGridObject(gridPos.x, gridPos.y).SetPlacedObject(instance, gridPositions, dir);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                dir = PlacableObjectSO.GetNextDir(dir, true);
            }

            if (Input.GetMouseButtonDown(1))
            {
                ExitBuildingMode();
            }

            UpdateGhostBuildingVisual(canBuild);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            int x = -10;
            int z = -10;

            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 999f, LayerMask.GetMask("MouseLayerMask")))
            {
                factoryGrid.GetXZ(hit.point, out x, out z);
            }

            if (factoryGrid.GetGridObject(x, z).GetPlacedObject() != null)
                ClearGridOfBuilding(x, z);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) { SwitchBuilding(0); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { SwitchBuilding(1); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { SwitchBuilding(2); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { SwitchBuilding(3); }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { SwitchBuilding(4); }
        if (Input.GetKeyDown(KeyCode.Alpha6)) { SwitchBuilding(5); }
        if (Input.GetKeyDown(KeyCode.Alpha7)) { SwitchBuilding(6); }
        if (Input.GetKeyDown(KeyCode.Alpha8)) { SwitchBuilding(7); }
        if (Input.GetKeyDown(KeyCode.Alpha9)) { SwitchBuilding(8); }
    }

    private void SwitchBuilding(int i)
    {
        if (!isBuilding)
            StartBuildingMode();

        if (i < 0 || i > arrayBuildingSO.Count)
            return;

        placableObjectSO = arrayBuildingSO[i];
        dir = PlacableObjectSO.Dir.Down;
    }

    private void ClearGridOfBuilding(int x, int z)
    {
        PlacedObject placedObject = factoryGrid.GetGridObject(x, z).GetPlacedObject();
        List<Vector2Int> buildingPosToRemove = factoryGrid.GetGridObject(x, z).GetBuildingPos();

        foreach(Vector2Int e in buildingPosToRemove) 
        {
            factoryGrid.GetGridObject(e.x, e.y).SetPlacedObject(null, null, PlacableObjectSO.Dir.Down);
        }

        placedObject.OnSupression();
        Destroy(placedObject.gameObject);
    }

    private void StartBuildingMode()
    {
        isBuilding = true;
    }

    private void ExitBuildingMode()
    {
        isBuilding = false;
        DeleteGhostBuilding();
    }

    public FactoryGridObject GetGridObject(Vector2Int arrayPos)
    {
        return factoryGrid.GetGridObject(arrayPos);
    }

    private bool CanBuild(List<Vector2Int> gridPositions)
    {
        if (!factoryGrid.IsInGrid(gridPositions))
        {
            return false;
        }

        foreach (Vector2Int gridPosition in gridPositions)
        {
            if (!factoryGrid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
            {
                return false;
            }
        }
        return true;
    }

    public void UpdateGhostBuildingVisual(bool canBuild)
    {
        if (buildingGhost != null)
            Destroy(buildingGhost.gameObject);

        if (!isBuilding)
        {
            return;
        }
            
        buildingGhost = Instantiate(placableObjectSO.phantomVisual, Vector3.zero, Quaternion.identity).gameObject;

        MeshRenderer mR = buildingGhost.transform.GetChild(0).GetComponent<MeshRenderer>();

        if (canBuild)
            mR.material = validMaterial;
        else
            mR.material = invalidMaterial;


        Vector2Int offset = placableObjectSO.GetRotationOffset(dir);
        Vector3 offsetBuilding = new Vector3(offset.x, 0, offset.y) * factoryGrid.GetCellSize();

        if (factoryGrid.IsInGrid(currentCaseHovered))
        {
            
            buildingGhost.transform.position = factoryGrid.GetWorldPosition(currentCaseHovered.x, currentCaseHovered.y) + offsetBuilding;

        }
        else
            buildingGhost.transform.position = currentMouseLocation + offsetBuilding;

        buildingGhost.transform.rotation = Quaternion.Euler(0, PlacableObjectSO.GetRotationAngle(dir), 0);
    }

    public void DeleteGhostBuilding()
    {
        if(buildingGhost != null)
            Destroy(buildingGhost);
    }

    public Vector3 GetWorldPosition(Vector2Int pos)
    {
        return factoryGrid.GetWorldPosition(pos.x, pos.y);
    }

    public Vector2Int GetXZ(Vector3 worldPos)
    {
        return factoryGrid.GetXZ(worldPos);
    }

    public PlacedObject GetSelectedObject()
    {
        return selectedObject;
    }
}

public class FactoryGridObject
{
    private Grid<FactoryGridObject> grid;
    private PlacedObject placedObject;
    List<Vector2Int> buildingPositions;
    private Vector2Int pos;
    private int value;

    public FactoryGridObject(Grid<FactoryGridObject> grid, int x, int z)
    {
        this.grid = grid;
        pos.x = x;
        pos.y = z;
        placedObject = null;
    }

    public void SetPlacedObject(PlacedObject placedObject, List<Vector2Int> buildingPositions, PlacableObjectSO.Dir dir)
    {
        this.placedObject = placedObject;
        placedObject?.SetOriginAndDirection(new Vector2Int(pos.x, pos.y), dir, this);
        this.buildingPositions = buildingPositions;
        grid.TriggerGridObjectChanged(pos.x, pos.y);
    }

    public void ClearBuilding()
    {
        placedObject = null;
    }

    public bool CanBuild()
    {
        return placedObject == null;
    }

    public PlacedObject GetPlacedObject()
    {
        return placedObject;
    }

    public List<Vector2Int> GetBuildingPos()
    {
        return buildingPositions;
    }

    public override string ToString()
    {
        return pos.x.ToString() + " , " + pos.y.ToString() + "\n\n " + (placedObject ? value  : "");
    }

    public void SetValue(int value)
    {
        this.value = value;
        grid.TriggerGridObjectChanged(pos.x, pos.y);
    }

}
