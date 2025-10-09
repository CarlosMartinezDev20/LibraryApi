# 📚 LibraryApi — API REST + Pruebas MSTest

Proyecto completo desarrollado con **.NET 8**, que implementa una **API REST** para la gestión de usuarios, libros y préstamos dentro de una biblioteca.  
Incluye un módulo de **pruebas unitarias e integración (MSTest)** para asegurar la calidad del sistema.

<p align="center">
  <img src="https://img.shields.io/badge/.NET-512BD4.svg?style=flat&logo=dotnet&logoColor=white" alt=".NET" />
  <img src="https://img.shields.io/badge/C%23-239120.svg?style=flat&logo=csharp&logoColor=white" alt="C#" />
  <img src="https://img.shields.io/badge/MSTest-0078D7.svg?style=flat&logo=visualstudio&logoColor=white" alt="MSTest" />
  <img src="https://img.shields.io/badge/SQLite-07405E.svg?style=flat&logo=sqlite&logoColor=white" alt="SQLite" />
  <img src="https://img.shields.io/badge/Entity_Framework_Core-512BD4?style=flat&logo=nuget&logoColor=white" alt="EF Core" />
</p>

---

## 🧠 Tecnologías utilizadas
- C# (.NET 8.0)
- ASP.NET Core Web API
- Entity Framework Core (Pomelo MySQL + SQLite InMemory)
- MSTest (Framework de pruebas)
- Microsoft.CodeCoverage (Cobertura de código)
- Swagger / OpenAPI

---

## 🧩 Estructura del proyecto

```
LibraryApi/
 ├── Controllers/
 ├── Data/
 ├── DTOs/
 ├── Models/
 ├── Migrations/
 ├── Program.cs
 ├── Program.TestVisibility.cs
 └── appsettings.json

LibraryApi.MSTests/
 ├── Integration/
 │   ├── BooksEndpointsTests.cs
 │   ├── UsersEndpointsTests.cs
 │   └── CustomWebApplicationFactory.cs
 ├── Unit/
 │   ├── BooksControllerTests.cs
 │   ├── LoansControllerTests.cs
 │   └── UsersControllerTests.cs
 └── MSTestSettings.cs
```

---

## ⚙️ Configuración y ejecución de la API

```bash
# 1. Clonar el repositorio
git clone https://github.com/tuusuario/LibraryApi.git

# 2. Restaurar dependencias
dotnet restore

# 3. Aplicar migraciones (si usas MySQL local)
dotnet ef database update

# 4. Ejecutar la API
dotnet run --project LibraryApi

# 5. Acceder a Swagger
https://localhost:7643/swagger/index.html
```

---

## 🚀 Endpoints principales

### 👤 Usuarios
| Método | Endpoint | Descripción |
|--------|-----------|-------------|
| POST | `/api/users/register` | Registrar usuario |
| PUT | `/api/users/{id}` | Actualizar usuario |
| DELETE | `/api/users/{id}` | Borrado lógico |
| GET | `/api/users` | Listar usuarios activos |
| GET | `/api/users/{id}` | Obtener usuario por ID |

### 📘 Libros
| Método | Endpoint | Descripción |
|--------|-----------|-------------|
| POST | `/api/books` | Registrar libro |
| PUT | `/api/books/{id}` | Modificar libro |
| DELETE | `/api/books/{id}` | Eliminar libro |
| GET | `/api/books` | Listar libros |
| GET | `/api/books/{id}` | Obtener libro por ID |
| POST | `/api/books/search` | Buscar por título o autor |

### 📦 Préstamos
| Método | Endpoint | Descripción |
|--------|-----------|-------------|
| POST | `/api/loans` | Registrar préstamo |
| POST | `/api/loans/{id}/return` | Registrar devolución |

---

## 🧪 Ejecución de pruebas MSTest

### 🔹 Desde Visual Studio
1. Abrir la solución `LibraryApi.sln`  
2. Seleccionar el proyecto `LibraryApi.MSTests`  
3. Ir a **Prueba → Ejecutar todas las pruebas** o presionar **Ctrl + R, A**

### 🔹 Desde CLI
```bash
dotnet test LibraryApi.MSTests --logger "console;verbosity=detailed"
```

### 🔹 Con cobertura de código
```bash
dotnet test --collect:"XPlat Code Coverage"
```

---

## 📈 Resultados esperados

```
========== Resultados ==========
Total: 7 pruebas
Exitosas: 7
Fallidas: 0
Omitidas: 0
Duración: ~5 segundos
===============================
```

---

## 💡 Notas importantes
- Usa SQLite InMemory para pruebas de integración.  
- Borrado lógico en controladores (`IsDeleted = true`).  
- Cobertura completa de CRUD y validaciones.  
- Mantén las migraciones actualizadas antes de ejecutar pruebas.

---
> 🧠 *“Las pruebas no garantizan que no haya errores, pero aseguran que los errores no regresen.”*
