// Date Created: 2023/01/04
// Created by: JSW

using Packed.ContractTest.Shared;
using PactNet;

namespace Packed.ContractTest.Consumer;

/// <summary>
/// Base class for all contract tests
/// </summary>
public abstract class ContractTestBase
{
    #region FIELDS

    /// <summary>
    /// Pact builder
    /// </summary>
    protected IPactBuilderV3 PactBuilder = null!;

    #endregion FIELDS

    #region TEST LIFE CYCLE

    /// <summary>
    /// Initialization to be run before each test
    /// </summary>
    [TestInitialize]
    public virtual void Initialize()
    {
        var pact = Pact.V3(ContractInfo.ConsumerName, ContractInfo.ProviderName, new PactConfig
        {
            PactDir = $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}pacts"
        });

        PactBuilder = pact.WithHttpInteractions();
    }

    #endregion TEST LIFE CYCLE
}