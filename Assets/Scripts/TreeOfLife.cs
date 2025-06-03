using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SequencerNote))]
public class TreeOfLife : MonoBehaviour
{
    [SerializeField, Range(0f, 1f), Tooltip("Chance to spawn a fruit when the note is played")]
    float _fruitSpawnChance = 0.3f;

    private SequencerNote _sequencerNote;

    private void Awake()
    {
        _sequencerNote = GetComponent<SequencerNote>();
    }

    private void OnEnable()
    {
        _sequencerNote.OnNotePlayed += GenerateFruit;
    }

    private void OnDisable()
    {
        _sequencerNote.OnNotePlayed -= GenerateFruit;
    }

    void GenerateFruit()
    {
        var chance = Random.Range(0f, 1f);
        if (chance < _fruitSpawnChance) {
            var availableFruits = GetComponentsInChildren<Fruit>(true).Where(f => !f.gameObject.activeSelf).ToArray();

            if (availableFruits.Length > 0) {
                // Activate a random inactive fruit
                var fruit = availableFruits[Random.Range(0, availableFruits.Length)];
                fruit.gameObject.SetActive(true);
            }
        }
    }
}
