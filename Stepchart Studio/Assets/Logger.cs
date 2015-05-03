using UnityEngine;
using System.Collections;

/// <summary>
/// Represents the main logger of Stepchart Studio.
/// </summary>
public static class Logger {
    /// <summary>
    /// Logs a verbose message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="args">The arguments to format into the message.</param>
    public static void LogVerbose(string message, params object[] args)
    {
        Debug.Log("V: " + string.Format(message, args));
    }

    /// <summary>
    /// Logs an informational message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="args">The arguments to format into the message.</param>
    public static void LogInfo(string message, params object[] args)
    {
        Debug.Log("I: " + string.Format(message, args));
    }

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="args">The arguments to format into the message.</param>
    public static void LogWarning(string message, params object[] args)
    {
        Debug.Log("W: " + string.Format(message, args));
    }

    /// <summary>
    /// Logs an error message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="args">The arguments to format into the message.</param>
    public static void LogError(string message, params object[] args)
    {
        Debug.Log("E: " + string.Format(message, args));
    }
}
