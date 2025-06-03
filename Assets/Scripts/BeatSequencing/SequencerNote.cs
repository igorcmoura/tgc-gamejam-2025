using System.Collections;
using UnityEngine;

public class SequencerNote : MonoBehaviour
{
    [SerializeField] bool _enabled = false;
    [SerializeField] int _scaleDegree; // 0-11 for chromatic scale
    [Range(0f, 1f)]
    [SerializeField] float _volume = 1f;

    public System.Action OnNotePlayed;

    private AudioSource _audioSource;

    public bool IsEnabled
    {
        get => _enabled;
        set => _enabled = value;
    }

    public float Volume
    {
        get => _volume;
        set {
            _volume = Mathf.Clamp01(value);
            _audioSource.volume = _volume;
        }
    }

    public int ScaleDegree
    {
        get => _scaleDegree;
        set => _scaleDegree = value;
    }

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null) {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
        ScaleDegree = Random.Range(0, 12); // Random initial scale degree
        Volume = Random.Range(0, 1f); // Random initial volume
    }

    public void PlayScheduled(float pitch, AudioClip noteClip, double time)
    {
        _audioSource.pitch = pitch;
        _audioSource.volume = _volume;
        _audioSource.clip = noteClip;
        _audioSource.PlayScheduled(time);
        StartCoroutine(ScheduleOnNotePlayed(time - AudioSettings.dspTime));
    }

    IEnumerator ScheduleOnNotePlayed(double delay)
    {
        yield return new WaitForSeconds((float)delay);
        OnNotePlayed?.Invoke();
    }
}
