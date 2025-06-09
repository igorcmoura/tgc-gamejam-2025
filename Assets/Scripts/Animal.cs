using System.Collections;
using UnityEngine;

public class Animal : MonoBehaviour
{
    private enum State { MovingToTarget, AwaitingFood, EatingFood, MovingAway }

    [Header("Movement Settings")]
    [SerializeField] float _jumpDistance = 1.0f;
    [SerializeField] float _jumpHeight = 0.5f;
    [SerializeField] float _jumpDuration = 0.3f;
    [SerializeField] int _waitForFoodBeats = 6;

    private State currentState = State.MovingToTarget;

    private Vector3 _target;
    private Vector3 _initialPosition;
    private float _rightDirectionScale;
    private int _remainingBeatsToWait;

    public void SetTarget(Fruit target)
    {
        _target = target.transform.position;
        currentState = State.MovingToTarget;
    }

    private void Start()
    {
        _initialPosition = transform.position;
        _rightDirectionScale = transform.localScale.x;
        EventsManager.OnBeat += OnBeat;
        EventsManager.OnFruitDropped += OnFruitDropped;
    }

    private void OnDestroy()
    {
        EventsManager.OnBeat -= OnBeat;
    }

    private void OnBeat(int _, int beat)
    {
        var middleBeat = Mathf.RoundToInt(BeatSequencer.Instance.BeastsPerSegment / 2);
        if (beat != 0 && beat != middleBeat) return; // Only respond to the first and middle beat of each segment

        switch (currentState) {
            case State.MovingToTarget:
                if (_target != null && Vector3.Distance(transform.position, TargetAtGround) > 0.1f) {
                    StartCoroutine(JumpTowards(TargetAtGround));
                } else {
                    _remainingBeatsToWait = _waitForFoodBeats;
                    currentState = State.AwaitingFood;
                }
                break;
            case State.AwaitingFood:
                if (_remainingBeatsToWait > 0) {
                    _remainingBeatsToWait--;
                } else {
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    currentState = State.MovingAway;
                }
                break;
            case State.MovingAway:
                if (Vector3.Distance(transform.position, _initialPosition) > 0.1f) {
                    StartCoroutine(JumpTowards(_initialPosition));
                } else {
                    Destroy(gameObject);
                }
                break;
        }
    }

    private void OnFruitDropped(Fruit fruit)
    {
        if (currentState == State.MovingAway) return; // Ignore if already moving away
        SetTarget(fruit);
    }

    private Vector3 TargetAtGround => new Vector3(_target.x, _initialPosition.y, _initialPosition.z);

    private IEnumerator JumpTowards(Vector3 destination)
    {
        Vector3 start = transform.position;
        Vector3 dir = (destination - start).normalized;
        Vector3 end = start + dir * Mathf.Min(_jumpDistance, Vector3.Distance(start, destination));
        float elapsed = 0f;

        if (dir.x < 0) {
            // Flip the animal to face left
            transform.localScale = new Vector3(-_rightDirectionScale, transform.localScale.y, transform.localScale.z);
        } else {
            // Keep the animal facing right
            transform.localScale = new Vector3(_rightDirectionScale, transform.localScale.y, transform.localScale.z);
        }

        while (elapsed < _jumpDuration) {
            float t = elapsed / _jumpDuration;
            float height = Mathf.Sin(Mathf.PI * t) * _jumpHeight;
            transform.position = Vector3.Lerp(start, end, t) + Vector3.up * height;
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = end;
    }
}
