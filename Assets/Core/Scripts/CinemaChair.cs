using UnityEngine;

public class CinemaChair : MonoBehaviour
{
    public Transform occupyTransform;
    public GameObject highlightedObject;
    public bool isHighlighted;
    public bool isOccupied { get; private set; }

    void Update()
    {
        highlightedObject.SetActive(isHighlighted);
    }

    public void Occupy()
    {
        isOccupied = true;
    }
    public void Unoccupy()
    {
        isOccupied = false;
    }
}
