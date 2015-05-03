using UnityEngine;
using System.Collections;

/// <summary>
/// Represents various radar category types.
/// </summary>
public enum StepsRadarCategoryType
{
    /// <summary>
    /// The frequency of stream arrow patterns.
    /// </summary>
    Stream,

    /// <summary>
    /// The frequency of fast arrow patterns.
    /// </summary>
    Voltage,

    /// <summary>
    /// The frequency of jump arrow patterns.
    /// </summary>
    Air,

    /// <summary>
    /// The frequency of freeze/roll arrow patterns.
    /// </summary>
    Freeze,

    /// <summary>
    /// The frequency of tricky beat arrow patterns.
    /// </summary>
    Chaos,

    /// <summary>
    /// The number of taps and holds in the song.
    /// </summary>
    TapsAndHoldsCount,

    /// <summary>
    /// The number of jumps in the song.
    /// </summary>
    JumpsCount,

    /// <summary>
    /// The number of holds in the song.
    /// </summary>
    HoldsCount,

    /// <summary>
    /// The number of mines in the song.
    /// </summary>
    MinesCount,

    /// <summary>
    /// The number of hands in the song.
    /// </summary>
    HandsCount,

    /// <summary>
    /// The number of rolls in the song.
    /// </summary>
    RollsCount,
}
