using UnityEngine;
using System.Collections;

/// <summary>
/// Represents different display types when rendering the BPM values for a song.
/// </summary>
public enum SongDisplayBpmType
{
    /// <summary>
    /// The BPM type was not specified.
    /// </summary>
    None,

    /// <summary>
    /// The BPM single value or range is specified.
    /// </summary>
    Specified,

    /// <summary>
    /// The BPM shows random values.
    /// </summary>
    Random,
}
