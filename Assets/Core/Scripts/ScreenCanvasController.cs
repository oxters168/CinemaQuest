using UnityEngine;

public class ScreenCanvasController : MonoBehaviour
{
    public GameObject loadingText;
    public UnityVideoPlayer unityVideoPlayer;

    void Update()
    {
        loadingText.SetActive(unityVideoPlayer != null && unityVideoPlayer.isPreparing);
    }
}
