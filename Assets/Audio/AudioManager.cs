using System.Xml.Serialization;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
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

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void ToggleAudio()
    {
        if(isAudioOn)
        {
            isAudioOn = false;
            audioSource.mute = true;
            turnOn.SetActive(false);
            turnOff.SetActive(true);
        }
        else
        {
            isAudioOn = true;
            audioSource.mute = false;
            turnOn.SetActive(true);
            turnOff.SetActive(false);
        }
    }
}
