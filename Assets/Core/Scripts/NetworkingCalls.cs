using UnityEngine;
using UnityHelpers;
using Mirror;
using UnityEngine.Events;

[System.Serializable]
public class FloatEvent : UnityEvent<float> {}

public class NetworkingCalls : NetworkManager
{
    public static NetworkingCalls networkingCallsInScene { get; private set; }
    public GameObject localClientCharacter;

    [Space(10)]
    public GameObject characterPrefab;
    public GameObject characterPrefabNetworked;
    private bool currentlyNetworked;
    public OVRRigInfo ovrRig;
    public Theater theater;
    public UnityVideoPlayer unityVideoPlayer;
    public bool tempCreate;
    private GameObject tempCharacter;

    [Space(10)]
    public UnityEvent onPlay;
    public UnityEvent onPause;
    public FloatEvent onScrub;

    public override void Awake()
    {
        base.Awake();
        Application.quitting += OnQuitting;
        networkingCallsInScene = this;
    }
    public override void Start()
    {
        base.Start();
        if (tempCreate)
        {
            tempCharacter = Instantiate(characterPrefab);
            tempCharacter.GetComponent<ChairTraverser>().Teleport(theater.GetRandomChair());
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.LogWarning("Mirror: OnStartClient");
        NetworkClient.RegisterHandler<PlaybackEvent>(OnPlaybackEvent);
        NetworkClient.RegisterHandler<SeekEvent>(OnSeekEvent);
        if (tempCharacter != null)
            Destroy(tempCharacter);
    }
    public override void OnStopClient()
    {
        base.OnStopClient();
        Debug.LogWarning("Mirror: OnStopClient");
        NetworkClient.UnregisterHandler<PlaybackEvent>();
        NetworkClient.UnregisterHandler<SeekEvent>();
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.LogWarning("Mirror: OnStartServer");
        if (tempCharacter != null)
            Destroy(tempCharacter);
        //InitCharacter(true);
    }
    public override void OnClientConnect(NetworkConnection conn)
    {
        //Happens on client side when connecting to the server
        base.OnClientConnect(conn);
        Debug.LogWarning("Mirror: OnClientConnect " + conn.address);
    }
    public override void OnServerConnect(NetworkConnection conn)
    {
        //Happens on server side when client connects to the server
        base.OnServerConnect(conn);
        Debug.LogWarning("Mirror: OnServerConnect " + conn.address);
        //InitCharacter(conn);
    }
    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);
        Debug.LogWarning("Mirror: OnServerReady " + conn.address);
        //InitCharacter(conn);
    }
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        //DestroyCharacter(conn);
    }

    private void OnQuitting()
    {
        if (NetworkServer.active || NetworkClient.active)
        {
            DebugPanel.Log("Mirror", "Shutting down", 5f);
            Debug.LogWarning("Mirror: Shutting down");
            NetworkManager.Shutdown();
        }
    }

    private void DestroyCharacter(NetworkConnection conn)
    {
        Debug.LogWarning("Mirror: Destroying player objects for " + conn.address);
        NetworkServer.DestroyPlayerForConnection(conn);
    }
    private void InitCharacter(NetworkConnection conn)
    {
        var currentMainCharacter = Instantiate(characterPrefabNetworked);
        NetworkServer.Spawn(currentMainCharacter, conn);

        if (theater != null)
            currentMainCharacter.GetComponent<ChairTraverser>().Teleport(theater.GetRandomChair());
    }

    public void StartServerInstance()
    {
        StartServer();
    }
    public void Host()
    {
        StartHost();
    }
    public void Join()
    {
        StartClient();
    }

    public GameObject GetLocalClientCharacterObject()
    {
        return localClientCharacter;
    }

    //Used for UI events
    public void CallPlayEventAsClient()
    {
        GetLocalClientCharacterObject().GetComponent<CharacterInput>().SendPlay();
    }
    //Used for UI events
    public void CallPauseEventAsClient()
    {
        GetLocalClientCharacterObject().GetComponent<CharacterInput>().SendPause();
    }
    //Used for UI events
    public void CallSeekEventAsClient(float value)
    {
        long frame = (long)(value * (unityVideoPlayer.frameCount - 1));
        GetLocalClientCharacterObject().GetComponent<CharacterInput>().SendSeek(frame);
    }
    public void CallPlayEventAsServer()
    {
        if (NetworkServer.active)
            RaisePlaybackEvent(1); // Custom Event 1: Used as "PlayTogether" event
        else
            InvokePlay();
    }
    public void CallPauseEventAsServer()
    {
        if (NetworkServer.active)
            RaisePlaybackEvent(2); // Custom Event 2: Used as "PauseTogether" event
        else
            InvokePause();
    }
    public void CallSeekEventAsServer(long frame)
    {
        if (NetworkServer.active)
            RaiseSeekEvent(frame);
        else
            InvokeSeek(frame);
    }

    private void RaisePlaybackEvent(byte eventCode)
    {
        if (NetworkServer.active)
        {
            var playbackEvent = new PlaybackEvent() { eventCode = eventCode };
            NetworkServer.SendToAll(playbackEvent);
            OnPlaybackEvent(playbackEvent);
        }
        else
        {
            Debug.LogError("Mirror: Cannot send event if not server");
        }
    }
    private void RaiseSeekEvent(long frame)
    {
        if (NetworkServer.active)
        {
            var seekEvent = new SeekEvent() { frame = frame };
            NetworkServer.SendToAll(seekEvent);
            OnSeekEvent(seekEvent);
        }
        else
        {
            Debug.LogError("Mirror: Cannot send event if not server");
        }
    }

    public void OnPlaybackEvent(PlaybackEvent playbackEvent)
    {
        if (playbackEvent.eventCode == 1)
            InvokePlay();
        else if (playbackEvent.eventCode == 2)
            InvokePause();
    }
    public void OnSeekEvent(SeekEvent seekEvent)
    {
        InvokeSeek(seekEvent.frame);
    }

    private void InvokePlay()
    {
        //onPlay?.Invoke();
        unityVideoPlayer.Play();
    }
    private void InvokePause()
    {
        //onPause?.Invoke();
        unityVideoPlayer.Pause();
    }
    private void InvokeSeek(long frame)
    {
        //onScrub?.Invoke(frame);
        unityVideoPlayer.Seek(frame);
    }

    public class PlaybackEvent : MessageBase
    {
        public byte eventCode;
    }
    public class SeekEvent : MessageBase
    {
        public long frame;
    }
}
