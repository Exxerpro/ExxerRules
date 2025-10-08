namespace IndTrace.Oee.Tests;
/// <summary>
/// Represents the FakeHttpMessageHandler.
/// </summary>

public class FakeHttpMessageHandler : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StringContent("OK")
        };
        return Task.FromResult(response);
    }
}
