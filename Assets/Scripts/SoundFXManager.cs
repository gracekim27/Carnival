using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance { get; private set; }

    [SerializeField] private AudioSource audioSource; // Attach an AudioSource component in the Inspector

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public AudioClip ChooseSoundFromList(AudioClip[] list)
    {
        if (list == null || list.Length == 0)
        {
            Debug.LogError("AudioClip list is empty or null");
            return null;
        }
        int index = Random.Range(0, list.Length);
        return list[index];
    }

    // Play a sound with adjustable volume and pitch
    public void PlaySound(AudioClip clip, float volume = 1.0f, float pitch = 1.0f)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.pitch = pitch; // Set the pitch
            audioSource.PlayOneShot(clip, volume); // Play the clip with the specified volume
        }
    }
}