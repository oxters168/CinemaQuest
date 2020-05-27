using UnityEngine;
using UnityHelpers;
using Photon.Pun;

public class CharacterInput : MonoBehaviour, IPunObservable
{
    private PhotonView PV { get { if (_pv == null) _pv = GetComponent<PhotonView>(); return _pv; } }
    private PhotonView _pv;

    private Quaternion handCorrection = Quaternion.Euler(90, 0, 0);
    
    private CharacterAppearance Appearance { get { if (_Appearance == null) _Appearance = GetComponent<CharacterAppearance>(); return _Appearance; } }
    private CharacterAppearance _Appearance;

    public bool isVREnabled;
    public OVRRigInfo ovrRig;
    public Transform ovrRigFollowTransform;
    private CharacterAppearance.TransformIK[] rigData;
    public Transform ikParent;

    void Update()
    {
        if (PV != null && PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            isVREnabled = PV.IsMine;
        
        Appearance.hideHead = isVREnabled;
        if (ovrRig != null)
        {
            ovrRig.gameObject.SetActive(isVREnabled);
            ovrRig.transform.position = ovrRigFollowTransform.position;
            ovrRig.transform.rotation = ovrRigFollowTransform.rotation;

            SetRigData(ovrRig.leftController.position, ovrRig.leftController.rotation, ovrRig.rightController.position, ovrRig.rightController.rotation, ovrRig.headTarget.position);
        }
        
        Appearance.SetIKTargets(rigData);
    }

    private void SetRigData(Vector3 leftHandPosition, Quaternion leftHandRotation, Vector3 rightHandPosition, Quaternion rightHandRotation, Vector3 headPosition)
    {
        rigData = new CharacterAppearance.TransformIK[] {
            new CharacterAppearance.TransformIK()
            {
                id = CharacterAppearance.ikId.leftHand,
                position = ikParent.InverseTransformPoint(leftHandPosition),
                rotation = ikParent.InverseTransformRotation(leftHandRotation) * handCorrection
            },
            new CharacterAppearance.TransformIK()
            {
                id = CharacterAppearance.ikId.rightHand,
                position = ikParent.InverseTransformPoint(rightHandPosition),
                rotation = ikParent.InverseTransformRotation(rightHandRotation) * handCorrection
            },
            new CharacterAppearance.TransformIK()
            {
                id = CharacterAppearance.ikId.head,
                position = ikParent.InverseTransformPoint(headPosition)
            }
        };
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo messageInfo)
    {
        if (ovrRig != null && isVREnabled && PV != null && PV.IsMine && stream.IsWriting)
        {
            stream.SendNext(ovrRig.leftController.position);
            stream.SendNext(ovrRig.leftController.rotation);
            stream.SendNext(ovrRig.rightController.position);
            stream.SendNext(ovrRig.rightController.rotation);
            stream.SendNext(ovrRig.headTarget.position);
        }
        if ((ovrRig == null || !isVREnabled) && ovrRig == null && PV != null && !PV.IsMine && stream.IsReading)
        {
            var leftHandPosition = (Vector3)stream.ReceiveNext();
            var leftHandRotation = (Quaternion)stream.ReceiveNext();
            var rightHandPosition = (Vector3)stream.ReceiveNext();
            var rightHandRotation = (Quaternion)stream.ReceiveNext();
            var headPosition = (Vector3)stream.ReceiveNext();

            SetRigData(leftHandPosition, leftHandRotation, rightHandPosition, rightHandRotation, headPosition);
        }
    }
}
