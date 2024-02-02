using System.Collections;
using System.Collections.Immutable;

namespace PodNet.EnumValues.Equality;

/// <summary>
/// A simple struct that holds an <see cref="ImmutableArray{T}"/> instance and implements value equality over its items using sequence equality.
/// </summary>
/// <typeparam name="T">The type of items in the array.</typeparam>
/// <param name="array">The array to wrap.</param>
public readonly struct EquatableArray<T>(ImmutableArray<T> array) : IEquatable<EquatableArray<T>>, IEnumerable<T>
{
    public ImmutableArray<T> Values { get; } = array.IsDefaultOrEmpty ? ImmutableArray<T>.Empty : array;
    public bool Equals(EquatableArray<T> other) => Values.SequenceEqual(other.Values);
    public override bool Equals(object obj) => obj is EquatableArray<T> equatableArray && Equals(equatableArray);
    public override int GetHashCode() => Values.Aggregate(0x5bd1e995, (acc, v) => (acc >> 17 | acc << sizeof(int) - 17) ^ (v?.GetHashCode() ?? 0));
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)Values).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Values).GetEnumerator();

    public static implicit operator ImmutableArray<T>(EquatableArray<T> array) => array.Values;
    public static implicit operator EquatableArray<T>(ImmutableArray<T> array) => new(array);
    public ImmutableArray<T> ToImmutableArray() => Values;
    public EquatableArray<T> ToEquatableArray() => new(Values);
    public static bool operator ==(EquatableArray<T> left, EquatableArray<T> right) => left.Equals(right);
    public static bool operator !=(EquatableArray<T> left, EquatableArray<T> right) => !(left == right);
}