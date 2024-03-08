using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestHeight : MonoBehaviour
{
   private float restHeight = 1.163706f;
public void RestHeightRest(){

      Vector3 newPosition = this.transform.position;
       newPosition.y = restHeight;
       this.transform.position = newPosition;
}
}
