

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityManagerTeleportWalking : MonoBehaviour
{
    public GameObject toDisableOnStart;
public GameObject toDisableOnMid;
    public MyProximityRunnerTeleportWalking myrunner;
    
public void StartExperiment(){
    if(toDisableOnMid.activeSelf==false){
    toDisableOnStart.SetActive(false);
    myrunner.GetAIReady();
    }

    else{
 toDisableOnMid.SetActive(false);
 myrunner.ContinueExperimentAfterBlockInstructions();
    }
}


}
