using System.Xml.Serialization;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource SFX_Music;

    [SerializeField] private AudioClip explosion_Sound;
    [SerializeField] private AudioClip slash_Sound;
    [SerializeField] private AudioClip click_Sound;

    [SerializeField] private GameObject turnOn;
    [SerializeField] private GameObject turnOff;
    private bool isAudioOn = true;
    public AudioSource AudioSource => audioSource;



    public static AudioManager Instance;
    private void Awake()
    {
        if(Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void SlashPlaySound()
    {
        Debug.Log("Playing slash sound");
        audioSource.clip = slash_Sound;
        audioSource.Play();
    }
    public void ExplosionPlaySound()
    {
        Debug.Log("Playing explosion sound");
        audioSource.PlayOneShot(explosion_Sound);
    }
    public void ClickPlaySound()
    {
        audioSource.PlayOneShot(click_Sound);
    }
    public void ToggleAudio()
    {
        ClickPlaySound();
        if (isAudioOn)
        {
            isAudioOn = false;
            audioSource.mute = true;
            SFX_Music.mute = true;

            turnOn.SetActive(false);
            turnOff.SetActive(true);
        }
        else
        {
            isAudioOn = true;
            audioSource.mute = false;
            SFX_Music.mute = false;

            turnOn.SetActive(true);
            turnOff.SetActive(false);
        }
    }
}
