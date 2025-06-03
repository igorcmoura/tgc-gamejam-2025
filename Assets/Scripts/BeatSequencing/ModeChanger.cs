using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ModeChanger : MonoBehaviour
{
    [SerializeField] Color[] _colors;

    private SpriteRenderer _spriteRenderer;

    private int _currentColorIndex = 0;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_colors.Length > 0) {
            _spriteRenderer.color = _colors[_currentColorIndex];
        }
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0)) {
            BeatSequencer.Instance.NextMode();
            _currentColorIndex = (_currentColorIndex + 1) % _colors.Length;
            _spriteRenderer.color = _colors[_currentColorIndex];
        }
    }
}
