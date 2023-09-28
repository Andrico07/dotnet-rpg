using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.BaseGrid;
using dotnet_rpg.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        private static Dictionary<string, string> ColumnMapping = new Dictionary<string, string>
        {
            {
                "name", "Name"
            },
            {
                "price", "Price"
            }
        };

        public ProductService(IMapper mapper, DataContext context)
        {
             _mapper = mapper;
            _context = context;
        }

        public async Task<ServiceResponse<List<ProductGridDto>>> AddProduct(ProductGridDto newProduct)
        {
            var serviceResponse = new ServiceResponse<List<ProductGridDto>>();
            var product = _mapper.Map<Product>(newProduct);

            _context.Product.Add(product);
            await _context.SaveChangesAsync();

            serviceResponse.Data =
                await _context.Product.Select(c => _mapper.Map<ProductGridDto>(c)).ToListAsync();
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<ProductGridDto>>> DeleteProduct(int id)
        {
            var serviceResponse = new ServiceResponse<List<ProductGridDto>>();

            try
            {
                var product = await _context.Product.FirstOrDefaultAsync(c => c.Id == id);
                if (product is null)
                    throw new Exception($"Product with Id '{id}' not found.");

                _context.Product.Remove(product);

                await _context.SaveChangesAsync();

                serviceResponse.Data =
                    await _context.Product.Select(c => _mapper.Map<ProductGridDto>(c)).ToListAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<ProductGridDto>>> GetAllProducts()
        {
            var serviceResponse = new ServiceResponse<List<ProductGridDto>>();
            var dbProduct = await _context.Product.ToListAsync();
            serviceResponse.Data = dbProduct.Select(c => _mapper.Map<ProductGridDto>(c)).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<ProductGridDto>> GetProductById(int id)
        {
            var serviceResponse = new ServiceResponse<ProductGridDto>();
            var dbProduct = await _context.Product.FirstOrDefaultAsync(c => c.Id == id);
            serviceResponse.Data = _mapper.Map<ProductGridDto>(dbProduct);
            return serviceResponse;
        }

        public async Task<ServiceResponse<ProductGridDto>> UpdateProduct(ProductGridDto updatedProduct)
        {
            var serviceResponse = new ServiceResponse<ProductGridDto>();
            try
            {
                var product =
                    await _context.Product.FirstOrDefaultAsync(c => c.Id == updatedProduct.Id);
                if (product is null)
                    throw new Exception($"Product with Id '{updatedProduct.Id}' not found.");

                product.Name = updatedProduct.Name;
                product.Price = updatedProduct.Price;

                await _context.SaveChangesAsync();
                serviceResponse.Data = _mapper.Map<ProductGridDto>(product);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }





        public async Task<Object> GetAllStandard(int page = 1, int pageSize = 10, string sortBy = "Name", string filterBy = "")
        {
            var query = _context.Product.AsQueryable();

            if(!string.IsNullOrEmpty(filterBy)) {
                query = query.Where(c => c.Name.Contains(filterBy));
            }

            switch(sortBy.ToLower())
            {
                case "name":
                    query = query.OrderBy(c => c.Name);
                    break;
                case "price":
                    query = query.OrderBy(c => c.Price);
                    break;
                default:
                    query = query.OrderBy(c => c.Id);
                    break;
            }

            var totalCount = query.Count();

            var dbProduct = await query.Skip((page-1)*pageSize).Take(pageSize).ToListAsync();
            var characters = dbProduct.Select(c => new ProductGridDto { 
                Id = c.Id,
                Name = c.Name,
                Price = c.Price
            }).ToList();

            return new {
                Characters = characters,
                TotalCount = totalCount
            };
        }

        public async Task<BaseGridResponse<ProductGridDto>> GetAllGeneric(BaseGridSearch search)
        {
            var query = _context.Product.AsQueryable();

            if (!string.IsNullOrEmpty(search.FilterBy))
            {
                query = query.Where(c => c.Name.Contains(search.FilterBy));
            }

            if (ColumnMapping.ContainsKey(search.SortBy.ToLower()))
            {
                var propertyName = ColumnMapping[search.SortBy.ToLower()];
                query = search.IsSortAsc ? query.OrderBy(c => EF.Property<Product>(c!, propertyName)) : query.OrderByDescending(c => EF.Property<Product>(c!, propertyName));
            }

            var totalCount = query.Count();
            var dbProduct = await query.Skip((search.Page - 1) * search.PageSize).Take(search.PageSize).ToListAsync();
            var products = dbProduct.Select(c => new ProductGridDto
            {
                Id = c.Id,
                Name = c.Name,
                Price = c.Price
            }).ToList();

            return new BaseGridResponse<ProductGridDto>()
            {
                Items = products,
                TotalCount = totalCount
            };
        }

        public async Task<BaseGridResponse<Object>> GetAllExpanded(ExpandedGridSearch search)
        {
            var query = _context.Product.AsQueryable();

            if (!string.IsNullOrEmpty(search.FilterBy))
            {
                query = query.Where(c => c.Name.Contains(search.FilterBy));
            }

            var totalCount = query.Count();

            if (ColumnMapping.ContainsKey(search.SortBy.ToLower()))
            {
                var propertyName = ColumnMapping[search.SortBy.ToLower()];
                query = search.IsSortAsc ? query.OrderBy(c => EF.Property<Product>(c!, propertyName)) : query.OrderByDescending(c => EF.Property<Product>(c!, propertyName));
            }

            var selectedProperties = typeof(Product).GetProperties().Where(prop => search.SelectedColumns.Contains(prop.Name)).ToList();
            var dbProduct = await query.Skip((search.Page - 1) * search.PageSize).Take(search.PageSize).Select(p => new
            {
                Id = p.Id,
                Name = selectedProperties.Contains(typeof(Product).GetProperty("Name")) ? p.Name : null,
                Price = selectedProperties.Contains(typeof(Product).GetProperty("Price")) ? p.Price : 0,
            }).ToListAsync();

            return new BaseGridResponse<Object>()
            {
                //Items = dbProduct,
                TotalCount = totalCount
            };
        }

        public Task<List<Object>> GetAllFinal()
        {
            throw new NotImplementedException();
        }

    }
}