using UnityEngine;

public abstract class GamePhase
{
    public virtual void StartPhase() { }
    public virtual void OnUpdate() { }
    public virtual void OnBeat(int segment, int beat) { }
    public virtual void EndPhase() { }
}

public class FruitDropTutorialPhase : GamePhase
{
    private const int SegmentToSpawnFruit = 2;
    private const int SegmentToAddAttention = 5;

    private int _segmentOffset;
    private Fruit _spawnedFruit;

    public override void StartPhase()
    {
        _segmentOffset = BeatSequencer.Instance.CurrentSegment;
    }

    public override void OnBeat(int segment, int beat)
    {
        if (segment == _segmentOffset + SegmentToSpawnFruit && beat == 0) {
            _spawnedFruit = GameController.Instance.TreeOfLife.SpawnFruit();
            EventsManager.OnFruitDropped += OnFruitDropped;
        }
        if (segment == _segmentOffset + SegmentToAddAttention && _spawnedFruit != null) {
            Debug.Log("Attention to the fruit: " + _spawnedFruit.name);
            // TODO: Add some attention to the fruit if the player doesn't interact with it
        }
    }

    void OnFruitDropped(Fruit fruit)
    {
        if (_spawnedFruit == fruit) {
            GameController.Instance.NextPhase();
            EventsManager.OnFruitDropped -= OnFruitDropped;
        }
    }
}

public class AnimalFeedingTutorialPhase : GamePhase
{
    private const int SegmentToSpawnFruit = 3;

    private int _segmentOffset;

    public override void StartPhase()
    {
        _segmentOffset = BeatSequencer.Instance.CurrentSegment;
    }

    public override void OnBeat(int segment, int beat)
    {
        if (segment == _segmentOffset + SegmentToSpawnFruit && beat == 0) {
            GameController.Instance.TreeOfLife.SpawnFruit();
            EventsManager.OnFruitDropped += OnFruitDropped;
        }
    }

    void OnFruitDropped(Fruit fruit)
    {
        GameController.Instance.AnimalSpawner.SpawnAnimal(fruit);
        EventsManager.OnFruitDropped -= OnFruitDropped;
    }
}

public class NormalPhase : GamePhase
{
    public override void OnBeat(int segment, int beat)
    {
        if (beat == 0) {
            TrySpawnFruit();
        }
    }

    void TrySpawnFruit()
    {
        var chance = Random.Range(0f, 1f);
        if (chance < GameController.Instance.FruitSpawnChance) {
            var fruit = GameController.Instance.TreeOfLife.SpawnFruit();
            if (fruit != null) { TrySpawnAnimal(fruit); }
        }
    }

    void TrySpawnAnimal(Fruit target)
    {
        var chance = Random.Range(0f, 1f);
        if (chance < GameController.Instance.AnimalSpawnChance) {
            GameController.Instance.AnimalSpawner.SpawnAnimal(target);
        }
    }
}

public class GameController : MonoBehaviour
{
    private static GameController _instance;
    public static GameController Instance => _instance;

    [SerializeField] BeatSequencer _beatSequencer;
    [SerializeField] TreeOfLife _treeOfLife;
    [SerializeField] AnimalSpawner _animalSpawner;

    [SerializeField, Range(0f, 1f), Tooltip("Chance to spawn a fruit when the note is played")]
    float _fruitSpawnChance = 0.3f;

    [SerializeField, Range(0f, 1f), Tooltip("Chance to spawn an animal at the same time a fruit spawns")]
    float _animalSpawnChance = 0.5f;

    private GamePhase[] _phases;
    private int _currentPhaseIndex;

    public TreeOfLife TreeOfLife => _treeOfLife;
    public AnimalSpawner AnimalSpawner => _animalSpawner;
    public float FruitSpawnChance => _fruitSpawnChance;
    public float AnimalSpawnChance => _animalSpawnChance;

    private void Awake()
    {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _phases = new GamePhase[]
        {
            //new FruitDropTutorialPhase(),
            //new AnimalFeedingTutorialPhase(),
            new NormalPhase(),
        };
        _currentPhaseIndex = 0;
        _phases[_currentPhaseIndex].StartPhase();
    }

    private void OnEnable()
    {
        EventsManager.OnBeat += OnBeat;
    }

    private void OnDisable()
    {
        EventsManager.OnBeat -= OnBeat;
    }

    public void NextPhase()
    {
        if (_currentPhaseIndex < _phases.Length - 1) {
            _phases[_currentPhaseIndex].EndPhase();
            _currentPhaseIndex++;
            _phases[_currentPhaseIndex].StartPhase();
        } else {
            Debug.Log("No more phases available.");
        }
    }

    private void OnBeat(int segment, int beat)
    {
        _phases[_currentPhaseIndex].OnBeat(segment, beat);

        //if (_state == GameState.FruitDropTutorial) {
        //    if (segment == 3) {
        //        _treeOfLife.SpawnFruit();
        //    }
        //} else if (_state == GameState.AnimalFeedingTutorial) {
        //    // Lógica para o tutorial de alimentação de animais
        //} else if (_state == GameState.GameActive) {
        //    // Lógica do jogo ativo
        //}
    }
}
