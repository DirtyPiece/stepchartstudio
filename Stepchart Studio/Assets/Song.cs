using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents a song which contains stepchart notes and timings
/// for various difficulties as well as media for playback.
/// </summary>
public class Song : MonoBehaviour
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Song"/> class.
    /// </summary>
    public Song()
    {
        this.Steps = new List<Steps>();
    }

    /// <summary>
    /// Gets or sets the list of steps that identify each stepchart for this song.
    /// </summary>
    public List<Steps> Steps
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the title of the song.
    /// </summary>
    public string Title
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the subtitle of the song.
    /// </summary>
    public string SubTitle
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the artist of the song.
    /// </summary>
    public string Artist
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets translit title of the song.
    /// </summary>
    public string TransLitTitle
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the translit subtitle of the song.
    /// </summary>
    public string TransLitSubTitle
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the translit artist of the song.
    /// </summary>
    public string TransLitArtist
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the genre of the song.
    /// </summary>
    public string Genre
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the credits of who made the stepcharts for the song.
    /// </summary>
    public string Credits
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the file path to the banner image for the song.
    /// </summary>
    public string BannerImageFilePath
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the file path to the background image for the song.
    /// </summary>
    public string BackgroundImageFilePath
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the file path to the lyrics for the song.
    /// </summary>
    public string LyricsFilePath
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets CD title of the song.
    /// </summary>
    public string CDTitle
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the file path to the music file that plays the soundtrack.
    /// </summary>
    public string MusicFilePath
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the length of the music in seconds.
    /// </summary>
    public float MusicLengthInSeconds
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the offset of the first beat in seconds.
    /// </summary>
    public float FirstBeatOffsetInSeconds
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the offset of the last beat in seconds.
    /// </summary>
    public float LastBeatOffsetInSeconds
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the song file name.
    /// </summary>
    public string FileName
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the song has music or not.
    /// </summary>
    public bool HasMusic
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the song has a banner or not.
    /// </summary>
    public bool HasBanner
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the start offset in seconds where the sample music preview will play.
    /// </summary>
    public float SampleMusicStartOffsetInSeconds
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the length of the sample audio clip in seconds.
    /// </summary>
    public float SampleLengthInSeconds
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets minimum beats per minute value to show on the song selection wheel.
    /// </summary>
    public float MinimumBeatsPerMinute
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the maximum beats per munute value to show on the song selection wheel.
    /// </summary>
    public float MaximumBeatsPerMinute
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the type of BPM display that is used.
    /// </summary>
    public SongDisplayBpmType BeatsPerMinuteDisplayType
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the visibility of the song to the player in the song selection screen.
    /// </summary>
    public SongVisibilityType Visibility
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the timing info for the song.
    /// </summary>
    public SongTimingInfo TimingInfo
    {
        get;
        set;
    }
}
