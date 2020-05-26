using UnityEngine;
using UnityHelpers;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class NetworkingCalls : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public UnityEngine.Events.UnityEvent onPlay;
    public UnityEngine.Events.UnityEvent onPause;

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            Debug.LogWarning("PUN: Left room on disable");
        }
    }

    public void ConnectToPun()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom("TestRoom");
    }
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom("TestRoom");
    }
    public override void OnConnectedToMaster()
    {
        DebugPanel.Log("PUN", "Connected", 3f);
    }
    public override void OnJoinedRoom()
    {
        DebugPanel.Log("PUN", "Joined Room", 3f);
    }
    public override void OnCreatedRoom()
    {
        DebugPanel.Log("PUN", "Created Room", 3f);
    }
    public override void OnErrorInfo(Photon.Realtime.ErrorInfo errorInfo)
    {
        DebugPanel.Log("PUN", errorInfo.Info, 10f);
        Debug.LogError(errorInfo.Info);
    }

    public void CallPlayEvent()
    {
        if (PhotonNetwork.InRoom)
            RaiseNetworkedEvent(1); // Custom Event 1: Used as "PlayTogether" event
        else
            InvokePlay();
    }
    public void CallPauseEvent()
    {
        if (PhotonNetwork.InRoom)
            RaiseNetworkedEvent(2); // Custom Event 2: Used as "PauseTogether" event
        else
            InvokePause();
    }

    private void RaiseNetworkedEvent(byte eventCode)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(eventCode, null, raiseEventOptions, sendOptions);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 1)
            InvokePlay();
        else if (photonEvent.Code == 2)
            InvokePause();
    }

    private void InvokePlay()
    {
        onPlay?.Invoke();
    }
    private void InvokePause()
    {
        onPause?.Invoke();
    }
}
