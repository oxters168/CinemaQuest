using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class LoadSceneOnSplashEnd : MonoBehaviour
{
    public VideoPlayer splashPlayer;
    
    void Update()
    {
        if (splashPlayer.frame > 0 && !splashPlayer.isPlaying)
        {
            SceneManager.LoadScene(1);
        }
    }
}
