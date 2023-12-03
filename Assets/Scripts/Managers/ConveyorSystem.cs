using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using UnityEngine;

public class ConveyorSystem : MonoBehaviour
{
    static public ConveyorSystem instance;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        TickManager.instance.OnTick.AddListener(AvanceConveyorBelts);
    }

    bool debug = true;
    private List<BeltPath> beltPaths = new List<BeltPath>();

    private void AvanceConveyorBelts() //fonctiuon qui avance tout les conveyor belts //àremplacer par directement 
    {
        foreach(BeltPath path in beltPaths) 
        { 
            List<Bld_ConveyorBelt> belts = path.GetConveyorBelts();
            bool nextIsFree = false;

            for (int i = belts.Count-1; i >= 0; i--)
            {
                if(i == belts.Count - 1) //Si le conveyor est le dernier de la liste on va verifier si il peut output son objet dans un batiment
                {
                    if (belts[i].GetHoldItem() != null)  //si il  a un bojet
                    {
                        PlacedObject endBuilding = FactoryGrid.instance.GetGridObject(belts[i].GetOriginAndDirForward())?.GetPlacedObject(); //si il yu a un batiment en face de lui
                        if(endBuilding != null)                                                                                               
                            if (endBuilding.IsPosOnInputPos(belts[i].GetOrigin()))           //Si il est positionné devant un accès de ce building                                
                            {
                                if (endBuilding.CheckIfCanSendItem(belts[i]))          //Si le building peut recevoir l'item du conveyor 
                                {
                                    if(endBuilding.GetComponent<Bld_Producer>())
                                    {
                                        endBuilding.AddItemToStorage(belts[i].GetHoldItem().itemSO);
                                        GameObject worldItem = belts[i].GetHoldItem().gameObject;
                                        belts[i].SetHoldItem(null);
                                        Destroy(worldItem);
                                    }
                                    else if(endBuilding.GetComponent<Bld_Merger>())
                                    {
                                        WorldItem worldItem = belts[i].GetHoldItem();
                                        endBuilding.GetComponent<Bld_Merger>().SetWorldItem(worldItem);
                                        worldItem.SetTargetPosition(endBuilding.GetComponent<Bld_Merger>().GetHoldPointPosition());
                                        belts[i].SetHoldItem(null);
}
                                    else if (endBuilding.GetComponent<Bld_Splitter>())
                                    {
                                        WorldItem worldItem = belts[i].GetHoldItem();
                                        endBuilding.GetComponent<Bld_Splitter>().SetWorldItem(worldItem);
                                        worldItem.SetTargetPosition(endBuilding.GetComponent<Bld_Splitter>().GetHoldPointPosition());
                                        belts[i].SetHoldItem(null);
                                    }

                                    nextIsFree = true;
                                }
                            }
                    }
                    else
                     nextIsFree = true;
                }
                else
                {
                    if (nextIsFree)
                    {
                        if (belts[i].GetHoldItem() != null && i < belts.Count - 1)
                        {
                            WorldItem worldItem = belts[i].GetHoldItem();
                            belts[i].SetHoldItem(null);
                            belts[i + 1].SetHoldItem(worldItem);
                            worldItem.SetTargetPosition(belts[i + 1].GetHoldPointPosition());
                        }
                    }

                    if (belts[i].GetHoldItem() == null)
                    {
                        nextIsFree = true;
                    }
                    else
                    {
                        nextIsFree = false;
                    }
                }
            }
        }
    }

    public bool AddConveyorToBeltPath(Bld_ConveyorBelt newConveyorBelt) //fonction qui recalcule les beltpaths
    {
        ///
        //Attention ne permet pas de faire des boucles ou les fait mais ne les incorpore pas !
        ///

        bool isStart = false;
        bool isEnd = false;

        Bld_ConveyorBelt beltAfter = null;
        Bld_ConveyorBelt beltBefore = null;

        if (beltPaths.Count >= 0) // si premier conveyor belt placé
        {
            Bld_ConveyorBelt beltToCheck = FactoryGrid.instance.GetGridObject(newConveyorBelt.GetOriginAndDirForward())?.GetPlacedObject()?.GetComponent<Bld_ConveyorBelt>();

            if (beltToCheck != null)
            {
                if (beltPaths[ReturnIndexBeltPath(beltToCheck)].PosInList(beltToCheck) == 0) //la belt selectionnée est la première de sa liste (pas d'intersection)
                    if (beltToCheck.GetOriginAndDirForward() != newConveyorBelt.GetOrigin()) //les deux belt ne se font pas face à face
                    {
                        beltAfter = beltToCheck;
                        isStart = true;
                    }
            }

            int success = 0;
            foreach (Vector2Int pos in newConveyorBelt.GetOriginAndDirSideAndBack()) //on vérifie les 3 autres côtés de la belt
            {
                beltToCheck = FactoryGrid.instance.GetGridObject(pos)?.GetPlacedObject()?.GetComponent<Bld_ConveyorBelt>();

                if (beltToCheck != null)
                {
                    if (beltToCheck.GetOriginAndDirForward() == newConveyorBelt.GetOrigin()) //la belt checkée fait face au conveyor belt
                    {
                        beltBefore = beltToCheck;
                        success++;
                    }
                }
            }

            if (success == 1) { isEnd = true; }
            if (success > 1) return false;

        }

        int beltAfterIndex = ReturnIndexBeltPath(beltAfter);
        int beltBeforeIndex = ReturnIndexBeltPath(beltBefore);

        if (isStart && isEnd) 
        {
            beltPaths[beltBeforeIndex].AddToList(newConveyorBelt, false);

            foreach(Bld_ConveyorBelt e in beltPaths[beltAfterIndex].GetConveyorBelts())
            {
                beltPaths[beltBeforeIndex].AddToList(e, false);
            }

            beltPaths.RemoveAt(beltAfterIndex);
            //Debug.Log("Is Junction");
        }

        if(isStart && !isEnd)
        {
            beltPaths[beltAfterIndex].AddToList(newConveyorBelt, true);
            //Debug.Log("Is Start");
        }

        if(!isStart && isEnd)
        {
            beltPaths[beltBeforeIndex].AddToList(newConveyorBelt, false);
            //Debug.Log("Is End");

        }

        if (!isStart && !isEnd) 
        {
            List<Bld_ConveyorBelt> cBList = new List<Bld_ConveyorBelt> { newConveyorBelt };
            beltPaths.Add(new BeltPath(cBList));
            //Debug.Log("Is New");

        }

        bool debug = true;
        DebugBeltPath(debug);

        return true;
    }

    public void RemoveConveyorFromBelthPath(Bld_ConveyorBelt removedBelt)
    {
        int beltPathIndex = ReturnIndexBeltPath(removedBelt);

        int indexInConveyorList = beltPaths[beltPathIndex].PosInList(removedBelt);
        List<Bld_ConveyorBelt> list = new List<Bld_ConveyorBelt>();

        list = beltPaths[beltPathIndex].GetConveyorBelts();

        if (list.Count == 1) //la section n'est compôsée que d'un seul belt
            beltPaths.RemoveAt(beltPathIndex);

        else if(indexInConveyorList == 0 || indexInConveyorList == list.Count - 1)  //l'element a suipprimé est au début ou en fin de chemin
            beltPaths[beltPathIndex].RemoveAt(indexInConveyorList);

        else
        {
            List<Bld_ConveyorBelt> listStart = new List<Bld_ConveyorBelt>();
            List<Bld_ConveyorBelt> listEnd = new List<Bld_ConveyorBelt>();

            for (int i = 0; i < list.Count; i++)
            {
                if (i < indexInConveyorList)
                    listStart.Add(list[i]);
                else if (i > indexInConveyorList)
                    listEnd.Add(list[i]);
            }

            beltPaths[beltPathIndex] = new BeltPath(listStart);
            beltPaths.Add(new BeltPath(listEnd));
        }

        DebugBeltPath(debug);
    }


    private int ReturnIndexBeltPath(Bld_ConveyorBelt newConveyorBelt)
    {
        for (int i = 0; i < beltPaths.Count; i++)
        {
            if (beltPaths[i].IsInList(newConveyorBelt))
                return i;
        }

        //Debug.Log("Not found");
        return -1;
    }

    public void DebugBeltPath(bool displayVisualDebug) //fonction de debug qui affiche les beltpaths
    {
        Vector3 offset = new Vector3(5, 0, 5);
        int i = 0;
        string pathDebug = "";
        foreach(BeltPath beltPath in beltPaths)
        {
            i++;
            Vector3 lastPos = Vector3.zero;
            //pathDebug += "\nPath " + i + " : ";
            foreach(Bld_ConveyorBelt conveyor in beltPath.GetConveyorBelts())
            {
                if (lastPos == Vector3.zero)
                { 
                    lastPos = FactoryGrid.instance.GetWorldPosition(conveyor.GetOrigin()); 
                }
                else
                {
                    Debug.DrawLine(lastPos + offset, FactoryGrid.instance.GetWorldPosition(conveyor.GetOrigin()) + offset, Color.red, 5);
                    lastPos = FactoryGrid.instance.GetWorldPosition(conveyor.GetOrigin());
                }

                //pathDebug += conveyor.GetOrigin() + " + "; 
            }
        }
        //Debug.Log(pathDebug);
    }

    public class BeltPath
    {
        List<Bld_ConveyorBelt> conveyorBelts;

        public BeltPath(List<Bld_ConveyorBelt> conveyorBelts)
        {
            this.conveyorBelts = conveyorBelts;
        }

        public List<Bld_ConveyorBelt> GetConveyorBelts()
        {
            return conveyorBelts;
        }

        public bool IsInList(Bld_ConveyorBelt conveyorToCheck)
        {
            foreach(Bld_ConveyorBelt e in conveyorBelts)
            {
                if(conveyorToCheck == e) return true;
            }

            return false;
        }

        public int PosInList(Bld_ConveyorBelt conveyorToCheck)
        {
            for(int i = 0; i < conveyorBelts.Count; i++)
            {
                if (conveyorBelts[i] == conveyorToCheck) return i;
            }

            return -1;
        }

        public void AddToList(Bld_ConveyorBelt conveyorToCheck, bool addToStart)
        {
            if(addToStart)
            {
                conveyorBelts.Insert(0, conveyorToCheck);
            }
            else
            {
                conveyorBelts.Add(conveyorToCheck);
            }
        }

        public void RemoveAt(int index)
        {
            conveyorBelts.RemoveAt(index);
        }
    }
}
