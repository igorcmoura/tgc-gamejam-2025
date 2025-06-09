using System;

public static class EventsManager
{
    /// <summary>
    /// Action called every beat of the music.
    /// Parameters:
    /// 1. The current segment number (int).
    /// 2. The current beat number within the segment (int).
    /// </summary>
    public static Action<int, int> OnBeat;

    /// <summary>
    /// Action called when a fruit is spawned.
    /// Parameters:
    /// 1. The spawned fruit instance (Fruit).
    /// </summary>
    public static Action<Fruit> OnFruitSpawned;

    /// <summary>
    /// Action called when a fruit starts being dropped.
    /// Parameters:
    /// 1. The fruit instance that is being dropped (Fruit).
    /// </summary>
    public static Action<Fruit> OnFruitDropped;
}
