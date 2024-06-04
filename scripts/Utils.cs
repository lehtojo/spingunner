using System.Collections.Generic;
using System.Linq;
using System;
using Godot;

public static class Utils
{
    public static float Random()
    {
        return (float)System.Random.Shared.NextDouble();
    }

    public static float Random(float min, float max)
    {
        return (float)System.Random.Shared.NextDouble() * (max - min) + min;
    }

    public static float Random(float max)
    {
        return (float)System.Random.Shared.NextDouble() * max;
    }

    public static int Random(int min, int max)
    {
        return System.Random.Shared.Next(min, max);
    }

    public static int Random(Vector2I range)
    {
        return System.Random.Shared.Next(range.X, range.Y + 1);
    }

    public static float Random(Vector2 range)
    {
        return range.X + (float)System.Random.Shared.NextDouble() * (range.Y - range.X);
    }

    public static int Random(int max)
    {
        return System.Random.Shared.Next(0, max);
    }

    public static Vector2 RandomDirection(float length = 1.0f)
    {
        var angle = Random(0.0f, Mathf.Pi * 2.0f);
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * length;
    }

    public static ulong GetRandomId()
    {
        var buffer = new byte[8];
        var random = System.Security.Cryptography.RandomNumberGenerator.Create();
        random.GetBytes(buffer);
        return BitConverter.ToUInt64(buffer, 0);
    }

    public static T Random<T>(ICollection<T> options)
    {
        var index = Random(0, options.Count);
        return options.ElementAt(index);
    }
}