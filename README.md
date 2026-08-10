# OpenAPI Practice API

API pequena em C#/.NET 10 para praticar contrato HTTP, OpenAPI 3.1 e documentação interativa com Scalar.

## Estrutura

```text
openapi-practice-api/
├── docs/openapi/                  # estudo do contrato e futura spec manual
└── src/OpenApiPractice.Api/
    ├── Controllers/               # traduz HTTP para chamadas da aplicação
    ├── Contracts/Products/        # request e response expostos pela API
    ├── Models/                    # modelo interno do exemplo
    ├── Services/                  # comportamento e armazenamento em memória
    └── Program.cs                 # DI e pipeline HTTP/OpenAPI/Scalar
```

Não há `Repository` nesta primeira versão porque ainda não existe persistência externa. A separação deve aparecer quando houver uma dependência real, como MySQL ou outro serviço.

## Executar

Requisitos: .NET SDK 10.

```bash
dotnet restore
dotnet run --project src/OpenApiPractice.Api
```

Com o perfil HTTP padrão:

- Scalar: <http://localhost:5226/scalar/v1>
- OpenAPI JSON: <http://localhost:5226/openapi/v1.json>
- API: <http://localhost:5226/api/products>

O arquivo `src/OpenApiPractice.Api/OpenApiPractice.Api.http` contém requisições prontas para os caminhos feliz e de validação.

## Exercício principal — evoluir o contrato da API

### Objetivo de aprendizagem

O objetivo não é apenas fazer um `PUT` funcionar. Você deverá entender como uma decisão no código se transforma em um contrato que outro sistema pode consumir:

```text
request HTTP
    → rota e action do controller
    → DTO de entrada e validação
    → service e modelo interno
    → DTO de saída e status code
    → documento OpenAPI
    → interface do Scalar
```

**Nível:** iniciante/intermediário  
**Tempo estimado:** 45–75 minutos  
**Evidência final:** endpoint funcionando, build limpo e contrato conferido no Scalar.

### 1. Conheça o ponto de partida

A API já possui:

| Método | Rota | Comportamento |
| --- | --- | --- |
| `GET` | `/api/products` | Lista produtos |
| `GET` | `/api/products/{id}` | Retorna `200` ou `404` |
| `POST` | `/api/products` | Cria um produto e retorna `201` |

Antes de programar, execute a API e abra o Scalar. Localize as operações em `paths`, os schemas `CreateProductRequest` e `ProductResponse`, as validações de `name` e `price` e os status codes documentados.

Faça uma previsão: **o que deverá aparecer no OpenAPI depois que o `PUT` for implementado?** Guarde essa resposta para comparar no final.

### 2. Sua tarefa

Implemente a atualização completa de um produto:

```http
PUT /api/products/{id}
Content-Type: application/json
```

Body de exemplo:

```json
{
  "name": "Mouse ergonômico",
  "price": 189.90
}
```

O contrato esperado é:

| Cenário | Status | Corpo da resposta |
| --- | --- | --- |
| Produto existe e o body é válido | `200 OK` | `ProductResponse` atualizado |
| Body viola alguma validação | `400 Bad Request` | `ValidationProblemDetails` |
| O `id` é válido, mas não existe | `404 Not Found` | `ProblemDetails` |

Em uma atualização bem-sucedida, altere apenas `name` e `price`. Preserve `id` e `createdAt`. Um identificador inexistente não deve criar um produto novo.

### 3. Onde cada mudança pertence

- `Contracts/Products`: crie o DTO recebido pelo `PUT`.
- `Services/ProductService.cs`: atualize o estado em memória e informe quando o produto não existir.
- `Controllers/ProductsController.cs`: receba a requisição e escolha entre `200` e `404`.
- `OpenApiPractice.Api.http`: adicione exemplos para sucesso, validação e produto inexistente.

O JSON do OpenAPI não deve ser editado manualmente. Ele deve mudar como consequência dos tipos, validações e metadados declarados no código.

### 4. Restrições

- Não adicione banco de dados, EF Core ou persistência em arquivo.
- Não crie `Repository`, interface ou design pattern para esta etapa.
- Não instale novos pacotes.
- Mantenha o `404` consistente com o `GET /api/products/{id}`.

As restrições mantêm o foco em uma única dificuldade: projetar e documentar uma operação de atualização.

### 5. Ordem sugerida

1. Defina os três comportamentos antes de escrever código.
2. Crie o DTO e suas regras de validação.
3. Implemente a atualização no service, preservando `id` e `createdAt`.
4. Adicione a action `PUT` e documente suas respostas.
5. Teste o caminho feliz e depois cada falha.
6. Abra o Scalar e compare o resultado com sua previsão.

### 6. Testes manuais obrigatórios

Use o produto inicial `11111111-1111-1111-1111-111111111111` no caminho feliz.

1. Atualização válida retorna `200` e os novos valores.
2. Um `GET` posterior mostra os valores atualizados.
3. Um `Guid` inexistente retorna `404`.
4. `name` com menos de três caracteres retorna `400`.
5. `price` igual a zero retorna `400`.
6. A resposta de sucesso preserva o `id` e o `createdAt` originais.

### 7. Definition of Done

- [ ] `dotnet build` termina sem erros ou warnings.
- [ ] O `PUT /api/products/{id}` aparece no Scalar.
- [ ] O request body usa um schema próprio.
- [ ] As respostas `200`, `400` e `404` aparecem no OpenAPI.
- [ ] Os três cenários estão registrados no arquivo `.http`.
- [ ] Um `id` inexistente não cria produto.
- [ ] Você consegue explicar por que DTO e modelo interno são tipos diferentes.

### Pistas — abra somente se travar

<details>
<summary>Pista 1: fluxo</summary>

Controller recebe `id` e body → service procura e tenta atualizar → controller transforma o resultado em `200` ou `404`.

</details>

<details>
<summary>Pista 2: armazenamento</summary>

O service usa `ConcurrentDictionary`. Investigue operações que substituem um valor associado a uma chave e permitem detectar uma chave inexistente.

</details>

<details>
<summary>Pista 3: OpenAPI</summary>

Compare os atributos das actions existentes. O gerador precisa conhecer o tipo do corpo e o status code de cada resposta possível.

</details>

## Extensão opcional — code-first vs contract-first

Somente depois do exercício principal, escreva `docs/openapi/openapi.yaml` com as quatro operações. Compare-o com o documento gerado e registre:

- uma vantagem de code-first;
- uma vantagem de contract-first;
- um risco de divergência entre contrato e implementação;
- qual abordagem você escolheria para uma API interna pequena e por quê.

Tempo adicional estimado: 30–45 minutos.

## Perguntas para a revisão

1. Por que usamos `200` no `PUT`? Quando `204 No Content` faria sentido?
2. Por que um body inválido vira `400` mesmo sem um `if` explícito na action?
3. Qual é o risco de receber e devolver diretamente `Product`?
4. Quais alterações poderiam quebrar um client existente?
