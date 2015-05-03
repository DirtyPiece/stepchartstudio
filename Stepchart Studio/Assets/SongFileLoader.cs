using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

/// <summary>
/// Represents a file loader for song files (DWI, SM, etc.).
/// </summary>
public static class SongFileLoader {
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

        Dictionary<string, List<string>> tagParamTable = this.LoadTagsAndParameters(fileContents);
        Song song = this.BuildSongFromTags(tagParamTable);

        return song;
    }

    /// <summary>
    /// Loads the tags and parameters from the song file.
    /// For example, #OFFSET:-0.040; where OFFSET is the tag and -0.040 is the parameter value.
    /// </summary>
    /// <param name="fileContents">The file contents to load the tags from.</param>
    /// <returns>The table of tags and their list of parameters.</returns>
    private Dictionary<string, List<string>> LoadTagsAndParameters(string fileContents)
    {
        Logger.LogVerbose("Loading the tags and parameters.");
        Scanner scanner = new Scanner(fileContents);

        Dictionary<string, List<string>> tagTable = new Dictionary<string, List<string>>();

        // Read to the first tag marker.
        scanner.ReadUntil('#');
        while (!scanner.IsEndOfFile)
        {
            // Skip the '#' character.
            scanner.Skip(1);

            // Scan to the next parameter marker or key marker.
            string key = scanner.ReadUntil(':', '#').ToUpper();

            if (this.IsDuplicateTag(key, scanner, tagTable))
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

            tagTable[key] = parameterList;
        }

        return tagTable;
    }

    /// <summary>
    /// Determines if the passed in key is a duplicate tag that has already
    /// been parsed.  If so, a warning will be logged and the entry will be
    /// skipped.
    /// </summary>
    /// <param name="key">The key to test (tag).</param>
    /// <param name="scanner">The scanner to use to bypass the tag if it's a duplicate.</param>
    /// <param name="tagTable">The tag table to use for testing the tag.</param>
    /// <returns><c>true</c> if the tag is a duplicate and was skipped, <c>false</c> otherwise.</returns>
    private bool IsDuplicateTag(
        string key,
        Scanner scanner,
        Dictionary<string,
        List<string>> tagTable)
    {
        if (tagTable.ContainsKey(key))
        {
            // If we already contain the current key, then we've encountered
            // a duplicate and need to skip it.
            Logger.LogWarning("The song file contains a duplicate '{0}' tag in it, ignoring.", key);
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
    /// Builds the song from tags table.
    /// </summary>
    /// <param name="tagParamTable">The tag parameter table.</param>
    /// <returns>The song that represents all the stepcharts for all difficulties in the file.</returns>
    private Song BuildSongFromTags(Dictionary<string, List<string>> tagParamTable)
    {
        Song song = new Song();

        // Parse out the timing information first.
        song.TimingInfo = this.ParseTimingInfo(tagParamTable);

        return song;
    }

    /// <summary>
    /// Parses the timing information from the tag table.
    /// </summary>
    /// <param name="tagParamTable">The tag parameter table.</param>
    /// <returns>The parsed timing information.</returns>
    private SongTimingInfo ParseTimingInfo(Dictionary<string, List<string>> tagParamTable)
    {
        SongTimingInfo info = new SongTimingInfo();

        // Read out the seconds offset of the first beat in the song.
        if (tagParamTable.ContainsKey("OFFSET"))
        {
            float offset = 0.0f;
            List<string> values = tagParamTable["OFFSET"];
            if (values.Count <= 0 || !float.TryParse(values[0], out offset))
            {
                Logger.LogWarning("Unable to parse the OFFSET value so setting the first beat offset to 0 seconds.");
            }

            info.FirstBeatOffsetInSeconds = offset;
        }

        // Read the stops in the song.
        List<string> stopsValues = new List<string>();
        if (tagParamTable.ContainsKey("STOPS"))
        {
            stopsValues = tagParamTable["STOPS"];
        }
        else if (tagParamTable.ContainsKey("FREEZE"))
        {
            stopsValues = tagParamTable["FREEZE"];
        }

        if (stopsValues.Count > 0)
        {
            string[] stopExpressions = stopsValues[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

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

        if (tagParamTable.ContainsKey("BPMS"))
        {
            List<string> bpmsValues = tagParamTable["BPMS"];
            if (bpmsValues.Count > 0)
            {
                string[] bpmsExpressions = bpmsValues[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

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
        }
    }
}
