/*
 * Zmanim .NET API
 * Copyright © 2004-2026 Eliyahu Hershfeld
 *
 * This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General
 * Public License as published by the Free Software Foundation; version 2.1 of the License.
 */
using Xunit;

namespace Kosher.Zmanim.Tests;

/// <summary>
/// Assertions with the signatures of JUnit 4's <c>org.junit.Assert</c> (optional leading message, <c>expected</c> before
/// <c>actual</c>, a <c>delta</c> for doubles), implemented with xUnit. The tests ported from KosherJava use these so they read
/// line for line like the Java originals, which keeps it easy to bring over future KosherJava test changes.
/// </summary>
internal static class JUnitAssert
{
    public static void AssertEquals<T>(T expected, T actual) => Assert.Equal(expected, actual);

    public static void AssertEquals<T>(string message, T expected, T actual)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            Assert.Fail($"{message} ==> expected: <{expected}> but was: <{actual}>");
        }
    }

    public static void AssertEquals(double expected, double actual, double delta) => AssertEquals(null, expected, actual, delta);

    public static void AssertEquals(string? message, double expected, double actual, double delta)
    {
        if (expected.Equals(actual))
        {
            return; // also covers NaN == NaN, as in JUnit
        }
        if (!(Math.Abs(expected - actual) <= delta))
        {
            Assert.Fail($"{message} ==> expected: <{expected:R}> but was: <{actual:R}> (delta {delta})");
        }
    }

    public static void AssertNotEquals<T>(T unexpected, T actual) => Assert.NotEqual(unexpected, actual);

    public static void AssertNotEquals<T>(string message, T unexpected, T actual)
    {
        if (EqualityComparer<T>.Default.Equals(unexpected, actual))
        {
            Assert.Fail($"{message} ==> values should be different. Actual: <{actual}>");
        }
    }

    public static void AssertTrue(bool condition) => Assert.True(condition);

    public static void AssertTrue(string message, bool condition) => Assert.True(condition, message);

    public static void AssertFalse(bool condition) => Assert.False(condition);

    public static void AssertFalse(string message, bool condition) => Assert.False(condition, message);

    public static void AssertNull(object? value) => Assert.Null(value);

    public static void AssertNull(string message, object? value)
    {
        if (value != null)
        {
            Assert.Fail($"{message} ==> expected null but was: <{value}>");
        }
    }

    public static void AssertNotNull(object? value) => Assert.NotNull(value);

    public static void AssertNotNull(string message, object? value)
    {
        if (value == null)
        {
            Assert.Fail(message);
        }
    }

    public static void AssertSame(object? expected, object? actual) => Assert.Same(expected, actual);

    public static void AssertNotSame(object? unexpected, object? actual) => Assert.NotSame(unexpected, actual);

    public static void AssertNotSame(string message, object? unexpected, object? actual)
    {
        if (ReferenceEquals(unexpected, actual))
        {
            Assert.Fail(message);
        }
    }

    public static void Fail(string message) => Assert.Fail(message);
}
