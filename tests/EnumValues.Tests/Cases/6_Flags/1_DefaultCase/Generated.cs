﻿// <auto-generated />
#nullable enable

using PodNet.EnumValues;

public static partial class PermissionsValueExtensions
{
    /// <summary>
    /// Gets the associated <see cref="ValueAttribute"/> flag values for each individual flag, concatenated using <paramref name="separator"/> for the <see cref="Permissions"/> as follows:
    /// <code>
    ///              [Invalid] = -1           => "!"
    ///                 [None] = 0            => "-"
    ///                 [Read] = 1            => "R"
    ///                [Write] = 2            => "W"
    ///              [Execute] = 4            => "X"
    ///          [ReadExecute] = 5            => "RX"
    ///                  [All] = 7            => "A"
    ///
    /// [Invalid | Read | ...] = -1 + 1 + ... => "!R..."
    ///
    ///                   [(int)&lt;flag&gt;] => (int).ToString()
    /// </code>
    /// </summary>
    /// <remarks>Uses <see cref="UndefinedValueHandling.RawValueToString"/> for undefined values.</remarks>
    /// <param name="value">The <see cref="Permissions"/> value.</param>
    /// <returns>A constant, deterministic string value representing the given <paramref name="value"/>.</returns>
    /// Type: int
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the provided <paramref name="value"/> is negative and has no corresponding [<see cref="ValueAttribute"/>] defined to it.</exception>
    public static string GetValue(this Permissions value, string? separator = "")
    {
        var iValue = (int)value;
        if ((separator == "" || ((iValue & (iValue - 1))) == 0) && GetSingleFlagValueOrNull(value) is { } shortCircuitStringValue)
            return shortCircuitStringValue;
        if (iValue < 0)
            throw new ArgumentOutOfRangeException(nameof(value), value, "Enum value was negative.");
        if (iValue == 0)
            return "-";
        var flags = new System.Text.StringBuilder();
        var hasSeparator = !string.IsNullOrEmpty(separator);
        var addedSeparator = false;
        for (int i = 1; i <= iValue; i *= 2)
        {
            if ((i & iValue) != 0)
            {
                if (GetSingleFlagValueOrNull((Permissions)i) is { } stringValue)
                    flags.Append(stringValue);
                else
                    flags.Append(i);
                if (hasSeparator)
                {
                    flags.Append(separator);
                    addedSeparator = true;
                }
            }
        }
        if (addedSeparator)
            return flags.ToString(0, flags.Length - separator!.Length);
        return flags.ToString();

        static string? GetSingleFlagValueOrNull(Permissions value) => value switch
        {
            Permissions.Invalid => "!",
            Permissions.None => "-",
            Permissions.Read => "R",
            Permissions.Write => "W",
            Permissions.Execute => "X",
            Permissions.ReadExecute => "RX",
            Permissions.All => "A",
            _ => null
        };
    }
}
#nullable restore