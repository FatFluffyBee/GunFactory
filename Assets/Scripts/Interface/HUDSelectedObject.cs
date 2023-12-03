using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDSelectedObject : MonoBehaviour
{
    public static HUDSelectedObject instance;

    private void Awake()
    {
        instance = this;
    }

    public HUDFactory hudFactory;
    public GameObject HUDRecipeChange;

    GameObject activeInterfaceWindow;

    public void SelectedObjectHaveBeenChanged(PlacedObject selectedObject)
    {
        if(activeInterfaceWindow!= null)
            activeInterfaceWindow.SetActive(false);

        if (selectedObject.GetComponent<Bld_Producer>() != null)
        {
            activeInterfaceWindow = hudFactory.gameObject;
            hudFactory.gameObject.SetActive(true);
            hudFactory.SetHUDSelectedObject(selectedObject);
            HUDRecipeChange.SetActive(true);
        }
    }
}

    
