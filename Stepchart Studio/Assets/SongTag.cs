using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SongTag
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SongTag"/> class.
    /// </summary>
    public SongTag()
    {
        this.Values = new List<string>();
    }

    /// <summary>
    /// Gets or sets the marker for the tag that comes after the '#' (ex. "NOTES").
    /// </summary>
    public string Marker
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the values of the tag.
    /// </summary>
    public List<string> Values
    {
        get;
        set;
    }
}
