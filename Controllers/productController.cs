using e_commerce.e_commerceData;
using e_commerce.e_commerceData.Models;
using e_commerce.Services;
using e_commerce.vModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace e_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = "Bearer")]
    public class productController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly businessContext db;
        public productController(IProductService productService, businessContext businessContext)
        {
            _productService = productService;
            db = businessContext;
        }
        [HttpPost("addProduct")]
        public async Task<IActionResult> AddProduct([FromForm] productModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _productService.addProduct(model);
            if(result != "")
                return BadRequest(result);
            return Ok("product added successfully");
        }

        [HttpPost("updateProduct")]
        public async Task<IActionResult> UpdateProduct([FromForm] productUpdateModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (model == null)
            {
                return BadRequest("model can't be empty");
            }
            var result = await _productService.updateProduct(model);
            if (result != "")
                return BadRequest(result);
            return Ok("product updated successfully");
        }

        [HttpDelete("deleteProduct/{productId}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _productService.deleteProduct(productId);
            if (result != "")
                return BadRequest(result);
            return Ok("product deleted successfully");
        }

        [HttpGet("GetProducts_Filteration")]
        public async Task<IActionResult> GetProductsFilteration(
            [FromQuery] string? search = null, // Explicitly set to null by default
            [FromQuery] decimal? minPrice = null, // Assuming you want these to be truly optional
            [FromQuery] decimal? maxPrice = null, // Assuming you want these to be truly optional
            [FromQuery] string sortBy = "UsersReviews", // Default value provided
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10)
        {

            var result = await _productService.GetProducts_Filteration(search, minPrice, maxPrice, sortBy, page, limit);

            //filteration if you used in filter HighestPrice => use from the response json MaxPriceInProductItems and if another use 
            if (sortBy == "MaxPriceInProductItems")
            {
                var finalResult = result.Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Description,
                    price = x.MaxPriceInProductItems,
                    x.ProductImage,
                    x.AverageRatesForProductItems
                });
                return Ok(finalResult);
            }
            else
            {
                var finalResult = result.Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Description,
                    price = x.MinPriceInProductItems,
                    x.ProductImage,
                    x.AverageRatesForProductItems
                });
                return Ok(finalResult);
            }
        }

        [HttpPost("addProductItem")]
        public async Task<IActionResult> AddProductItem([FromForm] productConfigurationModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _productService.addProductConfiguration(model);
            if (result != "")
                return BadRequest(result);
            return Ok("item added to the product successfully");
        }

        [HttpPost("updateProductItem")]
        public async Task<IActionResult> UpdateProductItem([FromForm] productConfigurationUpdateModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _productService.updateProductConfiguration(model);
            if (result != "")
                return BadRequest(result);
            return Ok("Product Item updated successfully");
        }

        [HttpDelete("deleteProductItem/{productItemId}")]
        public async Task<IActionResult> DeleteProductItem(int productItemId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _productService.deleteProductConfiguration(productItemId);
            if (result != "")
                return BadRequest(result);

            return Ok("Product Item deleted successfully");
        }

        [HttpGet("getAllForProduct/{productId}")]
        public async Task<IActionResult> GetAllForProduct(int productId)
        {
            var result = await _productService.getAllProductItemsForProduct(productId);
            return Ok(result);
        }
        /////////////////////
        [HttpGet("test/{productId}")]
        public async Task<IActionResult> getProductDetails(int productId)
        {
            var result = db.ProductItemPhoto.Where(r => r.ProductItem.ProductId == 13)
                .Include(x => x.ProductItem)
                .ThenInclude(p => p.Product)
                .ThenInclude(c => c.Category)
                .Include(pi => pi.ProductItem)
                .ThenInclude(pc => pc.ProductConfigurations)
                .ThenInclude(vp => vp.VariationOption)
                .ThenInclude(v => v.Variation).ToList();

            

            return Ok(result);
        }
        ////////////////////
        [HttpPost("addVariation")]
        public async Task<IActionResult> AddVariation([FromBody] variationModel model)
        {
            var result = await _productService.addVariation(model);
            return Ok(result);
        }
        [HttpPost("updateVariation/{varId}")]
        public async Task<IActionResult> UpdateVariation(int varId, [FromBody] variationModel model)
        {
            var result = await _productService.updateVariation(varId, model);
            return Ok(result);
        }

        [HttpDelete("deleteVariation/{varId}")]
        public async Task<IActionResult> DeleteVariation(int varId)
        {
            var result = await _productService.deleteVariation(varId);
            return Ok(result);
        }

        [HttpGet("getAllVariationsForCategory/{catId}")]
        public async Task<IActionResult> GetAllVariationsForCategory(int catId)
        {
            var result = await _productService.getAllVariationsForCategory(catId);
            return Ok(result);
        }

        [HttpGet("getAllVariationsForProduct/{productId}")]
        public async Task<IActionResult> GetAllVariationsForProduct(int productId)
        {
            var result = await _productService.getAllVariationsForProduct(productId);
            return Ok(result);
        }

        [HttpPost("addVariationOption")]
        public async Task<IActionResult> AddVariationOption([FromBody] variationOptionModel model)
        {
            var result = await _productService.addVariationOption(model);
            return Ok(result);
        }

        [HttpPost("updateVariationOption")]
        public async Task<IActionResult> UpdateVariationOption([FromBody] variationOptionModel model)
        {
            var result = await _productService.updateVariationOption(model);
            return Ok(result);
        }

        [HttpDelete("deleteVariationOption/{varOptionId}")]
        public async Task<IActionResult> DeleteVariationOption(int varOptionId)
        {
            var result = await _productService.deleteVariationOption(varOptionId);
            return Ok(result);
        }

        [HttpGet("getAllForVariation/{varId}")]
        public async Task<IActionResult> GetAllForVariation(int varId)
        {
            var result = await _productService.getAllForVariation(varId);
            return Ok(result);
        }

        [HttpPost("addUserReviewForProduct")]
        public async Task<IActionResult> AddUserReviewForProduct([FromBody] variationOptionModel model)
        {
            var result = await _productService.addUserReviewForProduct(model);
            return Ok(result);
        }

        [HttpPost("modifyUserReview")]
        public async Task<IActionResult> ModifyUserReview([FromBody] reviewModel model)
        {
            var result = await _productService.modifyUserReview(model);
            return Ok(result);
        }

        [HttpDelete("deleteUserReview/{varOptionId}")]
        public async Task<IActionResult> DeleteUserReview(int varOptionId)
        {
            var result = await _productService.DeleteUserReview(varOptionId);
            return Ok(result);
        }

        [HttpGet("getProductReviews/{productId}")]
        public async Task<IActionResult> GetProductReviews(int productId)
        {
            var result = await _productService.getProductReviews(productId);
            return Ok(result);
        }
    }
}
