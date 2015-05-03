using UnityEngine;
using System.Collections;

/// <summary>
/// Represents a class that holds contants used throughout the codebase.
/// </summary>
public static class Constants
{
    /// <summary>
    /// The divisor used as a fixed-point time/beat representation.  Must be evenly
    /// divisible by 2, 3, and 4, and exactly represent 8th, 12th, and 16th notes.
    /// </summary>
    public const int RowsPerBeat = 48;

    /// <summary>
    /// The minimum number of parameters that must be present when parsing NOTES.
    /// </summary>
    public const int MinimumNotesParameterCount = 7;
}
