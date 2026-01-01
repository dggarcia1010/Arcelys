using UnityEngine;

public class SceneMusicByEnemies : MonoBehaviour
{
    public AudioClip calmMusic;
    public AudioClip tensionMusic;

    AudioSource a;
    bool inTension;

    void Awake()
    {
        a = GetComponent<AudioSource>();
        a.playOnAwake = false;
        a.loop = true;
    }

    void Start()
    {
        UpdateMusic(); // al empezar, decide cuál poner
    }

    void Update()
    {
        UpdateMusic(); // para esta escena va perfecto
    }

    void UpdateMusic()
    {
        int enemiesAlive = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (enemiesAlive > 0 && !inTension)
        {
            inTension = true;
            SwitchTo(tensionMusic);
        }
        else if (enemiesAlive == 0 && inTension)
        {
            inTension = false;
            SwitchTo(calmMusic);
        }
    }

    void SwitchTo(AudioClip clip)
    {
        if (clip == null) return;
        if (a.clip == clip) return;

        a.clip = clip;
        a.Play();
    }
}
