using UnityEngine;
using System.Collections;

/// <summary>
/// Represents the values stored in the radar histogram.
/// </summary>
public class StepsRadarValues
{
    /// <summary>
    /// Gets or sets the magnitude of the stream value.
    /// </summary>
    public float Stream
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the magnitude of the voltage value.
    /// </summary>
    public float Voltage
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the magnitude of the air value.
    /// </summary>
    public float Air
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the magnitude of the freeze value.
    /// </summary>
    public float Freeze
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the magnitude of the chaos value.
    /// </summary>
    public float Chaos
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the number of taps and holds in the song.
    /// </summary>
    public int TapsAndHoldsCount
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the number of jumps in the song.
    /// </summary>
    public int JumpsCount
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the number of holds in the song.
    /// </summary>
    public int HoldsCount
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the number of mintes in the song.
    /// </summary>
    public int MinesCount
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the number of the hands in the song.
    /// </summary>
    public int HandsCount
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the number of rolls in the song.
    /// </summary>
    public int RollsCount
    {
        get;
        set;
    }
}
