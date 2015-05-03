using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Represents a helper class for performing beat calculations.
/// </summary>
public static class BeatHelper
{
    /// <summary>
    /// Converts the beat offset in seconds to a note row index.
    /// </summary>
    /// <param name="beatOffsetInSeconds">The beat offset in seconds to convert.</param>
    /// <returns>The index of the note row that this beat aligns with.</returns>
    public static int ConvertBeatToNoteRowIndex(float beatOffsetInSeconds)
    {
        return (int)Math.Round(beatOffsetInSeconds * Constants.RowsPerBeat);
    }
}
