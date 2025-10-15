using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using LibraryApi.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LibraryApi.MSTests.Integration;

[TestClass]
public class UsersEndpointsTests
{
    private static CustomWebApplicationFactory _factory = null!;
    private static HttpClient _client = null!;

    [ClassInitialize]
    public static void Setup(TestContext _)
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
    }

    [ClassCleanup]
    public static void Teardown()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    //  registrar un usuario y esperar 201.
    [TestMethod]
    public async Task Register_Returns_201()
    {
        // Email único para evitar 409 entre ejecuciones.
        var dto = new UserRegisterDto
        {
            FullName = "Carlos Pruebas",
            Email = $"carlos.{Guid.NewGuid():N}@example.com",
            Password = "Secreto123"
        };

        var resp = await _client.PostAsJsonAsync("/api/users/register", dto);

        // Debe devolver 201 Created si el registro es correcto.
        Assert.AreEqual(HttpStatusCode.Created, resp.StatusCode, await resp.Content.ReadAsStringAsync());
    }
}
