# Prueba Técnica Backend - Abank

API REST para la gestión de usuarios y el control de acceso a la aplicación, desarrollada como solución a la prueba técnica para desarrollador Backend de Abank.

## Descripción

El servicio expone operaciones de autenticación (login con emisión de JWT) y un CRUD de usuarios con borrado lógico. La persistencia se realiza sobre PostgreSQL mediante Dapper. El proyecto está organizado en capas dentro de un único proyecto (Controllers -> Services -> Data), donde cada capa depende de la siguiente únicamente a través de interfaces.

## Stack tecnológico

- .NET 8 (ASP.NET Core Web API)
- PostgreSQL
- Dapper (acceso a datos)
- JWT (autenticación mediante Bearer token)
- BCrypt (hash de contraseñas)
- FluentValidation (validación de entrada)
- Swagger / OpenAPI (documentación y pruebas)
- xUnit y Moq (pruebas unitarias)
- Docker y Docker Compose (empaquetado y orquestación)

## Estructura del repositorio

```
abank-prueba-tecnica/
├── abank-prueba-tecnica-backend/     (Backend)
│   ├── Abank.Api/                    Proyecto de la API
│   │   ├── Common/                   Excepciones de dominio
│   │   ├── Controllers/              AuthController, UsuariosController
│   │   ├── Data/                     Repositorio Dapper e interfaz
│   │   ├── Middleware/               Manejo global de excepciones (ProblemDetails)
│   │   ├── Models/                   Entidad, DTOs, validadores, JwtSettings
│   │   ├── Services/                 Servicios, mapeos, hasher y generador de JWT
│   │   └── Program.cs                Composición y configuración
│   ├── tests/Abank.Tests/            Pruebas unitarias (xUnit + Moq)
│   └── Dockerfile
├── abank-prueba-tecnica-db/          (Base de datos)
│   └── init.sql                      Objetos (tabla, secuencia, triggers) y datos de prueba
├── abank-prueba-tecnica-front/       (Front)
└── docker-compose.yml                Orquestación de base de datos y API
```

## Requisitos previos y descargas

Según la opción de ejecución elegida se necesitan unas u otras herramientas. Si no se dispone de alguna, puede descargarse desde los enlaces siguientes:

| Herramienta        | Uso                                             | Descarga |
|--------------------|-------------------------------------------------|----------|
| Docker Desktop     | Opción A (recomendada): levanta BD y API.       | https://www.docker.com/products/docker-desktop/ |
| .NET 8 SDK         | Opción B: compilar y ejecutar la API localmente.| https://dotnet.microsoft.com/download/dotnet/8.0 |
| PostgreSQL (incluye psql) | Opción B: base de datos y ejecución de `init.sql`. | https://www.postgresql.org/download/ |
| pgAdmin (opcional) | Administración gráfica de PostgreSQL.           | https://www.pgadmin.org/download/ |
| Git (opcional)     | Clonar el repositorio.                           | https://git-scm.com/downloads |

Con la Opción A solo se requiere Docker Desktop. Con la Opción B se requieren el .NET 8 SDK y PostgreSQL; `psql` se instala junto con PostgreSQL.

## Cómo ejecutar

### Opción A: Docker Compose (recomendada)

Requiere Docker Desktop. Levanta la base de datos y la API, e inicializa el esquema y los datos de prueba automáticamente.

```bash
cd abank-prueba-tecnica
docker compose up --build
```

- API y Swagger: http://localhost:8080/swagger
- PostgreSQL: localhost:5432 (base `abank_db`, usuario `postgres`, contraseña `postgres`)

Para reiniciar la base de datos desde cero (por ejemplo, tras modificar los scripts SQL):

```bash
docker compose down -v
docker compose up --build
```

> Nota: los scripts de inicialización solo se ejecutan la primera vez que se crea el volumen de datos de PostgreSQL.

### Opción B: Ejecución local con .NET SDK

Requiere .NET 8 SDK y una instancia de PostgreSQL en ejecución.

1. Crear la base de datos `abank_db` y ejecutar `init.sql` sobre ella.

   En pgAdmin: clic derecho en Databases, Create, Database, nombre `abank_db`, Save. Luego abrir el Query Tool conectado a `abank_db`, cargar `init.sql` y ejecutarlo (F5).

   Alternativa por consola:

   ```bash
   psql -U postgres -c "CREATE DATABASE abank_db;"
   psql -U postgres -d abank_db -f abank-prueba-tecnica-db/init.sql
   ```

2. Configurar los secretos (ver sección Configuración).

3. Ejecutar la API:

   ```bash
   dotnet run --project abank-prueba-tecnica-backend/Abank.Api
   ```

   Swagger quedará disponible en la URL indicada por la consola (por defecto `http://localhost:5080/swagger`).

## Configuración

Por seguridad, `appsettings.json` no contiene secretos: la clave de firma del JWT y la cadena de conexión se dejan vacías y deben proveerse por configuración externa. La aplicación falla al arrancar (fail-fast) si faltan.

- Con Docker Compose: se inyectan como variables de entorno (ya definidas en `docker-compose.yml`), no requiere acción.
- En ejecución local: se recomienda User Secrets.

```bash
cd abank-prueba-tecnica-backend/Abank.Api
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=abank_db;Username=postgres;Password=postgres"
dotnet user-secrets set "Jwt:Key" "AbankPruebaTecnica_ClaveSuperSecreta_2026_MinimoDe32Bytes!"
```

Alternativamente, mediante variables de entorno (doble guion bajo como separador de secciones):

```bash
ConnectionStrings__Default="Host=localhost;Port=5432;Database=abank_db;Username=postgres;Password=postgres"
Jwt__Key="AbankPruebaTecnica_ClaveSuperSecreta_2026_MinimoDe32Bytes!"
```

La clave del JWT debe tener al menos 32 bytes (requisito de HS256), de lo contrario la aplicación no inicia.

## Base de datos

El script `init.sql` es SQL estándar (compatible con el Query Tool de pgAdmin, DBeaver y psql) y debe ejecutarse conectado a la base `abank_db`. Crea:

- Una secuencia (`usuarios_id_seq`) para el identificador autogenerado.
- La tabla `usuarios` con los campos solicitados.
- Una restricción CHECK que limita el campo `estado` a los valores `A` (activo) o `I` (inactivo).
- Restricciones de unicidad para `telefono` y `email`.
- Un trigger que asigna `fecha_creacion` en la inserción y `fecha_modificacion` en la actualización.
- La inserción de usuarios de prueba cuyas contraseñas se almacenan como hash BCrypt.

La base `abank_db` debe crearse antes de ejecutar el script (un paso en pgAdmin o una sentencia `CREATE DATABASE abank_db;`). Con Docker, la base, el esquema y los datos se crean automáticamente al iniciar el contenedor, sin pasos manuales.

## Endpoints

| Método | Ruta                      | Protegido | Descripción                               |
|--------|---------------------------|-----------|-------------------------------------------|
| POST   | `/api/v1/auth/login`      | No        | Autentica por teléfono y contraseña; devuelve el JWT. |
| GET    | `/api/v1/users`           | No        | Lista los usuarios activos.               |
| GET    | `/api/v1/users/{id}`      | Sí        | Obtiene un usuario por identificador.     |
| POST   | `/api/v1/users`           | Sí        | Crea un usuario. Devuelve 201 Created.    |
| PUT    | `/api/v1/users/{id}`      | Sí        | Actualiza un usuario.                     |
| DELETE | `/api/v1/users/{id}`      | Sí        | Borrado lógico (cambia `estado` a `I`).   |

## Credenciales de prueba

| Teléfono | Contraseña  |
|----------|-------------|
| 70001111 | Abank123*   |
| 70002222 | Abank123*   |

## Autenticación

1. Invocar `POST /api/v1/auth/login` con un teléfono y contraseña válidos.
2. Copiar el valor de `access_token` de la respuesta.
3. En Swagger, usar el botón Authorize y pegar únicamente el token (sin el prefijo `Bearer`).
4. Invocar los endpoints protegidos.

## Pruebas

```bash
cd abank-prueba-tecnica-backend
dotnet test
```

Las pruebas unitarias cubren los validadores, los mapeos, el servicio de autenticación, el servicio de usuarios (con dependencias simuladas mediante Moq) y el hasher de contraseñas.

## Cumplimiento de los requisitos

| Requisito                                             | Estado    |
|-------------------------------------------------------|-----------|
| API REST en .NET 8                                    | Cumplido  |
| Autenticación con JWT                                 | Cumplido  |
| Base de datos PostgreSQL                              | Cumplido  |
| Persistencia con Dapper                               | Cumplido  |
| Id autogenerado por secuencia                         | Cumplido  |
| Restricción de estado a `A` / `I`                     | Cumplido  |
| Fechas de creación y modificación por trigger         | Cumplido  |
| Endpoints solicitados con protección por token        | Cumplido  |
| Documentación del proyecto (este documento)           | Cumplido  |
| Instrucciones para ejecutar y probar                  | Cumplido  |
| Configuración de Docker                               | Cumplido  |
| Pruebas unitarias                                     | Cumplido  |

## Decisiones de diseño

- Las contraseñas nunca se exponen en las respuestas de la API; se almacenan como hash BCrypt.
- El manejo de errores es centralizado y devuelve respuestas en formato ProblemDetails (RFC 7807).
- El borrado es lógico: los registros no se eliminan físicamente, se marcan con `estado = 'I'`.
- La validación de entrada se realiza con FluentValidation, reutilizando reglas comunes entre creación y actualización.
- Los secretos no se versionan; se gestionan mediante User Secrets en desarrollo y variables de entorno en despliegue.
