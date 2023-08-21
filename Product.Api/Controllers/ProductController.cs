using System.Threading.Tasks;
using AutoMapper;
using EventBus.Abstractions;
using Identity.Infrastructure.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Product.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Product.Api.Controllers;

[Route("api/products")]
public class ProductController : ControllerBase
{
    private readonly IMapper mapper;
    private readonly IMemoryCache cache;
    private readonly AppOptions.Jwt jwt;
    private readonly IEventBus eventHandler;
    private readonly ILogger<ProductController> logger;

    public ProductController(
        IMapper mapper,
        IMemoryCache cache,
        AppOptions.Jwt jwt,
        IEventBus eventHandler,
        ILogger<ProductController> logger
    )
    {
        this.mapper = mapper;
        this.cache = cache;
        this.jwt = jwt;
        this.eventHandler = eventHandler;
        this.logger = logger;
    }

    [HttpPost]
    [Authorize]
    public async Task<int> AddAsync([FromBody] ProductViewModel.AddProductInput input)
    {
        return 1;
    }
}