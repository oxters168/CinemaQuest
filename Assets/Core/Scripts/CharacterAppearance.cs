using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class CharacterAppearance : MonoBehaviour
{
    public enum ikId
    {
        leftHand = 0,
        rightHand = 1,
        leftFoot = 2,
        leftThigh = 3,
        rightFoot = 4,
        rightThigh = 5,
        body = 6,
        head = 7
    }
    [System.Serializable]
    public struct TransformIK
    {
        public ikId id;
        public Vector3 position;
        public Quaternion rotation;
    }

    public CharacterCustomization maleCharacter, femaleCharacter;
    public bool isFemale;
    public bool hideHead;

    public Transform[] ikTargets;
    public List<TransformIK> transformIKs = new List<TransformIK>();

    void Update()
    {
        UpdateAppearance();
        UpdateIK();
    }

    private void UpdateAppearance()
    {
        maleCharacter.gameObject.SetActive(!isFemale);
        femaleCharacter.gameObject.SetActive(isFemale);
        if (hideHead)
        {
            maleCharacter.HideParts(new string[] { "Head", "Hair" });
            femaleCharacter.HideParts(new string[] { "Head", "Hair" });
        }
        else
        {
            maleCharacter.UnHideParts(new string[] { "Head", "Hair" }, CharacterCustomization.ClothesPartType.Hat);
            femaleCharacter.UnHideParts(new string[] { "Head", "Hair" }, CharacterCustomization.ClothesPartType.Hat);
        }
    }
    private void UpdateIK()
    {
        foreach (var transformIK in transformIKs)
        {
            var currentTarget = ikTargets[(int)transformIK.id];
            currentTarget.localPosition = transformIK.position;
            currentTarget.localRotation = transformIK.rotation;
        }
    }

    public void SetIKTargets(params TransformIK[] targets)
    {
        foreach (var target in targets)
        {
            var index = transformIKs.FindIndex((transformIK) => { return transformIK.id == target.id; });
            if (index >= 0)
                transformIKs[index] = target;
            else
                Debug.LogError("IK Error");
        }
    }
}
