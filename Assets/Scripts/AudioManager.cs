using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : MonoBehaviour
{
    public static AudioManager INSTANCE;

    public AudioMixer master;

    private void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetMaster( float volume )
    {
        master.SetFloat("Volume_Master", convertToDb(volume));
    }

    public void SetSFX( float volume )
    {
        master.SetFloat("Volume_SFX", convertToDb(volume));
    }

    public void SetBGM( float volume )
    {
        master.SetFloat("Volume_BGM", convertToDb(volume));
    }

    private float convertToDb( float sliderValue )
    {
        return Mathf.Clamp(Mathf.Log10(sliderValue) * 20, -20, 80);
    }

}
