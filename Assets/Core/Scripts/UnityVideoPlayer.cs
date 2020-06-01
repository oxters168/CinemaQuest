using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class UnityVideoPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    public bool isPreparing { get; private set; }
    public bool isPrepared { get { return videoPlayer.isPrepared; } }
    public uint width { get { return videoPlayer.width; } }
    public uint height { get { return videoPlayer.height; } }
    public ulong frameCount { get { return videoPlayer.frameCount; } }
    private VideoPlayer videoPlayer;
    public RenderTexture renderTexture { get; private set; }

    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.SetTargetAudioSource(0, audioSource);

        videoPlayer.playOnAwake = false;
        videoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.RenderTexture;
        videoPlayer.prepareCompleted += PrepareCompleted;
    }

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
    public void Play()
    {
        videoPlayer.Play();
    }
    public void Pause()
    {
        videoPlayer.Pause();
    }
    public void Seek(long value)
    {
        if (value >= (long)videoPlayer.frameCount)
            value = (long)videoPlayer.frameCount - 1;

        videoPlayer.frame = value;
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
