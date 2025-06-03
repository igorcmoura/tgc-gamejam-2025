using System;
using UnityEngine;

public enum ScaleMode
{
    Ionian,      // Major
    Dorian,
    Phrygian,
    Lydian,
    Mixolydian,
    Aeolian,     // Natural Minor
    Locrian
}

public class BeatSequencer : MonoBehaviour
{
    private static BeatSequencer _instance;

    public static BeatSequencer Instance => _instance != null ? _instance : throw new InvalidOperationException("BeatSequencer instance is not initialized.");

    [SerializeField] NoteSettings _noteSettings;
    [SerializeField] float _bpm = 120f;
    [SerializeField] int _beatsPerSegment = 16;
    [SerializeField] SequencerNote[] _sequence;

    [Header("Scale Settings")]
    [SerializeField] ScaleMode _mode = ScaleMode.Ionian;
    [SerializeField] int _baseNoteMidi = 57; // A3 = MIDI 57
    [SerializeField] bool _randomizeModeChange = false;

    private int _currentStep = 0;
    private double _nextEventTime;
    private int _segmentsUntilModeChange = 0; // Steps until the next mode change, if randomization is enabled

    // Intervals for major and minor scales in semitones
    private static readonly int[] Ionian = { 0, 2, 4, 5, 7, 9, 11 };
    private static readonly int[] Dorian = { 0, 2, 3, 5, 7, 9, 10 };
    private static readonly int[] Phrygian = { 0, 1, 3, 5, 7, 8, 10 };
    private static readonly int[] Lydian = { 0, 2, 4, 6, 7, 9, 11 };
    private static readonly int[] Mixolydian = { 0, 2, 4, 5, 7, 9, 10 };
    private static readonly int[] Aeolian = { 0, 2, 3, 5, 7, 8, 10 };
    private static readonly int[] Locrian = { 0, 1, 3, 5, 6, 8, 10 };

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
        _nextEventTime = AudioSettings.dspTime + 2f; // Start 2 seconds in the future
        _segmentsUntilModeChange = 4; // Default to 4 segments before changing mode
    }

    private void Update()
    {
        var time = AudioSettings.dspTime;

        if (time + 1f > _nextEventTime) {
            if (_currentStep == 0 && _randomizeModeChange) {
                Debug.Log("BEAT");
                if (_segmentsUntilModeChange <= 0) {
                    // Randomly change mode after a certain number of steps
                    _mode = (ScaleMode)UnityEngine.Random.Range(0, Enum.GetValues(typeof(ScaleMode)).Length);
                    _segmentsUntilModeChange = UnityEngine.Random.Range(0, 3) * 2; // Randomize next change
                } else {
                    _segmentsUntilModeChange--;
                }
            }

            PlayCurrentStep();
            _currentStep = (_currentStep + 1) % _beatsPerSegment;
            _nextEventTime += 60f / _bpm;
        }
    }

    private void PlayCurrentStep()
    {
        if (_currentStep >= _sequence.Length) {
            return; // Prevent out of bounds access
        }

        var step = _sequence[_currentStep];
        if (step.IsEnabled && _noteSettings != null) {
            float pitch = GetPitchForScaleDegree(step.ScaleDegree);
            double scheduleTime = GetScheduleTime(pitch);
            step.PlayScheduled(pitch, _noteSettings.NoteClip, scheduleTime);
        }
    }

    private float GetPitchForScaleDegree(int degree)
    {
        int[] scale = GetScaleIntervals(_mode);
        int scaleLength = scale.Length;

        int octave = Mathf.FloorToInt((float)degree / scaleLength);
        int scaleIndex = degree % scaleLength;

        int semitoneOffset = scale[scaleIndex] + (octave * 12);

        // Calculate the difference between the base note and the AudioClip's note (A, MIDI 57)
        int baseNoteOffset = _baseNoteMidi - _noteSettings.BaseMidiNote;

        // Total semitone offset from the AudioClip's original pitch
        int totalSemitoneOffset = semitoneOffset + baseNoteOffset;

        return Mathf.Pow(2f, totalSemitoneOffset / 12f); // Convert to pitch factor
    }

    double GetScheduleTime(float pitch)
    {
        // Calculate start delay based on pitch to account for tempo changes
        var startDelay = _noteSettings.StartDelay - (_noteSettings.StartDelay / pitch);

        return _nextEventTime + startDelay;
    }

    private int[] GetScaleIntervals(ScaleMode mode)
    {
        return mode switch {
            ScaleMode.Ionian => Ionian,
            ScaleMode.Dorian => Dorian,
            ScaleMode.Phrygian => Phrygian,
            ScaleMode.Lydian => Lydian,
            ScaleMode.Mixolydian => Mixolydian,
            ScaleMode.Aeolian => Aeolian,
            ScaleMode.Locrian => Locrian,
            _ => Ionian
        };
    }


    // TODO: temporary
    public void SetModeInt(int modeInt)
    {
        if (modeInt < 0 || modeInt >= Enum.GetValues(typeof(ScaleMode)).Length) {
            Debug.LogError("Invalid mode index: " + modeInt);
            return;
        }
        _mode = (ScaleMode)modeInt;
    }

    public void NextMode()
    {
        int nextModeIndex = ((int)_mode + 1) % Enum.GetValues(typeof(ScaleMode)).Length;
        _mode = (ScaleMode)nextModeIndex;
    }
}
