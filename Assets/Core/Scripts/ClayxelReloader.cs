using UnityEngine;

public class ClayxelReloader : MonoBehaviour
{
    public Clayxels.ClayContainer clayContainer;

    void Start()
    {
        clayContainer.init();
    }
    void Update()
    {
        clayContainer.forceUpdateAllChunks();
    }
}
