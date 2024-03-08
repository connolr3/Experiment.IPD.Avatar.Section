using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;//required to read input
//using Inworld;

public class MatchTransform : MonoBehaviour
{
    public GameObject toMatch;
        public GameObject rig;
    public Transform targetMatch;

     public Transform cameraOffset;
    public GameObject MainCamera;
 //public InputActionProperty ResetPosition; 

    // Update is called once per frame
void Start(){

}
    void Update(){
       // Debug.Log(InworldController.CurrentCharacter);
        if(Input.GetKey("m")){
            Match();
        }
       // if(ResetPosition.action.ReadValue<float>()>0.5f){
    // Match();
       // }
    }

    private void MoveXROriginToFinalPos()
    {
        if (rig != null && MainCamera != null)
        {
            // Calculate the offset needed to move MainCamera to finalPos (only affecting X and Z)
            Vector3 offset = new Vector3(targetMatch.position.x - MainCamera.transform.position.x, 0f, targetMatch.position.z - MainCamera.transform.position.z);

            // Move XROrigin with the calculated offset
            rig.transform.position += offset;
        }
        else
        {
            Debug.LogWarning("Set 'XROrigin' and 'MainCamera' references in the inspector.");
        }
    }


    public void Match()
    {

        
            if (toMatch != null && targetMatch != null)
            {
                MoveXROriginToFinalPos();
                


            float totalYRotation = targetMatch.eulerAngles.y;
            float currentYRotation = MainCamera.transform.eulerAngles.y;

            // Calculate the rotation needed to go from the current Y rotation to the total Y rotation
            float deltaRotation = totalYRotation - currentYRotation;
            Quaternion rotationNeeded = Quaternion.Euler(0f, deltaRotation, 0f);

            // Apply the calculated rotation to toMatch
            toMatch.transform.rotation *= rotationNeeded;

cameraOffset.position = new Vector3(cameraOffset.position.x, -1*MainCamera.transform.localPosition.y, cameraOffset.position.z);

            }
            else
            {
                Debug.LogWarning("Set 'toMatch' and 'targetMatch' references in the inspector.");
            }
        
    }  
}
