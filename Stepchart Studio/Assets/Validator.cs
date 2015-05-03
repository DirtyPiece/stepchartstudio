using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Represents a validator for verifying parameters and program flow.
/// </summary>
public static class Validator {
    /// <summary>
    /// Validates that <paramref name="value"/> is not null or whitespace.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="parameterName">The name of the parameter being validated.</param>
    public static void IsNotNullOrWhiteSpace(string value, string parameterName)
    {
        if (StringHelper.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(parameterName);
        }
    }
}
