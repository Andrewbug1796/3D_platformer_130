using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField] private MusicTrack musicTrack;
    public AudioSource musicSource;
    [SerializeField] private float musicVolume;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;        
    }

    public void PlayMusic(string trackname, float faceDuration)
    {
        StartCoroutine(AnimateMusicCrossFade(musicTrack.GetClipFromName(trackname), faceDuration));
    }

    IEnumerator AnimateMusicCrossFade( AudioClip nexTrack, float faceDuration)
    {
        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / faceDuration;
            musicSource.volume = Mathf.Lerp(musicVolume, 0f, percent);
            yield return null;
        }

        musicSource.clip = nexTrack;
        musicSource.Play();
        percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / faceDuration;
            musicSource.volume = Mathf.Lerp(0, musicVolume, percent);
            yield return null;
        }
    }

    public void StopMusic()
    {
        StartCoroutine(StopMusicCrossFace(0.5f));
    }

    IEnumerator StopMusicCrossFace(float faceDuration)
    {
        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / faceDuration;
            musicSource.volume = Mathf.Lerp(musicVolume, 0, percent);
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = musicVolume;
    }
}
