using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disable : MonoBehaviour
{
    public GameObject toDisable;
    public GameObject[] toDisableAtStart;

    public GameObject[] toDisableArray;

    // Update is called once per frame

    void Start(){
        if (toDisableAtStart != null)
        {
            foreach (GameObject obj in toDisableAtStart)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }
        Invoke("DisableAIs",2f);
    }

  public void   DisableAIs(){
    // Disable all objects in the array
        if (toDisableArray != null)
        {
            foreach (GameObject obj in toDisableArray)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }
    }
    public void DisableObj()
    {
        toDisable.SetActive(false);
    }
}
