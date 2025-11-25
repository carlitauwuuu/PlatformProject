using UnityEngine;
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set;}

    public AudioSource musicSource;
    [SerializeField] AudioSource playerSound;
    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
        
        }
        else
        {
            Debug.LogError("There is two or more Sound manager");
        }
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource.clip == clip) return; 

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }
    public void PlayerSound(AudioClip audioClip) 
    {
        playerSound.clip = audioClip;
        playerSound.Play();
    }
}
