using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Inworld;
public class MyProximityRunner : MonoBehaviour
{
    public Transform[] spawnPoints;

    public GameObject[] Femalecharacters;
  //  public GameObject[] Malecharacters;
    public GameObject blockStartIndicator;
    public string[] AIGender = { "Female", "Male" };
    public string[] Locomotion = { "Walking", "Teleportation" };
    public int TrialRepetitions;


    public GameObject participantObject;
    public GameObject Finished;

    private int currentTrial = 0;
    private int currentBlock = 0;
    private List<ExpTrial> experimentDesign = new List<ExpTrial>();

    private int LastTrial;

    public SendPosition sendPositionScript;

    public InputActionProperty ComfortableLeft;
    public InputActionProperty ComfortableRight;

    private List<GameObject> availableFemaleSets = new List<GameObject>();
    private List<GameObject> availableMaleSets = new List<GameObject>();
    private bool runningExperiment = false;

public OVRInput.Controller controller; // specify the controller (Left or Right)

    void Start()
    {
        InvokeRepeating("SetAnimatorOn", 0f, 0.3f);
        LastTrial = TrialRepetitions * Locomotion.Length ;
        Debug.Log("Number of Trials: " + LastTrial);

        GenerateExpDesign();
        DebugExpDesign();

        Debug.Log("Starting Experiment");
    }

    private bool buttonPressed = false;
    private float buttonHoldDuration = 0f;
    private float requiredHoldDuration = 1.5f; // 3 se
    public CircleFillHandler fillHandler;
    float MapValue(float value, float inMin, float inMax, float outMin, float outMax)
    {
        float mappedValue = (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
        return Mathf.Clamp(mappedValue, outMin, outMax);
    }

    public GameObject circleIndicator;

    void Update()
    {
     
        if (runningExperiment)
        {
            if (OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger) > 0.5f || OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger) > 0.5f)
            {
                buttonPressed = true;
                circleIndicator.SetActive(true);
                buttonHoldDuration += Time.deltaTime;
                // float originalValue = 2.0f; // Replace with your actual value
                float mappedValue = MapValue(buttonHoldDuration, 0f, requiredHoldDuration, 0f, 100f);
                CircleFillHandler.fillValue = mappedValue;
            }
            else
            {
                CircleFillHandler.fillValue = 0;
                circleIndicator.SetActive(false);
                if (buttonPressed)
                {

                    buttonPressed = false;

                    if (buttonHoldDuration >= requiredHoldDuration)
                    {
                        // Button held for at least 3 seconds
                        if (experimentDesign[currentTrial].Locomotion == "Walking")
                            sendPositionScript.AddComfortDistanceEventWalk();
                        else
                            sendPositionScript.AddComfortDistanceEventTeleport();

                        bool IsStartingNewBlock = false;
                      
                        if (currentTrial + 1 < LastTrial)
                            IsStartingNewBlock = experimentDesign[currentTrial + 1].Locomotion == "Teleportation" && firstTeleport;
                        if (currentTrial + 1 != LastTrial && !IsStartingNewBlock)
                            TellParticipantToTurnAround();
                        SetUpNextTrial();
                    }
                    // Reset button hold duration
                    buttonHoldDuration = 0f;
                }
            }


        }
    }

    void TellParticipantToTurnAround()
    {
        participantObject.SetActive(true);
        Invoke("DeactivateParticipantObject", 5f);
    }

    void DeactivateParticipantObject()
    {
        participantObject.SetActive(false);
    }

    void GenerateExpDesign()
    {
        List<GameObject> availableFemaleSets = new List<GameObject>(Femalecharacters);
        //    List<CharacterSet> availableMaleSets = new List<CharacterSet>(MalecharacterSets);

        foreach (string locomotion in Locomotion)
        {
            //   List<string> genders = new List<string>(AIGender);

            // Duplicate each element in the genders list
            //  genders = genders.SelectMany(g => Enumerable.Repeat(g, TrialRepetitions)).ToList();

            // Randomize the order of genders
            System.Random rng = new System.Random();
            //  genders = genders.OrderBy(x => rng.Next()).ToList();
            for (int i = 0; i < TrialRepetitions; i++)
            {
                GameObject selectedFemale = null;
                if (availableFemaleSets.Count > 0)
                {
                    // Randomly select a Female character set
                    int randomIndex = rng.Next(availableFemaleSets.Count);
                    selectedFemale = availableFemaleSets[randomIndex];

                    // Remove the selected set from the available sets
                    availableFemaleSets.RemoveAt(randomIndex);
                }
                // Add the trial with the selected character set
                experimentDesign.Add(new ExpTrial(locomotion, selectedFemale));

            }

        }
    }

    public void ContinueExperimentAfterBlockInstructions()
    {
        if (currentTrial != LastTrial)
        {
            ExpTrial thisTrial = experimentDesign[currentTrial];
            GameObject thisGameObject = thisTrial.TrialCharacter;
            GetAIReady();
        }
    }

    // Rest of your code remains the same


    private bool firstTeleport = true;

    //run when the player hits the button
    public void SetUpNextTrial()
    {

        currentTrial++;
        if (currentTrial != 0)
        {
            experimentDesign[currentTrial - 1].TrialCharacter.SetActive(false);
        }
        if (currentTrial == LastTrial)
        {
            Debug.Log("Ending Experiment");
            EndExperiment();
            return;
        }


        Debug.Log("Trial Now Set to:" + currentTrial);
        // Debug.Log(experimentDesign[currentTrial].Locomotion);

        if (experimentDesign[currentTrial].Locomotion == "Teleportation" && firstTeleport)
        {
            Debug.Log("block");
            StartNewBlock();
        }
        else
        {
            GetAIReady();
        }


    }

    private Animator animator;

    private GameObject thisAI;
    //when the player is ready for the next ai
    public void GetAIReady()
    {
        runningExperiment = true;
        thisAI = experimentDesign[currentTrial].TrialCharacter;
        thisAI.SetActive(true);
     //   InworldController.CurrentCharacter = thisAI;
         Inworld.InworldCharacter inworldCharacterComponent = thisAI.GetComponent<Inworld.InworldCharacter>();
        if (inworldCharacterComponent != null)
        {
            InworldController.CurrentCharacter = inworldCharacterComponent;
        }
        else
        SetAnimatorOn();
        DisableCanvas();
        matchTransform(thisAI, spawnPoints[currentTrial % 2]);
        setCurrentCharacter(thisAI);
    }
    public void SetAnimatorOn() {
        if (runningExperiment) {
            animator = thisAI.GetComponent<Animator>();
            // Check if the Animator component is found
            if (animator != null)
            {
                // Activate or enable the Animator component
                animator.enabled = true;
            }
            else
            {
                Debug.LogError("Animator component not found on thisAI.");
            }

        }

    }

    //helper functions
    public void DisableCanvas()
    {
        GameObject canvasObject = thisAI.transform.Find("Canvas").gameObject;
        if (canvasObject != null)
        {
            Canvas canvasComponent = canvasObject.GetComponent<Canvas>();
            canvasComponent.enabled = false;
        }
    }
    // Optional Pre-Trial code. Useful for waiting for the participant to
    // do something before each trial (multiple frames). Also might be useful for fixation points etc.

    private void EndExperiment()
    {
        runningExperiment = false;
        Finished.SetActive(true);

    }
    private void StartNewBlock()
    {
        DeactivateParticipantObject();
        componentSet(true);//allow teleportation components
        firstTeleport = false;

        currentBlock++;
        Debug.Log($"Starting Block {currentBlock}");
        // Display the blockStartIndicator GameObject
        if (blockStartIndicator != null)
        {
            blockStartIndicator.SetActive(true);
        }
        // Additional logic or actions for the start of a new block can be added here
    }





    public void matchTransform(GameObject toChange, Transform toMatch)
    {
        toChange.transform.position = toMatch.position;
        toChange.transform.rotation = toMatch.rotation;
    }

    public void setCurrentCharacter(GameObject CharacterObj)
    {

        Inworld.InworldCharacter inworldCharacterComponent = CharacterObj.GetComponent<Inworld.InworldCharacter>();
        if (inworldCharacterComponent != null)
        {
            InworldController.CurrentCharacter = inworldCharacterComponent;
        }
        else
        {
            Debug.LogError("InworldCharacter component not found on the GameObject.");
        }
        Debug.Log("Inworld current character set to:" + InworldController.CurrentCharacter);
    }
    void DebugExpDesign()
    {
        Debug.Log("Experimental Design:");

        foreach (ExpTrial trial in experimentDesign)
        {
            string characterInfo = trial.TrialCharacter != null
                ? $"Character: {trial.TrialCharacter.name}, Texture: NoTexture"
                : "Character: null, Texture: null";

            Debug.Log($"Gender: Female (always), Locomotion: {trial.Locomotion}, {characterInfo}");
        }
    }
    public GameObject floor;
    public string[] components;
    public void componentSet(bool enabled)
    {
        if (floor != null)
        {
            foreach (string componentName in components)
            {
                // Try to find the component by name
                MonoBehaviour component = floor.GetComponent(componentName) as MonoBehaviour;

                // If the component is found, disable it
                if (component != null)
                {
                    component.enabled = enabled;
                    Debug.Log($"Component: {componentName}. is enabled:{enabled}");
                }
                else
                {
                    Debug.LogWarning($"Component not found: {componentName}");
                }
            }
        }
    }




    // Helper class to represent a single experimental trial
    private class ExpTrial
    {
       // public string Gender { get; }
        public string Locomotion { get; }
        public GameObject TrialCharacter { get; }

        public ExpTrial( string locomotion, GameObject trialCharacter)
        {
         //   Gender = gender;
            Locomotion = locomotion;
            TrialCharacter = trialCharacter;
        }
    }

}
