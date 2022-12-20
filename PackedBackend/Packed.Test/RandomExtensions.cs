// Date Created: 2022/12/19
// Created by: JSW

namespace Packed.Test;

/// <summary>
/// Extensions to the <see cref="Random"/> class
/// </summary>
public static class RandomExtensions
{
    /// <summary>
    /// Retrieve a random negative integer
    /// </summary>
    /// <param name="random">Random</param>
    /// <returns>
    /// A random negative integer
    /// </returns>
    public static int GetRandomNegativeId(this Random random)
    {
        return random.Next(int.MinValue, 0);
    }
}