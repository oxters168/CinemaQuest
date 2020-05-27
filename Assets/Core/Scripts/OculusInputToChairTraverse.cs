using UnityEngine;

public class OculusInputToChairTraverse : MonoBehaviour
{
    private ChairTraverser Traverser { get { if (_traverser == null) _traverser = GetComponent<ChairTraverser>(); return _traverser; } }
    private ChairTraverser _traverser;
    private bool aWasPressed;

    void Update()
    {
        bool aPressed = OVRInput.Get(OVRInput.RawButton.A, OVRInput.Controller.All);
        Traverser.highlightChairs = aPressed;
        Traverser.teleport = aWasPressed && !aPressed;

        aWasPressed = aPressed;
    }
}
