using UnityEngine;
using Photon.Pun;
using UnityHelpers;

public class Theater : MonoBehaviourPunCallbacks
{
    public GameObject characterPrefab, characterPrefabNetworked;
    private GameObject currentMainCharacter;
    private bool currentlyNetworked;
    public OVRRigInfo ovrRig;

    public CinemaChair[] chairs { get; private set; }

    void Awake()
    {
        chairs = GetComponentsInChildren<CinemaChair>();
    }
    void Start()
    {
        InitCharacter(false);
    }

    public CinemaChair GetRandomChair()
    {
        return chairs[Random.Range(0, chairs.Length)];
    }
    private void InitCharacter(bool networked)
    {
        string debugMessage = "Instantiating " + (networked ? "" : "non-") + "networked character";
        DebugPanel.Log("PUN", debugMessage, 5);
        Debug.LogWarning(debugMessage);

        if (currentMainCharacter != null)
        {
            if (currentlyNetworked)
                PhotonNetwork.Destroy(currentMainCharacter);
            else
                Destroy(currentMainCharacter);
        }

        if (networked)
        {
            currentMainCharacter = PhotonNetwork.Instantiate(characterPrefabNetworked.name, Vector3.zero, Quaternion.identity);
            currentlyNetworked = true;
        }
        else
        {
            currentMainCharacter = Instantiate(characterPrefab);
            currentlyNetworked = false;
        }

        var chairTraverser = currentMainCharacter.GetComponent<ChairTraverser>();
        chairTraverser.pointer = ovrRig.rightController;
        chairTraverser.Teleport(GetRandomChair());
        currentMainCharacter.GetComponent<CharacterInput>().ovrRig = ovrRig;
    }

    public override void OnJoinedRoom()
    {
        InitCharacter(true);
    }
    public override void OnCreatedRoom()
    {
        
    }
}
