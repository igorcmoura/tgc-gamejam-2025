using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    [SerializeField] Transform[] _spawnPoints;
    [SerializeField] Animal _animalPrefab;

    private Animal _currentAnimal;

    private void Start()
    {
        _currentAnimal = null;
    }

    public void SpawnAnimal(Fruit target)
    {
        var closestSpawnPoint = _spawnPoints[0];
        for (int i = 1; i < _spawnPoints.Length; i++) {
            var currentSpawnPointDistance = Vector3.Distance(target.transform.position, _spawnPoints[i].position);
            var closestSpawnPointDistance = Vector3.Distance(target.transform.position, closestSpawnPoint.position);
            if (currentSpawnPointDistance < closestSpawnPointDistance) {
                closestSpawnPoint = _spawnPoints[i];
            }
        }

        SpawnAnimal(target, closestSpawnPoint.position);
    }

    private void SpawnAnimal(Fruit target, Vector3 position)
    {
        if (_currentAnimal != null) return; // Only one animal at a time

        var animal = Instantiate(_animalPrefab, position, Quaternion.identity);
        animal.SetTarget(target);
        _currentAnimal = animal;
    }
}
