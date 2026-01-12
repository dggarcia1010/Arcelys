using UnityEngine;

public class CrystalProgress : MonoBehaviour
{
    public static CrystalProgress Instance;

    public bool crystal1Unlocked;
    public bool crystal2Unlocked;
    public bool crystal3Unlocked;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void UnlockCrystal1() => crystal1Unlocked = true;
    public void UnlockCrystal2() => crystal2Unlocked = true;
    public void UnlockCrystal3() => crystal3Unlocked = true;
}