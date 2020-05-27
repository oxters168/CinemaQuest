using UnityEngine;
using UnityHelpers;

public class CharacterInput : MonoBehaviour
{
    private Quaternion handCorrection = Quaternion.Euler(90, 0, 0);
    
    private CharacterAppearance Appearance { get { if (_Appearance == null) _Appearance = GetComponent<CharacterAppearance>(); return _Appearance; } }
    private CharacterAppearance _Appearance;

    public bool isVREnabled;
    public GameObject ovrRig;
    public Transform rightController, leftController;
    public Transform headTarget;
    public Transform ikParent;

    void Update()
    {
        Appearance.hideHead = isVREnabled;
        ovrRig.SetActive(isVREnabled);
        
        Appearance.SetIKTargets(
            new CharacterAppearance.TransformIK()
            {
                id = CharacterAppearance.ikId.leftHand,
                position = ikParent.InverseTransformPoint(leftController.position),
                rotation = ikParent.InverseTransformRotation(leftController.rotation) * handCorrection
            },
            new CharacterAppearance.TransformIK()
            {
                id = CharacterAppearance.ikId.rightHand,
                position = ikParent.InverseTransformPoint(rightController.position),
                rotation = ikParent.InverseTransformRotation(rightController.rotation) * handCorrection
            },
            new CharacterAppearance.TransformIK()
            {
                id = CharacterAppearance.ikId.head,
                position = ikParent.InverseTransformPoint(headTarget.position)
            }
        );
    }
}
