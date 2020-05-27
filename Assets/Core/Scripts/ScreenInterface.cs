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
        isShowing = unityVideoPlayer != null && !unityVideoPlayer.isPreparing && unityVideoPlayer.isPrepared;
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
            float width;
            float height;

            width = background.transform.localScale.y * ratio;
            if (width > background.transform.localScale.x)
            {
                width = background.transform.localScale.x;
                height = background.transform.localScale.x / ratio;
            }
            else
                height = background.transform.localScale.y;
            
            content.transform.localScale = new Vector3(width, height, background.transform.localScale.z);
        }
        else
            shown = false;
    }
}
