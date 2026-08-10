# Contrato OpenAPI

Durante a execução em `Development`, o ASP.NET Core gera o contrato em:

- `http://localhost:5226/openapi/v1.json`
- `http://localhost:5226/scalar/v1`

O projeto começa em abordagem **code-first**: os controllers, DTOs e metadados de resposta geram o documento. Como próximo exercício, crie aqui uma especificação `openapi.yaml` equivalente e compare os dois contratos para identificar possíveis diferenças.
