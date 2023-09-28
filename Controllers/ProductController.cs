using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_rpg.Dtos.BaseGrid;
using dotnet_rpg.Models;
using dotnet_rpg.Services.ProductService;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<ServiceResponse<List<ProductGridDto>>>> Get()
        {
            return Ok(await _productService.GetAllProducts());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<ProductGridDto>>> GetSingle(int id)
        {
            return Ok(await _productService.GetProductById(id));
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<ProductGridDto>>>> AddProduct(ProductGridDto newProduct)
        {
            return Ok(await _productService.AddProduct(newProduct));
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse<List<ProductGridDto>>>> UpdateProduct(ProductGridDto updatedProduct)
        {
            var response = await _productService.UpdateProduct(updatedProduct);
            if(response.Data is null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<ProductGridDto>>> DeleteProduct(int id)
        {
            var response = await _productService.DeleteProduct(id);
            if(response.Data is null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet("GetAllStandard")]
        public async Task<ActionResult<Object>> GetAllStandard(int page, int pageSize, string sortBy, string? filterBy)
        {
            if(string.IsNullOrEmpty(filterBy))
            {
            return Ok(await _productService.GetAllStandard(page, pageSize, sortBy));
            }

            return Ok(await _productService.GetAllStandard(page, pageSize, sortBy, filterBy));
        }

        [HttpPost("GetAllGeneric")]
        public async Task<ActionResult<BaseGridResponse<ProductGridDto>>> GetAllGeneric([FromBody] BaseGridSearch search)
        {
            return Ok(await _productService.GetAllGeneric(search));
        }

        [HttpPost("GetAllExpanded")]
        public async Task<ActionResult<ServiceResponse<BaseGridResponse<Object>>>> GetAllExpanded(ExpandedGridSearch search)
        {
            return Ok(await _productService.GetAllExpanded(search));
        }
    }
}