# OpenAPI Practice API

API REST pequena para cadastro de produtos, construída em C# e .NET 10 para exercitar design de contratos HTTP, OpenAPI 3.1 e documentação interativa.

## Características

- ASP.NET Core Web API com Controllers.
- .NET 10 e nullable reference types habilitados.
- Contrato OpenAPI 3.1 gerado em abordagem code-first.
- Documentação e cliente interativo com Scalar.
- DTOs separados do modelo interno.
- Validação de entrada com DataAnnotations.
- Erros de validação no formato `ValidationProblemDetails`.
- Recursos inexistentes no formato `ProblemDetails`.
- Armazenamento temporário em memória com `ConcurrentDictionary`.
- Requisições manuais reproduzíveis em arquivo `.http`.

## Funcionalidades da API

| Método | Rota | Sucesso | Falhas documentadas | Descrição |
| --- | --- | --- | --- | --- |
| `GET` | `/api/products` | `200` | — | Lista produtos ordenados por nome. |
| `GET` | `/api/products/{id}` | `200` | `404` | Busca um produto por `Guid`. |
| `POST` | `/api/products` | `201` | `400` | Cria um produto e informa sua URL no header `Location`. |
| `PUT` | `/api/products/{id}` | `200` | `400`, `404` | Atualiza nome e preço preservando ID e data de criação. |

## Contratos e validações

Os endpoints não recebem nem devolvem diretamente o modelo interno `Product`:

- `CreateProductRequest`: body usado na criação;
- `UpdateProductRequest`: body usado na atualização;
- `ProductResponse`: representação pública devolvida ao consumidor;
- `ValidationProblemDetails`: falhas de validação;
- `ProblemDetails`: erros HTTP como produto inexistente.

Regras atuais de `name` e `price`:

| Campo | Regra |
| --- | --- |
| `name` | Obrigatório, normalizado com `Trim()`, entre 3 e 100 caracteres. |
| `price` | Entre `0.01` e `999999.99`. |

O atributo `[ApiController]` interrompe automaticamente a requisição inválida e devolve `400`, antes de executar a action. Os atributos `ProducesResponseType` documentam os status e schemas possíveis; os retornos da action, como `Ok`, `CreatedAtAction` e `NotFound`, implementam o comportamento em runtime.

## OpenAPI e Scalar

O contrato é produzido a partir de metadados complementares:

```text
controllers, rotas e verbos
        +
assinaturas, DTOs e DataAnnotations
        +
tipos/status declarados em ProducesResponseType
        ↓
documento /openapi/v1.json
        ↓
Scalar /scalar/v1
```

`AddOpenApi` registra a geração do documento, `MapOpenApi` publica o JSON e `MapScalarApiReference` disponibiliza a interface. O Scalar não analisa os controllers diretamente: ele lê e renderiza o documento OpenAPI publicado pela aplicação.

Mais detalhes estão em [docs/openapi/README.md](docs/openapi/README.md).

## Estrutura

```text
openapi-practice-api/
├── docs/openapi/                  # documentação e especificações OpenAPI
├── src/OpenApiPractice.Api/
│   ├── Contracts/Products/        # contratos HTTP de entrada e saída
│   ├── Controllers/               # rotas, status e tradução HTTP
│   ├── Models/                    # modelo interno
│   ├── Services/                  # comportamento e armazenamento em memória
│   ├── OpenApiPractice.Api.http   # requisições manuais
│   └── Program.cs                 # DI e pipeline OpenAPI/Scalar
└── OpenApiPractice.slnx
```

Não há `Repository`, EF Core ou banco de dados nesta versão. O armazenamento em memória é uma escolha intencional para manter o foco no contrato HTTP; todos os dados voltam ao estado inicial quando a aplicação reinicia.

## Executar

Requisito: .NET SDK 10.

```bash
dotnet restore
ASPNETCORE_ENVIRONMENT=Development \
ASPNETCORE_URLS=http://0.0.0.0:5226 \
dotnet run --project src/OpenApiPractice.Api
```

Com a aplicação em execução, os recursos locais ficam disponíveis em:

- API: <http://localhost:5226/api/products>
- OpenAPI JSON: <http://localhost:5226/openapi/v1.json>
- Scalar: <http://localhost:5226/scalar/v1>

## Executar requisições manuais

Abra `src/OpenApiPractice.Api/OpenApiPractice.Api.http` com uma extensão compatível, como REST Client no VS Code, e use **Send Request** no bloco desejado.

Cada `###` separa uma requisição. As variáveis declaradas no topo, como `@host` e `@existingProductId`, são reutilizadas por meio da sintaxe `{{nome}}`.

O arquivo `.http` é um cliente manual: ele não registra endpoints, não gera o documento OpenAPI e não alimenta o Scalar.

## Limitações atuais

- Dados não persistem após reiniciar a API.
- Não há autenticação ou autorização.
- Não há paginação, filtros ou ordenação configurável.
- Não há suíte de testes automatizados.
- A documentação OpenAPI é exposta somente no ambiente `Development`.
