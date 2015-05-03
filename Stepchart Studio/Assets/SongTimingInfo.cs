using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents timing info for a song which includes things like
/// multiple beats per minute ranges, stops/freezes, etc.
/// </summary>
public class SongTimingInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SongTimingInfo"/> class.
    /// </summary>
    public SongTimingInfo()
    {
        this.BpmSegments = new List<SongBpmSegment>();
        this.StopSegments = new List<SongStopSegment>();
    }

    /// <summary>
    /// Gets the BPM segments that exist in the song.
    /// </summary>
    public List<SongBpmSegment> BpmSegments
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the stop segments that exist in the song.
    /// </summary>
    public List<SongStopSegment> StopSegments
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets or sets the offset in seconds of the first beat.
    /// </summary>
    public float FirstBeatOffsetInSeconds
    {
        get;
        set;
    }
}
