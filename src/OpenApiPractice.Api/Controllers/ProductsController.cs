using Microsoft.AspNetCore.Mvc;
using OpenApiPractice.Api.Contracts.Products;
using OpenApiPractice.Api.Models;
using OpenApiPractice.Api.Services;

namespace OpenApiPractice.Api.Controllers;

[ApiController]
[Route("api/products")]
[Produces("application/json")]
public sealed class ProductsController(ProductService productService) : ControllerBase
{
    [HttpPut("{id:guid}", Name = "UpdateProduct")]
    [ProducesResponseType<ProductResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<ProductResponse> Update(Guid id, UpdateProductRequest request)
    {
        var product = productService.Update(id, request.Name, request.Price);

        if (product is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Produto não encontrado",
                Detail = $"Não existe produto com o identificador '{id}'.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(ToResponse(product));
    }

    [HttpGet(Name = "ListProducts")]
    [ProducesResponseType<IReadOnlyCollection<ProductResponse>>(StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyCollection<ProductResponse>> GetAll()
    {
        var products = productService.GetAll()
            .Select(ToResponse)
            .ToArray();

        return Ok(products);
    }

    [HttpGet("{id:guid}", Name = "GetProductById")]
    [ProducesResponseType<ProductResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<ProductResponse> GetById(Guid id)
    {
        var product = productService.GetById(id);

        if (product is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Produto não encontrado",
                Detail = $"Não existe produto com o identificador '{id}'.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(ToResponse(product));
    }

    [HttpPost(Name = "CreateProduct")]
    [ProducesResponseType<ProductResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public ActionResult<ProductResponse> Create(CreateProductRequest request)
    {
        var product = productService.Create(request.Name, request.Price);
        var response = ToResponse(product);

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, response);
    }

    private static ProductResponse ToResponse(Product product) =>
        new(product.Id, product.Name, product.Price, product.CreatedAt);
}
