using System;
using System.Linq;
using UnityEngine;

[Serializable]
public struct UnlockableNotesGroup
{
    public SequencerNote[] notes;
}

public class TreesManager : MonoBehaviour
{
    private static TreesManager _instance;
    public static TreesManager Instance => _instance != null ? _instance : throw new InvalidOperationException("TreesManager instance is not initialized.");

    [SerializeField] UnlockableNotesGroup[] _unlockableNotes;

    private int _currentGroupIndex = 0;
    private int _currentGroupAvailableNotes = 0;

    private void Awake()
    {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    private void Start()
    {
        _currentGroupIndex = 0;
        _currentGroupAvailableNotes = _unlockableNotes[_currentGroupIndex].notes.Length;
    }

    public Sprout UnlockNextTree()
    {
        var availableNotes = _unlockableNotes[_currentGroupIndex].notes.Where(note => !note.IsEnabled).ToArray();

        if (availableNotes.Length != _currentGroupAvailableNotes) {
            throw new InvalidOperationException("Available notes count does not match the current group available notes count.");
        }

        var selectedNote = availableNotes[UnityEngine.Random.Range(0, availableNotes.Length)];
        selectedNote.IsEnabled = true;
        _currentGroupAvailableNotes--;

        if (_currentGroupAvailableNotes <= 0) {
            _currentGroupIndex++;
            if (_currentGroupIndex < _unlockableNotes.Length) {
                _currentGroupAvailableNotes = _unlockableNotes[_currentGroupIndex].notes.Length;
            } else {
                Debug.Log("All trees unlocked!");
            }
        }

        return selectedNote.GetComponent<Sprout>();
    }
}
