using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class UnityVideoPlayer : MonoBehaviour
{
    //public Renderer[] renderersToPlayOn;
    public bool isPreparing { get; private set; }
    public bool isPrepared { get { return videoPlayer.isPrepared; } }
    public uint width { get { return videoPlayer.width; } }
    public uint height { get { return videoPlayer.height; } }
    private VideoPlayer videoPlayer;
    public RenderTexture renderTexture { get; private set; }

    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        videoPlayer.playOnAwake = false;
        videoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.RenderTexture;
        videoPlayer.prepareCompleted += PrepareCompleted;
    }
    //void Start()
    //{
    //    PlayVideo("D:/Projects/Media/HandedHands/TrackedForceFirstTest.mp4");
    //}

    public void PrepareVideo(string url)
    {
        if (renderTexture != null)
            Destroy(renderTexture);
        
        isPreparing = true;
        videoPlayer.url = url;
        videoPlayer.Prepare();
    }
    public void PlayVideo(string url)
    {
        if (renderTexture != null)
            Destroy(renderTexture);
        
        isPreparing = true;
        videoPlayer.url = url;
        videoPlayer.Play();
    }

    private void PrepareCompleted(VideoPlayer source)
    {
        if (source.isPrepared)
        {        
            renderTexture = new RenderTexture((int)source.width, (int)source.height, 16);
            videoPlayer.targetTexture = renderTexture;
        }

        isPreparing = false;
    }
}
