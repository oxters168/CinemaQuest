using UnityEngine;
using UnityEngine.Video;

public class UnityVideoPlayer : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RenderTexture renderTexture;

    void Start()
    {
        // Play on awake defaults to true. Set it to false to avoid the url set
        // below to auto-start playback since we're in Start().
        videoPlayer.playOnAwake = false;

        // By default, VideoPlayers added to a camera will use the far plane.
        // Let's target the near plane instead.
        videoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = renderTexture;

        // This will cause our Scene to be visible through the video being played.
        //videoPlayer.targetCameraAlpha = 0.5F;

        // Set the video to play. URL supports local absolute or relative paths.
        // Here, using absolute.
        videoPlayer.url = "D:/Projects/Media/HandedHands/TheGreenSpheres.mp4";

        // Skip the first 100 frames.
        //videoPlayer.frame = 100;

        // Restart from beginning when done.
        //videoPlayer.isLooping = true;

        // Each time we reach the end, we slow down the playback by a factor of 10.
        //videoPlayer.loopPointReached += EndReached;

        // Start playback. This means the VideoPlayer may have to prepare (reserve
        // resources, pre-load a few frames, etc.). To better control the delays
        // associated with this preparation one can use videoPlayer.Prepare() along with
        // its prepareCompleted event.
        videoPlayer.Play();
    }

    void Update()
    {
        
    }
}
