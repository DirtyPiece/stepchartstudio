using UnityEngine;
using System.Collections;

/// <summary>
/// Represents a segment of time in the song where the beats per
/// minute change to a different value.
/// </summary>
public class SongBpmSegment {
    /// <summary>
    /// Gets or sets the index of the row that this BPM segment begins at.
    /// </summary>
    public int StartRowIndex
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the beats per minute for this segment.
    /// </summary>
    public float BeatsPerMinute
    {
        get;
        set;
    }
}
