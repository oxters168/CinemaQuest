using UnityEngine;

public class RunServer : MonoBehaviour
{
    void Start()
    {
        GetComponent<NetworkingCalls>().ConnectToPun();
    }
}
