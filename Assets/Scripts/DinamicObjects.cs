using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinamicObjects : MonoBehaviour {

    public PlayerPhysics playerIsHere;

    public GameObject buttonToChangeFrom;
    public GameObject buttonToChangeTo;
    public GameObject objectToChangeFrom;
    public GameObject objectToChangeTo;



    void Update()
    {
        if (playerIsHere.isInFrontOfTheButton)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                objectToChangeFrom.SetActive(false);
                objectToChangeTo.SetActive(true);
                buttonToChangeFrom.SetActive(false);
                buttonToChangeTo.SetActive(true);
            }
        }
    }
    
}
