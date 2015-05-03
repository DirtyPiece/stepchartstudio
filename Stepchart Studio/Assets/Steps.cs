using UnityEngine;
using System.Collections;

/// <summary>
/// Represents the steps in a song (series of stepcharts for various difficulties).
/// </summary>
public class Steps
{
    /// <summary>
    /// Gets or sets the type/mode of these steps.
    /// </summary>
    public StepsType Type
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the difficulty of the steps.
    /// </summary>
    public StepsDifficulty Difficulty
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the description of the steps.
    /// </summary>
    public string Description
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the meter index offset.
    /// </summary>
    public int MeterIndexOffset
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the radar values for the song.
    /// </summary>
    public StepsRadarValues RadarValues
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the notes data for the song.
    /// </summary>
    public StepsNoteData NoteData
    {
        get;
        set;
    }
}
