using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

/// <summary>
/// Represents a file loader for song files (DWI, SM, etc.).
/// </summary>
public static class SongFileLoader
{
    /// <summary>
    /// The step type to tracks lookup table.
    /// </summary>
    private static SortedDictionary<string, int> stepTypeToTracksTable = new SortedDictionary<string, int>();

    /// <summary>
    /// Initializes the <see cref="SongFileLoader"/> class.
    /// </summary>
    static SongFileLoader()
    {
        stepTypeToTracksTable.Add("dance-single", 4);
        stepTypeToTracksTable.Add("dance-double", 8);
        stepTypeToTracksTable.Add("dance-couple", 8);
        stepTypeToTracksTable.Add("dance-solo", 6);
        stepTypeToTracksTable.Add("pump-single", 5);
        stepTypeToTracksTable.Add("pump-halfdouble", 6);
        stepTypeToTracksTable.Add("pump-double", 10);
        stepTypeToTracksTable.Add("pump-couple", 10);
        stepTypeToTracksTable.Add("ez2-single", 5);
        stepTypeToTracksTable.Add("ez2-double", 10);
        stepTypeToTracksTable.Add("ez2-real", 7);
        stepTypeToTracksTable.Add("para-single", 5);
        stepTypeToTracksTable.Add("para-versus", 10);
        stepTypeToTracksTable.Add("ds3ddx-single", 8);
        stepTypeToTracksTable.Add("bm-single5", 6);
        stepTypeToTracksTable.Add("bm-double5", 12);
        stepTypeToTracksTable.Add("bm-single7", 8);
        stepTypeToTracksTable.Add("bm-double7", 16);
        stepTypeToTracksTable.Add("maniax-single", 4);
        stepTypeToTracksTable.Add("maniax-double", 8);
        stepTypeToTracksTable.Add("techno-single4", 4);
        stepTypeToTracksTable.Add("techno-single5", 5);
        stepTypeToTracksTable.Add("techno-single8", 8);
        stepTypeToTracksTable.Add("techno-double4", 8);
        stepTypeToTracksTable.Add("techno-double5", 10);
        stepTypeToTracksTable.Add("pnm-five", 5);
        stepTypeToTracksTable.Add("pnm-nine", 9);
        stepTypeToTracksTable.Add("lights-cabinet", 8);
    }

    /// <summary>
    /// Loads a stepchart song file from the specified path.
    /// </summary>
    /// <param name="filePath">The file path to the stepchart to load.</param>
    /// <returns>The loaded song.</returns>
    public Song LoadFromFile(string filePath)
    {
        Logger.LogInfo("Loading a song file from '{0}'.", filePath);

        string fileContents = File.ReadAllText(filePath);
        Logger.LogVerbose("Loaded song file contents of:\n{0}", fileContents);

        List<SongTag> tagList = this.LoadTags(fileContents);
        Song song = this.BuildSongFromTags(tagList);

        return song;
    }

    /// <summary>
    /// Loads the tags and parameters from the song file.
    /// For example, #OFFSET:-0.040; where OFFSET is the tag and -0.040 is the parameter value.
    /// </summary>
    /// <param name="fileContents">The file contents to load the tags from.</param>
    /// <returns>The list of tags and their list of parameters.</returns>
    private List<SongTag> LoadTags(string fileContents)
    {
        Logger.LogVerbose("Loading the tags and parameters.");
        Scanner scanner = new Scanner(fileContents);

        List<SongTag> tagList = new List<SongTag>();

        // Read to the first tag marker.
        scanner.ReadUntil('#');
        while (!scanner.IsEndOfFile)
        {
            // Skip the '#' character.
            scanner.Skip(1);

            // Scan to the next parameter marker or key marker.
            string marker = scanner.ReadUntil(':', '#').ToUpper();

            if (this.IsDuplicateTag(marker, scanner, tagList))
            {
                continue;
            }

            List<string> parameterList = new List<string>();

            if (!scanner.IsEndOfFile)
            {
                // Read all the parameters (if there are any).
                while (!scanner.IsEndOfFile && scanner.Peek() == ':')
                {
                    scanner.Skip(1);
                    string parameterValue = scanner.ReadUntil(':', '#');
                    parameterList.Add(parameterValue);
                }
            }

            tagList.Add(new SongTag { Marker = marker, Values = parameterList });
        }

        return tagList;
    }

    /// <summary>
    /// Determines if the passed in key is a duplicate tag that has already
    /// been parsed.  If so, a warning will be logged and the entry will be
    /// skipped.
    /// </summary>
    /// <param name="tag">The tag to test.</param>
    /// <param name="scanner">The scanner to use to bypass the tag if it's a duplicate.</param>
    /// <param name="tagList">The tag list to use for testing the tag.</param>
    /// <returns><c>true</c> if the tag is a duplicate and was skipped, <c>false</c> otherwise.</returns>
    private bool IsDuplicateTag(
        string tag,
        Scanner scanner,
        List<SongTag> tagList)
    {
        if (tag == "NOTES" || tag == "NOTES2")
        {
            // We allow duplicate notes because there can be multiple for different
            // difficulties.
            return false;
        }

        if (tagList.Select(t => t.Marker).Contains(tag))
        {
            // If we already contain the current key, then we've encountered
            // a duplicate and need to skip it.
            Logger.LogWarning("The song file contains a duplicate '{0}' tag in it, ignoring.", tag);
            if (!scanner.IsEndOfFile && scanner.Peek() != '#')
            {
                // Move to the next key marker.
                scanner.ReadUntil('#');
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// Builds the song from the tags list.
    /// </summary>
    /// <param name="tagList">The tag parameter list.</param>
    /// <returns>The song that represents all the stepcharts for all difficulties in the file.</returns>
    private Song BuildSongFromTags(List<SongTag> tagList)
    {
        Song song = new Song();
        song.TimingInfo = new SongTimingInfo();

        foreach (SongTag tag in tagList)
        {
            switch (tag.Marker)
            {
                case "OFFSET":
                    this.ParseTimingInfoOffset(song.TimingInfo, tag);
                    break;
                case "STOPS":
                case "FREEZE":
                    this.ParseTimingInfoStops(song.TimingInfo, tag);
                    break;
                case "BPMS":
                    this.ParseTimingInfoBpms(song.TimingInfo, tag);
                    break;
                case "TITLE":
                    song.Title = tag.Values.FirstOrDefault();
                    break;
                case "SUBTITLE":
                    song.SubTitle = tag.Values.FirstOrDefault();
                    break;
                case "ARTIST":
                    song.Artist = tag.Values.FirstOrDefault();
                    break;
                case "TITLETRANSLIT":
                    song.TransLitTitle = tag.Values.FirstOrDefault();
                    break;
                case "SUBTITLETRANSLIT":
                    song.TransLitSubTitle = tag.Values.FirstOrDefault();
                    break;
                case "ARTISTTRANSLIT":
                    song.TransLitArtist = tag.Values.FirstOrDefault();
                    break;
                case "GENRE":
                    song.Genre = tag.Values.FirstOrDefault();
                    break;
                case "CREDIT":
                    song.Credits = tag.Values.FirstOrDefault();
                    break;
                case "BANNER":
                    song.BannerImageFilePath = tag.Values.FirstOrDefault();
                    break;
                case "BACKGROUND":
                    song.BackgroundImageFilePath = tag.Values.FirstOrDefault();
                    break;
                case "LYRICSPATH":
                    song.LyricsFilePath = tag.Values.FirstOrDefault();
                    break;
                case "CDTITLE":
                    song.CDTitle = tag.Values.FirstOrDefault();
                    break;
                case "MUSIC":
                    song.MusicFilePath = tag.Values.FirstOrDefault();
                    break;
                case "MUSICLENGTH":
                    if (tag.Values.Count > 0)
                    {
                        float musicLengthInSeconds = 0.0f;
                        if (!float.TryParse(tag.Values[0], out musicLengthInSeconds))
                        {
                            Logger.LogWarning("Could not load the MUSICLENGTH value of '{0}'.  It is in an incorrect format and should be in seconds.", tag.Values[0]);
                            continue;
                        }

                        song.MusicLengthInSeconds = musicLengthInSeconds;
                    }
                    break;
                case "FIRSTBEAT":
                    if (tag.Values.Count > 0)
                    {
                        float firstBeatInSeconds = 0.0f;
                        if (!float.TryParse(tag.Values[0], out firstBeatInSeconds))
                        {
                            Logger.LogWarning("Could not load the FIRSTBEAT value of '{0}'.  It is in an incorrect format and should be in seconds.", tag.Values[0]);
                            continue;
                        }

                        song.FirstBeatOffsetInSeconds = firstBeatInSeconds;
                    }
                    break;
                case "LASTBEAT":
                    if (tag.Values.Count > 0)
                    {
                        float lastBeatInSeconds = 0.0f;
                        if (!float.TryParse(tag.Values[0], out lastBeatInSeconds))
                        {
                            Logger.LogWarning("Could not load the LASTBEAT value of '{0}'.  It is in an incorrect format and should be in seconds.", tag.Values[0]);
                            continue;
                        }

                        song.LastBeatOffsetInSeconds = lastBeatInSeconds;
                    }
                    break;
                case "SONGFILENAME":
                    song.FileName = tag.Values.FirstOrDefault();
                    break;
                case "HASMUSIC":
                    if (tag.Values.Count > 0)
                    {
                        int hasMusic = 0;
                        if (!int.TryParse(tag.Values[0], out hasMusic))
                        {
                            Logger.LogWarning("Could not load the HASMUSIC value of '{0}'.  It is in an incorrect format and should be 0 or 1.", tag.Values[0]);
                            continue;
                        }

                        song.HasMusic = hasMusic != 0;
                    }
                    break;
                case "HASBANNER":
                    if (tag.Values.Count > 0)
                    {
                        int hasBanner = 0;
                        if (!int.TryParse(tag.Values[0], out hasBanner))
                        {
                            Logger.LogWarning("Could not load the HASBANNER value of '{0}'.  It is in an incorrect format and should be 0 or 1.", tag.Values[0]);
                            continue;
                        }

                        song.HasBanner = hasBanner != 0;
                    }
                    break;
                case "SAMPLESTART":
                    song.SampleMusicStartOffsetInSeconds = this.ConvertHoursMinutesSecondsToSeconds(tag);
                    break;
                case "SAMPLELENGTH":
                    song.SampleLengthInSeconds = this.ConvertHoursMinutesSecondsToSeconds(tag);
                    break;
                case "DISPLAYBPM":
                    this.ParseDisplayBpm(tag, song);
                    break;
                case "SELECTABLE":
                    this.ParseSelectable(tag, song);
                    break;
                case "NOTES":
                case "NOTES2":
                    this.ParseNotes(tag, song);
                    break;
                case "MUSICBYTES":
                    // Ignore.
                    break;
                default:
                    Logger.LogWarning("Unknown marker '{0}' encountered so skipping it.", tag.Marker);
                    break;
            }
        }

        return song;
    }

    /// <summary>
    /// Parses the notes.
    /// </summary>
    /// <param name="tag">The tag to parse.</param>
    /// <param name="song">The song to parse into.</param>
    private void ParseNotes(SongTag tag, Song song)
    {
        if (tag.Values.Count < Constants.MinimumNotesParameterCount)
        {
            Logger.LogWarning("The {0} tag has '{1}' values in the tag, but should have at least '{2}'.", tag.Marker, tag.Values.Count, Constants.MinimumNotesParameterCount);
            return;
        }

        Steps steps = new Steps();
        steps.Type = this.ParseStepsType(tag.Values[0]);
        steps.Description = tag.Values[1];
        steps.Difficulty = this.ParseDifficulty(tag.Values[2]);

        // SMANIAC and DIFFICULTY_HARD used to be stored with special description.
        // Now it is stored as DIFFICULTY_CHALLENGE.
        if (steps.Description.ToLower() == "smaniac")
        {
            steps.Difficulty = StepsDifficulty.Challenge;
        }

        // CHALLENGE and DIFFICULTY_HARD used to be stored with special description.
        // Now it is stored as DIFFICULTY_CHALLENGE.
        if (steps.Description.ToLower() == "challenge")
        {
            steps.Difficulty = StepsDifficulty.Challenge;
        }

        int meterIndex = 0;
        if (!int.TryParse(tag.Values[3], out meterIndex))
        {
            Logger.LogWarning("Could not parse the meter index value for the {0} tag. Should be an integer offset value.", tag.Marker);
        }

        steps.MeterIndexOffset = meterIndex;
        steps.RadarValues = this.ParseRadarValues(tag.Values[4]);
        steps.NoteData = this.ParseNoteData(tag.Values[5]);

        song.Steps.Add(steps);
    }

    /// <summary>
    /// Parses the note data.
    /// </summary>
    /// <param name="value">The value to parse.</param>
    /// <returns>The note data for the song.</returns>
    private StepsNoteData ParseNoteData(string value)
    {

    }

    /// <summary>
    /// Parses the radar values.
    /// </summary>
    /// <param name="value">The value to parse the values from.</param>
    /// <returns>The radar values for the song.</returns>
    private StepsRadarValues ParseRadarValues(string value)
    {
        string[] values = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        if (values.Length != Enum.GetValues(typeof(StepsRadarCategoryType)).Length)
        {
            Logger.LogWarning("The number of radar values specified was '{0}' when it should contain only of the following:\n{1}", values.Length, Enum.GetNames(typeof(StepsRadarCategoryType)));
            return null;
        }

        StepsRadarValues radarValues = new StepsRadarValues();

        float floatValue = 0.0f;
        if (!float.TryParse(values[0], out floatValue))
        {
            Logger.LogWarning("Could not parse the Stream radar value of '{0}' as it is invalid. Must be a float value.", values[0]);
            floatValue = 0.0f;
        }

        radarValues.Stream = floatValue;

        if (!float.TryParse(values[1], out floatValue))
        {
            Logger.LogWarning("Could not parse the Voltage radar value of '{0}' as it is invalid. Must be a float value.", values[1]);
            floatValue = 0.0f;
        }

        radarValues.Voltage = floatValue;

        if (!float.TryParse(values[2], out floatValue))
        {
            Logger.LogWarning("Could not parse the Air radar value of '{0}' as it is invalid. Must be a float value.", values[2]);
            floatValue = 0.0f;
        }

        radarValues.Air = floatValue;

        if (!float.TryParse(values[3], out floatValue))
        {
            Logger.LogWarning("Could not parse the Freeze radar value of '{0}' as it is invalid. Must be a float value.", values[3]);
            floatValue = 0.0f;
        }

        radarValues.Freeze = floatValue;

        if (!float.TryParse(values[4], out floatValue))
        {
            Logger.LogWarning("Could not parse the Chaos radar value of '{0}' as it is invalid. Must be a float value.", values[4]);
            floatValue = 0.0f;
        }

        radarValues.Chaos = floatValue;

        int intValue = 0;
        if (!int.TryParse(values[5], out intValue))
        {
            Logger.LogWarning("Could not parse the TapsAndHoldsCount radar value of '{0}' as it is invalid. Must be an int value.", values[5]);
            intValue = 0;
        }

        radarValues.TapsAndHoldsCount = intValue;

        if (!int.TryParse(values[6], out intValue))
        {
            Logger.LogWarning("Could not parse the JumpsCount radar value of '{0}' as it is invalid. Must be an int value.", values[6]);
            intValue = 0;
        }

        radarValues.JumpsCount = intValue;

        if (!int.TryParse(values[7], out intValue))
        {
            Logger.LogWarning("Could not parse the HoldsCount radar value of '{0}' as it is invalid. Must be an int value.", values[7]);
            intValue = 0;
        }

        radarValues.HoldsCount = intValue;

        if (!int.TryParse(values[8], out intValue))
        {
            Logger.LogWarning("Could not parse the MinesCount radar value of '{0}' as it is invalid. Must be an int value.", values[8]);
            intValue = 0;
        }

        radarValues.MinesCount = intValue;

        if (!int.TryParse(values[9], out intValue))
        {
            Logger.LogWarning("Could not parse the HandsCount radar value of '{0}' as it is invalid. Must be an int value.", values[9]);
            intValue = 0;
        }

        radarValues.HandsCount = intValue;

        if (!int.TryParse(values[10], out intValue))
        {
            Logger.LogWarning("Could not parse the RollsCount radar value of '{0}' as it is invalid. Must be an int value.", values[10]);
            intValue = 0;
        }

        radarValues.RollsCount = intValue;

        return radarValues;
    }

    /// <summary>
    /// Parses the difficulty value.
    /// </summary>
    /// <param name="value">The value to parse.</param>
    /// <returns>The parsed difficulty value.</returns>
    private StepsDifficulty ParseDifficulty(string value)
    {
        switch (value.ToLower())
        {
            case "beginner":
                return StepsDifficulty.Beginner;
            case "easy":
            case "basic":
            case "light":
                return StepsDifficulty.Easy;
            case "medium":
            case "another":
            case "trick":
            case "standard":
            case "difficult":
                return StepsDifficulty.Medium;
            case "hard":
            case "ssr":
            case "maniac":
            case "heavy":
                return StepsDifficulty.Hard;
            case "smaniac":
            case "expert":
            case "oni":
                return StepsDifficulty.Challenge;
            default:
                Logger.LogWarning("Unsupported difficulty of '{0}' parsed so setting to Easy.", value);
                return StepsDifficulty.Easy;
        }
    }

    /// <summary>
    /// Parses the type of the steps.
    /// </summary>
    /// <param name="value">The value to parse.</param>
    /// <returns>The parsed steps type.</returns>
    private StepsType ParseStepsType(string value)
    {
        value = value.ToLower();

        // A ez2-single-hard type used to exist but doesn't anymore, so convert it.
        if (value == "ez2-single-hard")
        {
            value = "ez2-single";
        }

        // A para-single also used to be used, but is no longer.
        if (value == "para")
        {
            value = "para-single";
        }

        if (!stepTypeToTracksTable.ContainsKey(value))
        {
            Logger.LogWarning("Step type of '{0}' is invalid so reverting to dance-single. Must be one of:\n{1}", value, stepTypeToTracksTable.Select(e => e.Key));
            return StepsType.DanceSingle;
        }

        int index = 0;
        foreach (KeyValuePair<string, int> pair in stepTypeToTracksTable)
        {
            if (pair.Key == value)
            {
                break;
            }

            ++index;
        }

        return (StepsType)index;
    }

    /// <summary>
    /// Parses the selectable tag.
    /// </summary>
    /// <param name="tag">The tag to parse.</param>
    /// <param name="song">The song to parse into.</param>
    private void ParseSelectable(SongTag tag, Song song)
    {
        string value = tag.Values.FirstOrDefault();
        if (StringHelper.IsNullOrWhiteSpace(value))
        {
            Logger.LogWarning("The SELECTABLE value was not specified.");
            return;
        }

        switch (value)
        {
            case "YES":
                song.Visibility = SongVisibilityType.Visible;
                break;
            case "NO":
                song.Visibility = SongVisibilityType.Hidden;
                break;
            case "ROULETTE":
                song.Visibility = SongVisibilityType.RouletteOnly;
                break;
            default:
                Logger.LogWarning("Unknown SELECTABLE value of '{0}' parsed, ignoring.", value);
                break;
        }
    }

    /// <summary>
    /// Parses the display BPM.
    /// </summary>
    /// <param name="tag">The tag to parse.</param>
    /// <param name="song">The song to parse into.</param>
    private void ParseDisplayBpm(SongTag tag, Song song)
    {
        // Possible values for the display BPM are:
        // [XXX]        - Single BPM value displayed (ex. "140.50").
        // [XXX:XXX]    - BPM range values displayed (ex. "100.25-300").
        // [*]          - BPM is random (cycles random numbers).
        string value = tag.Values.FirstOrDefault();
        if (StringHelper.IsNullOrWhiteSpace(value))
        {
            Logger.LogWarning("The DISPLAYBPM value was not specified.");
            return;
        }

        if (value == "*")
        {
            song.BeatsPerMinuteDisplayType = SongDisplayBpmType.Random;
        }
        else
        {
            float beatsPerMinuteMinimum = 0.0f;
            if (!float.TryParse(value, out beatsPerMinuteMinimum))
            {
                Logger.LogWarning("The DISPLAYBPM value of '{0}' is not valid. Should be one of (float values): [XXX|XXX-XXX|*].", value);
                return;
            }

            song.MinimumBeatsPerMinute = beatsPerMinuteMinimum;

            float beatsPerMinuteMaximum = beatsPerMinuteMinimum;
            if (tag.Values.Count > 1)
            {
                if (!float.TryParse(tag.Values[0], out beatsPerMinuteMaximum))
                {
                    Logger.LogWarning("The DISPLAYBPM maximum range value of '{0}' is not valid. Should be one of (float values): [XXX|XXX-XXX|*].", tag.Values[0]);
                    return;
                }
            }

            song.MaximumBeatsPerMinute = beatsPerMinuteMaximum;
            song.BeatsPerMinuteDisplayType = SongDisplayBpmType.Specified;
        }
    }

    /// <summary>
    /// Convers the HH:MM:SS format to seconds.
    /// </summary>
    /// <param name="tag">The tag that we're parsing the value from.</param>
    /// <returns>
    /// The total number of seconds.
    /// </returns>
    private float ConvertHoursMinutesSecondsToSeconds(SongTag tag)
    {
        string value = tag.Values.FirstOrDefault();
        if (StringHelper.IsNullOrWhiteSpace(value))
        {
            Logger.LogWarning("The {0} tag doesn't contain a value.", tag.Marker);
            return 0.0f;
        }

        List<string> components = value.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).ToList();

        // Pad the upper components if they're missing.
        while (components.Count < 3)
        {
            components.Insert(0, "0");
        }

        float seconds = 0.0f;
        int intValue = 0;
        if (!int.TryParse(components[0], out intValue))
        {
            Logger.LogWarning("Unable to parse the hours component of the {0} marker with value {1}. It's in an invalid format - should be an integer.", tag.Marker, components[0]);
            return 0.0f;
        }

        // Hours.
        seconds += intValue * 60 * 60;

        if (!int.TryParse(components[1], out intValue))
        {
            Logger.LogWarning("Unable to parse the minutes component of the {0} marker with value {1}. It's in an invalid format - should be an integer.", tag.Marker, components[1]);
            return 0.0f;
        }

        // Minutes.
        seconds += intValue * 60;

        float floatValue = 0.0f;
        if (!float.TryParse(components[2], out floatValue))
        {
            Logger.LogWarning("Unable to parse the seconds component of the {0} marker with value {1}. It's in an invalid format - should be an float value.", tag.Marker, components[2]);
            return 0.0f;
        }

        // Minutes.
        seconds += floatValue;

        Logger.LogVerbose("Parsed out {0} value of '{1}' total seconds.", tag.Marker, seconds);
        return seconds;
    }

    /// <summary>
    /// Parses the timing information BPMS.
    /// </summary>
    /// <param name="info">The information to parse into.</param>
    /// <param name="tag">The tag that contains the values.</param>
    private void ParseTimingInfoBpms(SongTimingInfo info, SongTag tag)
    {
        if (tag.Values.Count > 0)
        {
            string[] bpmsExpressions = tag.Values[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < bpmsExpressions.Length; ++i)
            {
                string[] pair = bpmsExpressions[i].Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (pair.Length != 2)
                {
                    Logger.LogWarning("The expression of '{0}' in the BPMS section is invalid so skipping it.", bpmsExpressions[i]);
                    continue;
                }

                float bpmOffsetInSeconds = 0.0f;
                if (!float.TryParse(pair[0], out bpmOffsetInSeconds))
                {
                    Logger.LogWarning("The BPMS offset in seconds of '{0}' is not valid so skipping it.", pair[0]);
                    continue;
                }

                float bpmValue = 0.0f;
                if (!float.TryParse(pair[1], out bpmValue))
                {
                    Logger.LogWarning("The BPMS value of '{0}' is not valid so skipping it.", pair[1]);
                    continue;
                }

                SongBpmSegment bpmSegment = new SongBpmSegment
                {
                    StartRowIndex = BeatHelper.ConvertBeatToNoteRowIndex(bpmOffsetInSeconds),
                    BeatsPerMinute = bpmValue,
                };

                info.BpmSegments.Add(bpmSegment);
            }
        }
        else
        {
            Logger.LogWarning("There was a {0} marker specified with no values.", tag.Marker);
        }
    }

    /// <summary>
    /// Parses the timing info stops.
    /// </summary>
    /// <param name="info">The information to parse into.</param>
    /// <param name="tag">The tag that contains the values.</param>
    private void ParseTimingInfoStops(SongTimingInfo info, SongTag tag)
    {
        if (tag.Values.Count > 0)
        {
            string[] stopExpressions = tag.Values[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < stopExpressions.Length; ++i)
            {
                string[] pair = stopExpressions[i].Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (pair.Length != 2)
                {
                    Logger.LogWarning("The expression of '{0}' in the STOPS/FREEZE section is invalid so skipping it.", stopExpressions[i]);
                    continue;
                }

                float stopOffsetInSeconds = 0.0f;
                if (!float.TryParse(pair[0], out stopOffsetInSeconds))
                {
                    Logger.LogWarning("The STOPS/FREEZE offset in seconds of '{0}' is not valid so skipping it.", pair[0]);
                    continue;
                }

                float stopLengthInSeconds = 0.0f;
                if (!float.TryParse(pair[1], out stopLengthInSeconds))
                {
                    Logger.LogWarning("The STOPS/FREEZE length in seconds of '{0}' is not valid so skipping it.", pair[1]);
                    continue;
                }

                SongStopSegment stopSegment = new SongStopSegment
                {
                    StartRowIndex = BeatHelper.ConvertBeatToNoteRowIndex(stopOffsetInSeconds),
                    StopTimeInSeconds = stopLengthInSeconds,
                };

                info.StopSegments.Add(stopSegment);
            }
        }
        else
        {
            Logger.LogWarning("There was a {0} marker specified with no values.", tag.Marker);
        }
    }

    /// <summary>
    /// Parses the timing information offset in seconds.
    /// </summary>
    /// <param name="info">The information to parse into.</param>
    /// <param name="tag">The tag that contains the values.</param>
    private void ParseTimingInfoOffset(SongTimingInfo info, SongTag tag)
    {
        float offset = 0.0f;
        if (tag.Values.Count <= 0 || !float.TryParse(tag.Values[0], out offset))
        {
            Logger.LogWarning("Unable to parse the OFFSET value so setting the first beat offset to 0 seconds.");
        }

        info.FirstBeatOffsetInSeconds = offset;
    }
}
