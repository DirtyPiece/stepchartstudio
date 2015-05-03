using UnityEngine;
using System.Collections;

/// <summary>
/// Represents a segment of time in the song where the steps
/// stop to a halt.
/// </summary>
public class SongStopSegment {
    /// <summary>
    /// Gets or sets the index of the row that this stop segment begins at.
    /// </summary>
    public int StartRowIndex
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the number of seconds that the segment stops for.
    /// </summary>
    public float StopTimeInSeconds
    {
        get;
        set;
    }
}
