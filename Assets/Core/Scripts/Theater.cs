using UnityEngine;

public class Theater : MonoBehaviour
{
    public CinemaChair[] chairs { get; private set; }

    void Awake()
    {
        chairs = GetComponentsInChildren<CinemaChair>();
    }

    public CinemaChair GetRandomChair()
    {
        return chairs[Random.Range(0, chairs.Length)];
    }
}
