using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SequencerNote))]
public class BumpOnBeat : MonoBehaviour
{
    [SerializeField] float _bumpScale = 1.1f;
    [SerializeField] float _bumpDuration = 0.2f;

    private SequencerNote _sequencerNote;

    private void Awake()
    {
        _sequencerNote = GetComponent<SequencerNote>();
    }

    private void OnEnable()
    {
        _sequencerNote.OnNotePlayed += AnimateBump;
    }

    private void OnDisable()
    {
        _sequencerNote.OnNotePlayed -= AnimateBump;
    }

    void AnimateBump()
    {
        // Start the bump animation coroutine
        StartCoroutine(BumpCoroutine());
    }

    private IEnumerator BumpCoroutine()
    {
        Vector3 originalScale = transform.localScale;

        // Apply a gamma curve to make the bump less sensitive to lower volumes
        float gamma = 0.4f; // Lower gamma means less reduction for low volumes
        float adjustedVolume = Mathf.Pow(_sequencerNote.Volume, gamma);
        var _scaledBumpScale = Mathf.Lerp(1f, _bumpScale, adjustedVolume);

        float elapsed = 0f;

        // Scale up
        while (elapsed < _bumpDuration / 2f) {
            float t = elapsed / (_bumpDuration / 2f);
            transform.localScale = Vector3.Lerp(originalScale, originalScale * _scaledBumpScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalScale * _scaledBumpScale;

        // Scale back down
        elapsed = 0f;
        while (elapsed < _bumpDuration / 2f) {
            float t = elapsed / (_bumpDuration / 2f);
            transform.localScale = Vector3.Lerp(originalScale * _scaledBumpScale, originalScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalScale;
    }
}
