using UnityEngine;
using UnityHelpers;
using Mirror;

public class NetworkingCalls : NetworkManager
{
    public static NetworkingCalls networkingCallsInScene { get; private set; }

    [Space(10)]
    public GameObject characterPrefab;
    public GameObject characterPrefabNetworked;
    private GameObject currentMainCharacter;
    private bool currentlyNetworked;
    public OVRRigInfo ovrRig;
    public Theater theater;
    //public string uri;
    public bool tempSpawn = true;

    [Space(10)]
    public UnityEngine.Events.UnityEvent onPlay;
    public UnityEngine.Events.UnityEvent onPause;

    public override void Awake()
    {
        base.Awake();
        Application.quitting += OnQuitting;
        networkingCallsInScene = this;
    }
    public override void Start()
    {
        base.Start();
        if (tempSpawn)
            InitCharacter(false);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        DebugPanel.Log("Mirror", "OnStartClient", 5f);
        Debug.LogWarning("Mirror: OnStartClient");
        DestroyCharacter();
        //InitCharacter(true);
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        DebugPanel.Log("Mirror", "OnStartServer", 5f);
        Debug.LogWarning("Mirror: OnStartServer");
        //InitCharacter(true);
    }
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        //Happens on client side when connecting to the server
        DebugPanel.Log("Mirror", "OnClientConnect " + conn.address, 5f);
        Debug.LogWarning("Mirror: OnClientConnect " + conn.address);
    }
    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        //Happens on server side when client connects to the server
        DebugPanel.Log("Mirror", "OnServerConnect " + conn.address, 5f);
        Debug.LogWarning("Mirror: OnServerConnect " + conn.address);
        //InitCharacter(true, conn);
    }
    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);
        DebugPanel.Log("Mirror", "OnServerReady " + conn.address, 5f);
        Debug.LogWarning("Mirror: OnServerReady " + conn.address);
        //InitCharacter(true, conn);
        //JoinRoom();
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

    private void DestroyCharacter()
    {
        if (currentMainCharacter != null)
            Destroy(currentMainCharacter);
    }
    private void InitCharacter(bool networked, NetworkConnection conn = null)
    {
        string debugMessage = "Instantiating " + (networked ? "" : "non-") + "networked character";
        DebugPanel.Log("PUN", debugMessage, 5);
        Debug.LogWarning(debugMessage);

        DestroyCharacter();

        if (networked)
        {
            currentMainCharacter = Instantiate(characterPrefabNetworked);
            NetworkServer.Spawn(currentMainCharacter, conn);
            //currentMainCharacter = PhotonNetwork.Instantiate(characterPrefabNetworked.name, Vector3.zero, Quaternion.identity);
            currentlyNetworked = true;
        }
        else
        {
            currentMainCharacter = Instantiate(characterPrefab);
            currentlyNetworked = false;
        }

        if (theater != null)
            currentMainCharacter.GetComponent<ChairTraverser>().Teleport(theater.GetRandomChair());
        if (ovrRig != null)
            currentMainCharacter.GetComponent<CharacterInput>().ovrRig = ovrRig;
    }

    public void ConnectToPun()
    {
        StartServer();
        //PhotonNetwork.ConnectUsingSettings();
    }
    public void CreateRoom()
    {
        StartHost();
        //StartServer();
        //NetworkServer.Listen(4);
    }
    public void JoinRoom()
    {
        NetworkClient.RegisterHandler<NetworkEvent>(OnEvent); //Will probably be better off in one of the start/stop functions
        StartClient();
        //NetworkClient.Connect(uri);
    }

    public void CallPlayEvent()
    {
        if (NetworkServer.active || NetworkClient.active)
            RaiseNetworkedEvent(1); // Custom Event 1: Used as "PlayTogether" event
        else
            InvokePlay();
    }
    public void CallPauseEvent()
    {
        if (NetworkServer.active || NetworkClient.active)
            RaiseNetworkedEvent(2); // Custom Event 2: Used as "PauseTogether" event
        else
            InvokePause();
    }

    private void RaiseNetworkedEvent(byte eventCode)
    {
        if (NetworkServer.active)
        {
            var networkEvent = new NetworkEvent() { eventCode = eventCode };
            NetworkServer.SendToAll(networkEvent);
            OnEvent(networkEvent);
        }
        else
        {
            DebugPanel.Log("Mirror", "Cannot send event if not server", 5f);
            Debug.LogWarning("Mirror: Cannot send event if not server");
        }
    }

    public void OnEvent(NetworkEvent networkEvent)
    {
        if (networkEvent.eventCode == 1)
            InvokePlay();
        else if (networkEvent.eventCode == 2)
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

    public class NetworkEvent : MessageBase
    {
        public byte eventCode;
    }
}
