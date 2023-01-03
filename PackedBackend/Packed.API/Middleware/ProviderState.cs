// Date Created: 2023/01/03
// Created by: JSW

namespace Packed.API.Middleware;

/// <summary>
/// Class representing provider state as it appears in requests
/// </summary>
public class ProviderState
{
    /// <summary>
    /// String key of the state
    /// </summary>
    public string State { get; set; } = null!;

    /// <summary>
    /// Parameters
    /// </summary>
    public IDictionary<string, string> Params { get; set; } = null!;
}