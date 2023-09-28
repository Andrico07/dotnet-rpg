using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_rpg.Dtos.BaseGrid;
using dotnet_rpg.Models;

namespace dotnet_rpg.Services.ProductService
{
    public interface IProductService
    {
        Task<ServiceResponse<List<ProductGridDto>>> GetAllProducts();
        Task<ServiceResponse<ProductGridDto>> GetProductById(int id);
        Task<ServiceResponse<List<ProductGridDto>>> AddProduct(ProductGridDto newProduct);
        Task<ServiceResponse<ProductGridDto>> UpdateProduct(ProductGridDto updatedProduct);
        Task<ServiceResponse<List<ProductGridDto>>> DeleteProduct(int id);
        Task<Object> GetAllStandard(int page = 1, int pageSize = 10, string sortBy = "Name", string filterBy = "");
        Task<BaseGridResponse<ProductGridDto>> GetAllGeneric(BaseGridSearch search);
        Task<BaseGridResponse<Object>> GetAllExpanded(ExpandedGridSearch search);
    }
}