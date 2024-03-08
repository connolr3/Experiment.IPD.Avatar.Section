
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;



public class OculusUIINteraction : MonoBehaviour
{
    public OVRInput.Controller controller; // specify the controller (Left or Right)
    public LayerMask uiLayer;

    [System.Serializable]
    public class ButtonClickEvent : UnityEngine.Events.UnityEvent<GameObject> { }

    public ButtonClickEvent onButtonClick;

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller))
        {
            GameObject clickedObject = GetClickedObject();
            if (clickedObject != null && IsOnUILayer(clickedObject))
            {
                onButtonClick.Invoke(clickedObject);
            }
        }
    }

    private GameObject GetClickedObject()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = new Vector2(Screen.width / 2, Screen.height / 2);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        if (results.Count > 0)
        {
            return results[0].gameObject;
        }

        return null;
    }

    private bool IsOnUILayer(GameObject obj)
    {
        return (1 << obj.layer & uiLayer) != 0;
    }
}
