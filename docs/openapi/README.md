# OpenAPI Contract

The application uses a **code-first** approach and publishes an OpenAPI 3.1 document while running in `Development`.

- JSON: <http://localhost:5226/openapi/v1.json>
- Scalar: <http://localhost:5226/scalar/v1>

## Metadata sources

- Controllers, `HttpGet`, `HttpPost` and `HttpPut` define paths and methods.
- Route constraints such as `{id:guid}` describe parameters and formats.
- Signatures and DTOs describe request bodies and schemas.
- DataAnnotations describe required properties and limits.
- `ProducesResponseType` describes status codes and response schemas.
- The `Name` of the route attributes is used as the `operationId`.

`AddOpenApi` gathers this metadata, `MapOpenApi` publishes the document and Scalar only fetches and displays that document. Changing the `.http` file does not modify the contract.

## Current source of truth

In the running application, the code still generates `/openapi/v1.json`. During exercise 003, the manual [`openapi.yaml`](openapi.yaml) file is treated as the source of truth to practice contract-first and reveal differences the generator cannot infer.

The observed comparison is in [`COMPARISON.md`](COMPARISON.md), and the step-by-step explanation is in [`EXERCISE_003_GUIDE.md`](EXERCISE_003_GUIDE.md). If both contracts are kept after the exercise, it will be necessary to choose a single authoritative document and automate divergence detection.
