using UnityEngine;
using System.Collections;

/// <summary>
/// Represents the different types of visibility that a song can have in game.
/// </summary>
public enum SongVisibilityType
{
    /// <summary>
    /// The song should always be displayed for selection by the player.
    /// </summary>
    Visible,

    /// <summary>
    /// The song is hidden from selection by the player.
    /// </summary>
    Hidden,

    /// <summary>
    /// The song is only visible when selecting roulette.
    /// </summary>
    RouletteOnly,
}