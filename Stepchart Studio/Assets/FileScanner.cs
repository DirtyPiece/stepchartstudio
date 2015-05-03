using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Linq;

/// <summary>
/// Represents a file scanner for reading DWI, SM,
/// BMS, etc. files.
/// </summary>
public class Scanner {
    /// <summary>
    /// The character buffer that contains the contents of the file being scanned.
    /// </summary>
    private string buffer = null;

    /// <summary>
    /// The current character offset in the scanner.
    /// </summary>
    private int characterOffset = -1;

    /// <summary>
    /// Initializes a new instance of the <see cref="Scanner"/> class.
    /// </summary>
    /// <param name="buffer">The buffer to scan.</param>
    public Scanner(string buffer)
    {
        Validator.IsNotNullOrWhiteSpace(buffer, "buffer");
        this.buffer = buffer;
        this.IgnoreCommentsOnScans = true;
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not comments should be ignored when scanning.
    /// </summary>
    public bool IgnoreCommentsOnScans
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a value indicating whether the current character offset is at the end of the file or not.
    /// </summary>
    /// <value>
    /// <c>true</c> if the character offset has reached the end of the file, <c>false</c> otherwise.
    /// </value>
    public bool IsEndOfFile
    {
        get
        {
            return this.characterOffset == this.buffer.Length;
        }
    }

    /// <summary>
    /// Reads the character from the current position and advances
    /// the offset by 1 in the file.
    /// </summary>
    /// <returns>The read character.</returns>
    public char ReadChar()
    {
        this.ValidateOffsetIsValid();
        return this.buffer[this.characterOffset++];
    }

    /// <summary>
    /// Returns the current character at the current scan position
    /// without advancing the character offset.
    /// </summary>
    /// <param name="offset">The offset to add to the current offset (can be negative to look back).</param>
    /// <returns>
    /// The peeked character.
    /// </returns>
    public char Peek(int offset = 0)
    {
        this.ValidateOffsetIsValid(offset);
        return this.buffer[this.characterOffset + offset];
    }

    /// <summary>
    /// Reads a string from the current position until one of the passed
    /// in parameters is found (or the end of file is reached).
    /// </summary>
    /// <param name="characters">The characters that will halt reading when encountered.</param>
    /// <returns>All of the characters that were read up until one of the specified characters was reached.</returns>
    public string ReadUntil(params char[] characters)
    {
        StringBuilder builder = new StringBuilder();
        while (!this.IsEndOfFile)
        {
            char c = this.Peek();
            if (this.IgnoreCommentsOnScans
             && c == '/'
             && this.IsOffsetValid(1)
             && this.Peek(1) == '/')
            {
                // If we've hit a comment (//) then scan until the end of the newline.
                while (!this.IsEndOfFile && this.ReadChar() != '\n')
                {
                    // Scan away.
                }
            }

            if (characters.Contains(c))
            {
                break;
            }

            builder.Append(this.ReadChar());
        }

        return builder.ToString();
    }

    /// <summary>
    /// Skips the specified number of characters.
    /// </summary>
    /// <param name="count">The number of characters to skip (can be negative to skip backwards).</param>
    public void Skip(int count)
    {
        int delta = count > 0 ? 1 : -1;
        int absoluteCount = Math.Abs(count);
        while (absoluteCount > 0)
        {
            this.ValidateOffsetIsValid();
            this.characterOffset += delta;
            --absoluteCount;
        }
    }

    /// <summary>
    /// Checks if the current offset plus an optional additional offset is valid (in bounds).
    /// </summary>
    /// <param name="offset">The offset to apply to the current offset.</param>
    /// <returns><c>true</c> if the specified offset is in bounds, <c>false</c> otherwise.</returns>
    public bool IsOffsetValid(int offset = 0)
    {
        int adjustedOffset = this.characterOffset + offset;
        if (adjustedOffset < 0 || adjustedOffset >= this.buffer.Length)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Validates the offset is valid (will throw an exception if not).
    /// </summary>
    /// <param name="offset">The offset to add from the current offset.</param>
    /// <exception cref="InvalidOperationException">The character offset is currently invalid.</exception>
    private void ValidateOffsetIsValid(int offset = 0)
    {
        if (!this.IsOffsetValid(offset))
        {
            throw new InvalidOperationException("The character offset is currently invalid.");
        }
    }
}
