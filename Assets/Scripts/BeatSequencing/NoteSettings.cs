using UnityEngine;

[CreateAssetMenu(fileName = "Note Settings", menuName = "Beat Sequencing/Note Settings")]
public class NoteSettings : ScriptableObject
{
    [SerializeField] AudioClip _noteClip;

    [SerializeField, Tooltip("Base MIDI note for this note. Middle C is 60.")]
    int _baseMidiNote = 60; // Middle C

    [SerializeField, Tooltip("Audio delay before the note sound starts playing, in seconds.")]
    float _startDelay = 0f;

    public AudioClip NoteClip => _noteClip;
    public int BaseMidiNote => _baseMidiNote;
    public float StartDelay => _startDelay;
}
