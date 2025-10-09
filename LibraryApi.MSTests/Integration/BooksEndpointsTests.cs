using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using LibraryApi.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LibraryApi.MSTests.Integration;

[TestClass]
public class BooksEndpointsTests
{
    private static CustomWebApplicationFactory _factory = null!;
    private static HttpClient _client = null!;

    [ClassInitialize]
    public static void Setup(TestContext _)
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost") // evita 307 por UseHttpsRedirection
        });
    }

    [ClassCleanup]
    public static void Teardown()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    [TestMethod]
    public async Task Create_Then_GetById_Works()
    {
        var create = await _client.PostAsJsonAsync("/api/books", new BookCreateDto
        {
            Title = $"Integration Book {Guid.NewGuid():N}",
            Author = "Tester"
        });
        Assert.AreEqual(HttpStatusCode.Created, create.StatusCode, await create.Content.ReadAsStringAsync());

        // Sigue Location de CreatedAtAction (robusto)
        var location = create.Headers.Location;
        Assert.IsNotNull(location);

        var get = await _client.GetAsync(location);
        Assert.AreEqual(HttpStatusCode.OK, get.StatusCode, await get.Content.ReadAsStringAsync());
    }
}
