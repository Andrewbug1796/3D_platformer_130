using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource sfxSource;

    [Header("Sound Effects")]
    public AudioClip swordSwing;
    public AudioClip coinCollect;
    public AudioClip footstep;
    public AudioClip enemyHit;

    [Header("Volumes")]
    [Range(0, 1)] public float swordVolume = 0.8f;
    [Range(0, 1)] public float coinVolume = 0.8f;
    [Range(0, 1)] public float footstepVolume = 0.4f;
    [Range(0, 1)] public float enemyHitVolume = 1f;

    private void Awake()
    {
        Instance = this;
    }

    public void PlaySwordSwing() { Play(swordSwing, swordVolume); }
    public void PlayCoin() { Play(coinCollect, coinVolume); }
    public void PlayFootstep() { Play(footstep, footstepVolume); }
    public void PlayEnemyHit() { Play(enemyHit, enemyHitVolume); }

    void Play(AudioClip clip, float volume)
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip, volume);
    }
}