namespace PodNet.EnumValues.Integrals;

internal static class IntegralValues
{
    public static bool IsZeroEnumValue(object? boxedValue)
        => boxedValue is (byte)0 or (sbyte)0 or (short)0 or (ushort)0 or 0 or 0U or 0L or 0UL;

    public static bool IsFlagCandidate(object? boxedValue)
        => boxedValue switch
        {
            null => false,
            byte v => (v & v - 1) == 0,
            sbyte v => (v & v - 1) == 0,
            short v => (v & v - 1) == 0,
            ushort v => (v & v - 1) == 0,
            int v => (v & v - 1) == 0,
            uint v => (v & v - 1) == 0,
            long v => (v & v - 1) == 0,
            ulong v => (v & v - 1) == 0,
            _ => throw new ArgumentException($"Unknown type of boxed integral: {boxedValue.GetType()} ({boxedValue})", nameof(boxedValue))
        };

    public static int Compare(ulong value, object? boxedValue)
        => boxedValue switch
        {
            null => value.CompareTo(null),
            byte v => value.CompareTo(v),
            ushort v => value.CompareTo(v),
            uint v => value.CompareTo(v),
            ulong v => value.CompareTo(v),
            sbyte v => v < 0 ? 1 : value.CompareTo((ulong)v),
            short v => v < 0 ? 1 : value.CompareTo((ulong)v),
            int v => v < 0 ? 1 : value.CompareTo((ulong)v),
            long v => v < 0 ? 1 : value.CompareTo((ulong)v),
            _ => throw new ArgumentException($"Unknown type of boxed integral: {boxedValue.GetType()} ({boxedValue})", nameof(boxedValue))
        };

    public sealed class EnumFlagValueComparer : IEqualityComparer<object?>
    {
        private EnumFlagValueComparer() { }
        public static EnumFlagValueComparer Instance { get; } = new();
        public new bool Equals(object? x, object? y) => object.Equals(x, y) || x is ulong u && Compare(u, y) == 0;
        public int GetHashCode(object? obj) => obj?.GetHashCode() ?? 0;
    }
}