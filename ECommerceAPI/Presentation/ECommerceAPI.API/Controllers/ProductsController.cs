using ECommerceAPI.Application.Abstractions.Storage;
using ECommerceAPI.Application.Repositories;
using ECommerceAPI.Application.RequestParameters;
using ECommerceAPI.Application.ViewModels.Products;
using ECommerceAPI.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        readonly private IProductWriteRepository _productWriteRepository;
        readonly private IProductReadRepository _productReadRepository;
        readonly private IOrderWriteRepository _orderWriteRepository;
        readonly private IOrderReadRepository _orderReadRepository;
        readonly private ICustomerWriteRepository _customerWriteRepository;
        readonly private ICustomerReadRepository _customerReadRepository;
        readonly private IWebHostEnvironment _webHostEnvironment;
        readonly private IFileWriteRepository _fileWriteRepository;
        readonly private IFileReadRepository _fileReadRepository;
        readonly private IProductImageFileReadRepository _productImageFileReadRepository;
        readonly private IProductImageFileWriteRepository _productImageFileWriteRepository;
        readonly private IStorageService _storageService;



        public ProductsController(IProductWriteRepository productWriteRepository, IProductReadRepository productReadRepository, IOrderWriteRepository orderWriteRepository, IOrderReadRepository orderReadRepository, ICustomerWriteRepository customerWriteRepository, ICustomerReadRepository customerReadRepository, IWebHostEnvironment webHostEnvironment, IFileWriteRepository fileWriteRepository, IFileReadRepository fileReadRepository, IProductImageFileReadRepository productImageFileReadRepository, IProductImageFileWriteRepository productImageFileWriteRepository, IStorageService storageService)
        {
            _productWriteRepository = productWriteRepository;
            _productReadRepository = productReadRepository;
            _orderWriteRepository = orderWriteRepository;
            _orderReadRepository = orderReadRepository;
            _customerWriteRepository = customerWriteRepository;
            _customerReadRepository = customerReadRepository;
            _webHostEnvironment = webHostEnvironment;
            _fileWriteRepository = fileWriteRepository;
            _fileReadRepository = fileReadRepository;
            _productImageFileReadRepository = productImageFileReadRepository;
            _productImageFileWriteRepository = productImageFileWriteRepository;
            _storageService = storageService;
        }

        //// Just Testing Order
        //[HttpGet("{id},{address}")]
        //public async Task<ActionResult<Order>> Get(string id, string address)
        //{

        //	Order order = await _orderReadRepository.GetByIdAsync(id);
        //	try
        //	{
        //		order.Address = address;
        //		await _orderWriteRepository.SaveAsync();
        //		return Ok(order);
        //	}
        //	catch (Exception)
        //	{
        //		return StatusCode(StatusCodes.Status500InternalServerError,
        //			"Error retrieving data from the database");
        //	}
        //}
        //GetAllProducts
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] Pagination pagination)
        {
            try
            {
                var totalCount = _productReadRepository.GetAll(false).Count();

                IQueryable products = _productReadRepository.GetAll(false).Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Stock,
                    p.Price,
                    p.CreatedDate,
                    p.UpdatedDate
                }).Skip(pagination.Page * pagination.Size).Take(pagination.Size);
                return Ok(new
                {
                    totalCount,
                    products
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }
        //GetProductById
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(string id)
        {
            try
            {
                Product product = await _productReadRepository.GetByIdAsync(id, false);
                return Ok(product);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }
        //AddProduct
        [HttpPost]
        public async Task<ActionResult> Add(VM_Create_Product model)
        {
            if (ModelState.IsValid)
            {
                await _productWriteRepository.AddAsync(new()
                {
                    Name = model.Name,
                    Stock = model.Stock,
                    Price = model.Price,

                });
                await _productWriteRepository.SaveAsync();
                return Ok(model);
            }
            else
            {
                return BadRequest(ModelState);
            }


        }
        //UpdateProduct
        [HttpPut]
        public async Task<ActionResult<Product>> Put(VM_Update_Product model)
        {
            Product product = await _productReadRepository.GetByIdAsync(model.Id);

            try
            {
                product.Name = model.Name;
                product.Stock = model.Stock;
                product.Price = model.Price;

                await _productWriteRepository.SaveAsync();
                return Ok(product);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error sending data to the database");
            }
        }
        //DeleteProduct
        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> Del(string id)
        {

            try
            {
                await _productWriteRepository.RemoveAsync(id);
                await _productWriteRepository.SaveAsync();
                return Ok(new
                {
                    message = "Product Succesfully Deleted"
                });

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error sending data to the database");
            }
        }
        //PostFile
        [HttpPost("[action]")]
        public async Task<IActionResult> Upload(string id)
        {
            //Can Be Used On LocalStorage
            //var datas = await _storageService.UploadAsync("files", Request.Form.Files);
            //await _productImageFileWriteRepository.AddRangeAsync(datas.Select(d => new ProductImageFile()
            //{
            //    FileName = d.fileName,
            //    Path = d.pathOrContainerName,
            //    Storage = _storageService.StorageName
            //}).ToList());
            //await _productImageFileWriteRepository.SaveAsync();

            List<(string fileName, string pathOrContainerName)> result = await _storageService.UploadAsync("photo-images", Request.Form.Files);
            Product product = await _productReadRepository.GetByIdAsync(id);

            await _productImageFileWriteRepository.AddRangeAsync(result.Select(r => new ProductImageFile
            {
                FileName = r.fileName,
                Path = r.pathOrContainerName,
                Storage = _storageService.StorageName,
                Products = new List<Product>() { product }
            }).ToList());
            await _productImageFileWriteRepository.SaveAsync();

            return Ok();
        }

    }
}
