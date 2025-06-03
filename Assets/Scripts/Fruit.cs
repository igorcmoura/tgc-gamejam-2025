using System.Collections;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    [Header("Pop Animation Settings")]
    [SerializeField] float _popDuration = 0.3f;
    [SerializeField] float _popOvershoot = 1.2f;
    [SerializeField] AnimationCurve _popCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Falling Settings")]
    [SerializeField] float _groundY = -3.5f;
    [SerializeField] float _gravity = 9.81f;

    private Vector3 _defaultPosition;
    private Vector3 _normalScale;

    private bool _isFalling = false;

    private void Awake()
    {
        _defaultPosition = transform.position;
        _normalScale = transform.localScale;
    }

    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(PopCoroutine());
    }

    private void OnMouseDown()
    {
        if (!_isFalling && Input.GetMouseButtonDown(0)) {
            Drop();
        }
    }

    private IEnumerator PopCoroutine()
    {
        float elapsed = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 overshootScale = _normalScale * _popOvershoot;

        // Scale up to overshoot
        while (elapsed < _popDuration * 0.6f) {
            float t = elapsed / (_popDuration * 0.6f);
            float curveT = _popCurve.Evaluate(t);
            transform.localScale = Vector3.LerpUnclamped(startScale, overshootScale, curveT);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Scale back to normal
        float backDuration = _popDuration * 0.4f;
        elapsed = 0f;
        Vector3 currentScale = transform.localScale;
        while (elapsed < backDuration) {
            float t = elapsed / backDuration;
            float curveT = _popCurve.Evaluate(t);
            transform.localScale = Vector3.LerpUnclamped(currentScale, _normalScale, curveT);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = _normalScale;
    }

    void Drop()
    {
        _isFalling = true;
        StartCoroutine(FallCoroutine());
    }

    private IEnumerator FallCoroutine()
    {
        var speed = 0f;
        while (transform.position.y > _groundY) {
            speed += _gravity * Time.deltaTime;
            transform.position += speed * Time.deltaTime * Vector3.down;
            yield return null;
        }

        var sprout = TreesManager.Instance.UnlockNextTree();
        sprout.transform.position = transform.position;
        sprout.gameObject.SetActive(true);

        Destroy(gameObject);
    }

    public void GetEaten()
    {
        // Reset the state of the fruit
        _isFalling = false;
        transform.position = _defaultPosition;
        gameObject.SetActive(false);
    }
}
