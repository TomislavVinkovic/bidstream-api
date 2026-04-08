using System.Net;
using FluentAssertions;

namespace BidStream.Tests.Extensions;

public static class HttpResponseExtensions
{
    public static async Task ShouldBeOkAsync(this HttpResponseMessage response)
    {
        if (response.StatusCode == HttpStatusCode.OK) return;

        var errorBody = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, 
            because: $"the API returned an error.\n\nError Body:\n{errorBody}\n");
    }
}