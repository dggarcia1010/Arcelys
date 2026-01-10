using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }

    public void ChangeMusic(AudioClip newClip)
    {
        audioSource.clip = newClip;
        audioSource.Play();
    }
}
