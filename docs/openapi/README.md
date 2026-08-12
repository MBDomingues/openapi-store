# Contrato OpenAPI

A aplicação usa abordagem **code-first** e publica um documento OpenAPI 3.1 durante a execução em `Development`.

- JSON: <http://localhost:5226/openapi/v1.json>
- Scalar: <http://localhost:5226/scalar/v1>

## Origem dos metadados

- Controllers, `HttpGet`, `HttpPost` e `HttpPut` definem paths e métodos.
- Route constraints como `{id:guid}` descrevem parâmetros e formatos.
- Assinaturas e DTOs descrevem request bodies e schemas.
- DataAnnotations descrevem propriedades obrigatórias e limites.
- `ProducesResponseType` descreve status codes e schemas de resposta.
- O `Name` dos atributos de rota é usado como `operationId`.

`AddOpenApi` reúne esses metadados, `MapOpenApi` publica o documento e o Scalar apenas busca e apresenta esse documento. Alterar o arquivo `.http` não modifica o contrato.

## Fonte de verdade atual

Nesta versão, o código é a fonte de verdade. Um contrato manual `openapi.yaml` poderá ser adicionado posteriormente para comparar code-first e contract-first. Se os dois forem mantidos ao mesmo tempo, será necessário definir explicitamente qual é autoritativo e automatizar a detecção de divergências.
