using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(SequencerNote))]
public class Sprout : MonoBehaviour
{
    [SerializeField] private Sprite[] _sprites;

    private SpriteRenderer _spriteRenderer;
    private SequencerNote _sequencerNote;

    private int _growthStage = 0;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _sequencerNote = GetComponent<SequencerNote>();
    }

    private void Start()
    {
        if (_sprites != null && _sprites.Length > 0) {
            _spriteRenderer.sprite = _sprites[0];
            _growthStage = 1;
        }
    }

    private void OnEnable()
    {
        _sequencerNote.OnNotePlayed += UpdateSprite;
    }

    private void OnDisable()
    {
        _sequencerNote.OnNotePlayed -= UpdateSprite;
    }

    void UpdateSprite()
    {
        if (_sprites == null || _sprites.Length == 0 || _growthStage >= _sprites.Length)
            return;

        _spriteRenderer.sprite = _sprites[_growthStage];
        _growthStage++;
    }
}
