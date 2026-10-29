# OpenAPI Practice API

Small REST API for product registration, built in C# and .NET 10 to practice HTTP contract design, OpenAPI 3.1 and interactive documentation.

## Features

- ASP.NET Core Web API with Controllers.
- .NET 10 with nullable reference types enabled.
- OpenAPI 3.1 contract generated with a code-first approach.
- Interactive documentation and client with Scalar.
- DTOs separated from the internal model.
- Input validation with DataAnnotations.
- Validation errors in the `ValidationProblemDetails` format.
- Nonexistent resources in the `ProblemDetails` format.
- Temporary in-memory storage with `ConcurrentDictionary`.
- Reproducible manual requests in a `.http` file.

## API features

| Method | Route | Success | Documented failures | Description |
| --- | --- | --- | --- | --- |
| `GET` | `/api/products` | `200` | — | Lists products sorted by name. |
| `GET` | `/api/products/{id}` | `200` | `404` | Fetches a product by `Guid`. |
| `POST` | `/api/products` | `201` | `400` | Creates a product and returns its URL in the `Location` header. |
| `PUT` | `/api/products/{id}` | `200` | `400`, `404` | Updates name and price while preserving ID and creation date. |

## Contracts and validations

The endpoints neither receive nor return the internal `Product` model directly:

- `CreateProductRequest`: body used on creation;
- `UpdateProductRequest`: body used on update;
- `ProductResponse`: public representation returned to the consumer;
- `ValidationProblemDetails`: validation failures;
- `ProblemDetails`: HTTP errors such as a nonexistent product.

Current rules for `name` and `price`:

| Field | Rule |
| --- | --- |
| `name` | Required, normalized with `Trim()`, between 3 and 100 characters. |
| `price` | Between `0.01` and `999999.99`. |

The `[ApiController]` attribute automatically stops the invalid request and returns `400` before running the action. The `ProducesResponseType` attributes document the possible statuses and schemas; the action's return values, such as `Ok`, `CreatedAtAction` and `NotFound`, implement the behavior at runtime.

## OpenAPI and Scalar

The contract is produced from complementary metadata:

```text
controllers, routes and verbs
        +
signatures, DTOs and DataAnnotations
        +
types/statuses declared in ProducesResponseType
        ↓
/openapi/v1.json document
        ↓
Scalar /scalar/v1
```

`AddOpenApi` registers document generation, `MapOpenApi` publishes the JSON and `MapScalarApiReference` makes the interface available. Scalar does not analyze the controllers directly: it reads and renders the OpenAPI document published by the application.

More details are in [docs/openapi/README.md](docs/openapi/README.md).

## Structure

```text
openapi-practice-api/
├── docs/openapi/                  # OpenAPI documentation and specifications
├── src/OpenApiPractice.Api/
│   ├── Contracts/Products/        # input and output HTTP contracts
│   ├── Controllers/               # routes, statuses and HTTP translation
│   ├── Models/                    # internal model
│   ├── Services/                  # behavior and in-memory storage
│   ├── OpenApiPractice.Api.http   # manual requests
│   └── Program.cs                 # DI and OpenAPI/Scalar pipeline
└── OpenApiPractice.slnx
```

There is no `Repository`, EF Core or database in this version. In-memory storage is an intentional choice to keep the focus on the HTTP contract; all data returns to its initial state when the application restarts.

## Run

Requirement: .NET SDK 10.

```bash
dotnet restore
ASPNETCORE_ENVIRONMENT=Development \
ASPNETCORE_URLS=http://0.0.0.0:5226 \
dotnet run --project src/OpenApiPractice.Api
```

With the application running, the local resources are available at:

- API: <http://localhost:5226/api/products>
- OpenAPI JSON: <http://localhost:5226/openapi/v1.json>
- Scalar: <http://localhost:5226/scalar/v1>

## Run manual requests

Open `src/OpenApiPractice.Api/OpenApiPractice.Api.http` with a compatible extension, such as REST Client in VS Code, and use **Send Request** on the desired block.

Each `###` separates a request. The variables declared at the top, such as `@host` and `@existingProductId`, are reused through the `{{name}}` syntax.

The `.http` file is a manual client: it does not register endpoints, does not generate the OpenAPI document and does not feed Scalar.

## Current limitations

- Data does not persist after restarting the API.
- There is no authentication or authorization.
- There is no pagination, filtering or configurable sorting.
- There is no automated test suite.
- The OpenAPI documentation is exposed only in the `Development` environment.
