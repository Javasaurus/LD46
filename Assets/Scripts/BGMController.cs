using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BGMController : MonoBehaviour
{

    public AudioClip gameOverMusic;

    private AudioSource bgm;

    // Start is called before the first frame update
    private void Start()
    {
        bgm = GetComponent<AudioSource>();
    }

    public void SetGameOver()
    {
        bgm.Stop();
        bgm.PlayOneShot(gameOverMusic);
    }


}
