using UnityEngine;
using System.Collections;

/// <summary>
/// Represents helper methods for the string class.
/// </summary>
public static class StringHelper
{
    /// <summary>
    /// Determines whether the passed in string is <c>null</c> or whitespace.
    /// </summary>
    /// <param name="str">The string to test.</param>
    /// <returns><c>true</c> if the string is <c>null</c> or whitespace, <c>false</c> otherwise.</returns>
    public static bool IsNullOrWhiteSpace(string str)
    {
        return string.IsNullOrEmpty(str) || str.Trim().Length == 0;
    }
}
