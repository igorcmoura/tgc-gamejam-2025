using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SequencerNote))]
public class TreeOfLife : MonoBehaviour
{
    private SequencerNote _sequencerNote;

    private void Awake()
    {
        _sequencerNote = GetComponent<SequencerNote>();
    }

    public Fruit SpawnFruit()
    {
        var availableFruits = GetComponentsInChildren<Fruit>(true).Where(f => !f.gameObject.activeSelf).ToArray();

        if (availableFruits.Length > 0) {
            // Activate a random inactive fruit
            var fruit = availableFruits[Random.Range(0, availableFruits.Length)];
            fruit.gameObject.SetActive(true);
            EventsManager.OnFruitSpawned?.Invoke(fruit);
            return fruit;
        }
        return null;
    }

    void TryGenerateAnimal(Transform targetFruit)
    {
        //var chance = Random.Range(0f, 1f);
        //if (chance < _animalSpawnChance) {
        //    var availableAnimals = GetComponentsInChildren<AnimalBehaviour>(true).Where(a => !a.gameObject.activeSelf).ToArray();
        //    if (availableAnimals.Length > 0) {
        //        // Activate a random inactive animal
        //        var animal = availableAnimals[Random.Range(0, availableAnimals.Length)];
        //        animal.gameObject.SetActive(true);
        //        animal._target = targetFruit;
        //    }
        //}
    }
}
