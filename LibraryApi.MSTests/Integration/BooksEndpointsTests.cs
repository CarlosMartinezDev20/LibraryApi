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
    // WebApplicationFactory arrancará la app ASP.NET Core en memoria para probar los endpoints reales.
    private static CustomWebApplicationFactory _factory = null!;
    // Cliente HTTP contra el servidor en memoria.
    private static HttpClient _client = null!;

    // Se ejecuta UNA VEZ antes de todos los tests de esta clase.
    [ClassInitialize]
    public static void Setup(TestContext _)
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            // Hacemos que la base sea https://localhost para evitar un 307 por UseHttpsRedirection.
            BaseAddress = new Uri("https://localhost")
        });
    }

    // Se ejecuta UNA VEZ después de todos los tests de esta clase.
    [ClassCleanup]
    public static void Teardown()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    // crear un libro y luego leerlo por su Location.
    [TestMethod]
    public async Task Create_Then_GetById_Works()
    {
        // 1) POST /api/books con un título único (Guid) para no colisionar entre ejecuciones.
        var create = await _client.PostAsJsonAsync("/api/books", new BookCreateDto
        {
            Title = $"Integration Book {Guid.NewGuid():N}",
            Author = "Tester"
        });

        // Esperamos 201 Created (creación correcta).
        Assert.AreEqual(HttpStatusCode.Created, create.StatusCode, await create.Content.ReadAsStringAsync());

        // 2) Leemos la cabecera Location que devuelve CreatedAtAction (más robusto que inferir la URL).
        var location = create.Headers.Location;
        Assert.IsNotNull(location);

        // 3) GET al recurso recién creado. Debe devolver 200 OK.
        var get = await _client.GetAsync(location);
        Assert.AreEqual(HttpStatusCode.OK, get.StatusCode, await get.Content.ReadAsStringAsync());
    }
}
