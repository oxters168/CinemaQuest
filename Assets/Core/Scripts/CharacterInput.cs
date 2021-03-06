﻿using UnityEngine;
using UnityHelpers;
using Mirror;
using System;

public class CharacterInput : NetworkBehaviour
{
    private Quaternion handCorrection = Quaternion.Euler(90, 0, 0);
    
    private CharacterAppearance Appearance { get { if (_Appearance == null) _Appearance = GetComponent<CharacterAppearance>(); return _Appearance; } }
    private CharacterAppearance _Appearance;

    private bool isOwner;
    public bool isVREnabled;
    public Transform leftHand, rightHand, head;
    public OVRRigInfo ovrRig;
    public Transform ovrRigFollowTransform;
    private CharacterAppearance.TransformIK[] rigData;
    public Transform ikParent;
    private bool sentPlay, sentPause;

    void Start()
    {
        if (!NetworkServer.active && !NetworkClient.active)
        {
            isVREnabled = true;
            ovrRig = NetworkingCalls.networkingCallsInScene.ovrRig;
            NetworkingCalls.networkingCallsInScene.localClientCharacter = gameObject;
        }
    }
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        Debug.LogWarning("CharacterInput: OnStartAuthority");
        isOwner = true;
        ovrRig = NetworkingCalls.networkingCallsInScene.ovrRig;
        NetworkingCalls.networkingCallsInScene.localClientCharacter = gameObject;
    }
    
    void Update()
    {
        if (NetworkServer.active || NetworkClient.active)
            isVREnabled = isOwner;
        
        PlaybackControls();

        Appearance.hideHead = isVREnabled;
        if (ovrRig != null)
        {
            ovrRig.gameObject.SetActive(isVREnabled);
            ovrRig.transform.position = ovrRigFollowTransform.position;
            ovrRig.transform.rotation = ovrRigFollowTransform.rotation;

            //SetRigData(ovrRig.leftController.position, ovrRig.leftController.rotation, ovrRig.rightController.position, ovrRig.rightController.rotation, ovrRig.headTarget.position);
            leftHand.position = ovrRig.leftController.position;
            leftHand.rotation = ovrRig.leftController.rotation;
            rightHand.position = ovrRig.rightController.position;
            rightHand.rotation = ovrRig.rightController.rotation;
            head.position = ovrRig.headTarget.position;
        }

        SetRigData(leftHand.position, leftHand.rotation, rightHand.position, rightHand.rotation, head.position);
        
        if (rigData != null)
            Appearance.SetIKTargets(rigData);
        //else
        //    Debug.LogWarning("CharacterInput: No rig data received");
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

    private void PlaybackControls()
    {
        bool bPressed = OVRInput.Get(OVRInput.RawButton.B, OVRInput.Controller.All);
        bool yPressed = OVRInput.Get(OVRInput.RawButton.Y, OVRInput.Controller.All);
        if (bPressed && !sentPlay)
        {
            SendPlay();
            sentPlay = true;
        }
        else if (!bPressed)
            sentPlay = false;

        if (yPressed && !sentPause)
        {
            SendPause();
            sentPause = true;
        }
        else if (!yPressed)
            sentPause = false;
    }

    public void SendPlay()
    {
        DoTheThing(
            () =>
            {
                NetworkingCalls.networkingCallsInScene.CallPlayEventAsServer();
            },
            () =>
            {
                CmdCallPlay();
            }
        );
    }
    public void SendPause()
    {
        DoTheThing(
            () =>
            {
                NetworkingCalls.networkingCallsInScene.CallPauseEventAsServer();
            },
            () =>
            {
                CmdCallPause();
            }
        );
    }
    public void SendSeek(long frame)
    {
        DoTheThing(
            () =>
            {
                NetworkingCalls.networkingCallsInScene.CallSeekEventAsServer(frame);
            },
            () =>
            {
                CmdCallSeek(frame);
            }
        );
    }

    private void DoTheThing(Action ifNonNetworked, Action ifNetworked)
    {
        if (NetworkServer.active || NetworkClient.active)
        {
            if (isOwner)
            {
                ifNetworked?.Invoke();
            }
        }
        else
        {
            ifNonNetworked?.Invoke();
        }
    }

    [Command]
    private void CmdCallPlay()
    {
        NetworkingCalls.networkingCallsInScene.CallPlayEventAsServer();
    }
    [Command]
    private void CmdCallPause()
    {
        NetworkingCalls.networkingCallsInScene.CallPauseEventAsServer();
    }
    [Command]
    private void CmdCallSeek(long frame)
    {
        NetworkingCalls.networkingCallsInScene.CallSeekEventAsServer(frame);
    }
}
