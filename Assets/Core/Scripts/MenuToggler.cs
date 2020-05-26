using UnityEngine;

public class MenuToggler : MonoBehaviour
{
    public GameObject toggleObject;
    private bool buttonPressed;

    void Update()
    {
        bool menuButtonDown = OVRInput.Get(OVRInput.RawButton.Start, OVRInput.Controller.All);
        if (menuButtonDown && !buttonPressed)
        {
            toggleObject.SetActive(!toggleObject.activeSelf);
            buttonPressed = true;
        }
        else if (!menuButtonDown)
            buttonPressed = false;
    }
}
