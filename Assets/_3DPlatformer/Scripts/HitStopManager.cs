using System.Collections;
using UnityEngine;

public class HitStopManager : MonoBehaviour
{
    public static HitStopManager Instance;

    public float hitStopDuration;
    private float timeSpeed;
    [SerializeField] private float pendingStopDuration;
    [SerializeField] private bool isFrozen;

    private void Start()
    {
        if (Instance == null)
            Instance = this;
        isFrozen = false;
    }

    private void Update()
    {
        if (pendingStopDuration != 0 && !isFrozen)
            StartCoroutine(HitStopTimer());
    }

    public void DoHitStop(float duration, float speed)
    {
        hitStopDuration = duration;
        timeSpeed = speed;
        pendingStopDuration = hitStopDuration;
    }

    IEnumerator HitStopTimer()
    {
        isFrozen = true;
        var _original = Time.timeScale;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(hitStopDuration);
        Time.timeScale = _original;
        pendingStopDuration = 0;
        isFrozen = false;
    }
}
