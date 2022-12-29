// Date Created: 2022/12/27
// Created by: JSW

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Packed.API;
using Packed.ContractTest.Shared;
using PactNet.Verifier;

namespace Packed.ContractTest.Provider;

/// <summary>
/// Provider contract tests for Packed API
/// </summary>
[TestClass]
public class PackedApiShould
{
    #region TEST METHODS

    /// <summary>
    /// Test to ensure that contract with consumer is honored
    /// </summary>
    [TestMethod]
    public void HonorPactWithConsumer()
    {
        // Declare server URI
        var serverUri = new Uri("http://localhost:9222");

        // Build server using a special test startup
        var server = Host
            .CreateDefaultBuilder()
            .ConfigureWebHostDefaults(builder =>
            {
                builder.UseUrls(serverUri.ToString());
                builder.UseStartup<ContractTestStartup>();
            }).Build();

        // Start the API
        server.Start();

        // Arrange
        var pactPath =
            $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}pacts{Path.DirectorySeparatorChar}{ContractInfo.ConsumerName}-{ContractInfo.ProviderName}.json";
        var verifier = new PactVerifier(new PactVerifierConfig());

        // Act/Assert
        verifier
            .ServiceProvider(ContractInfo.ProviderName, serverUri)
            .WithFileSource(new FileInfo(pactPath))
            .WithProviderStateUrl(new Uri(serverUri, "/provider-states"))
            .WithRequestTimeout(TimeSpan.FromSeconds(30))
            .WithSslVerificationDisabled()
            .Verify();
    }

    #endregion TEST METHODS
}