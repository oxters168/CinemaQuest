using UnityEngine;

public class ScreenInterface : MonoBehaviour
{
    public UnityVideoPlayer unityVideoPlayer;
    public Renderer content;

    [Space(10), Tooltip("The shaper of the content, defines the size for ratios and stuff")]
    public Transform background;

    public bool isShowing { get; private set; }
    private bool shown;

    void Update()
    {
        isShowing = !unityVideoPlayer.isPreparing && unityVideoPlayer.isPrepared;
        content.gameObject.SetActive(isShowing);
        if (isShowing)
        {
            if (!shown)
            {
                content.material.mainTexture = unityVideoPlayer.renderTexture;
                shown = true;
            }
            
            content.transform.rotation = background.rotation;

            float ratio = (float)unityVideoPlayer.width / unityVideoPlayer.height;
            content.transform.localScale = new Vector3(background.transform.localScale.y * ratio, background.transform.localScale.y, background.transform.localScale.z);
        }
        else
            shown = false;
    }
}
