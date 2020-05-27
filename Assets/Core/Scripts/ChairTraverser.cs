using UnityEngine;

public class ChairTraverser : MonoBehaviour
{
    public bool highlightChairs;
    public bool teleport;
    private bool teleported;
    public Transform pointer;
    private CinemaChair currentlyPointedAt;

    void Update()
    {
        if (teleport && !teleported)
        {
            Teleport(currentlyPointedAt);
            teleported = true;
        }
        else if (!teleport)
            teleported = false;

        if (highlightChairs)
            Highlight();
        else
            SetPointedAt(null);
    }

    public void Teleport(CinemaChair chair)
    {
        if (chair != null)
        {
            transform.position = chair.occupyTransform.position;
            transform.rotation = chair.occupyTransform.rotation;
        }
    }
    private void Highlight()
    {
        CinemaChair chair = null;
        
        RaycastHit raycastHit;
        if (Physics.Raycast(pointer.position, pointer.forward, out raycastHit, 10, LayerMask.GetMask("Chair"), QueryTriggerInteraction.Collide))
        {
            chair = raycastHit.transform.GetComponent<CinemaChair>();
        }
        SetPointedAt(chair);
    }
    private void SetPointedAt(CinemaChair chair)
    {
        if (currentlyPointedAt != null)
            currentlyPointedAt.isHighlighted = false;
        
        currentlyPointedAt = chair;

        if (currentlyPointedAt != null)
            currentlyPointedAt.isHighlighted = true;
    }
}
