using e_commerce.e_commerceData;
using e_commerce.e_commerceData.Models;
using e_commerce.vModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace e_commerce.Services
{
    public class ProductService : IProductService
    {
        private readonly businessContext _db;
        private readonly ITransferPhotosToPathWithStoreService _ImageService;
        public ProductService(businessContext db, ITransferPhotosToPathWithStoreService imageservice)
        {
            _db = db;
            _ImageService = imageservice;
        }
        public async Task<string> addProduct(productModel model)
        {
            var message = "";
            if (_db.ProductCategories.FirstOrDefault(x => x.Id == model.CategoryId) == null)
                return "there is no category with this category Id";
            var imageResultPath = _ImageService.GetPhotoPath(model.imageFormFile);
            if (imageResultPath.StartsWith("error"))
            {
                return imageResultPath;
            }
            var newProduct = new Product 
            {
                CategoryId = model.CategoryId,
                Name = model.Name,
                Description = model.Description,
                ProductImage = imageResultPath
            };

            _db.Products.Add(newProduct);
            var rowsEffected = _db.SaveChanges();
            if(!(rowsEffected > 0))
            {
                return "some thing went wrong!";
            }
            return message;
        }

        // ------------------- helping functions for the product item area -------------------

        // you need to test and revise this => getVariationsForProduct
        public dynamic getVariationsForProduct(int ProdId) // returns also "this item doesn't have any variations"
        {
            //var variations = new List<dynamic>();

            var ListStringIds = _db.ProductConfigurations
                            .Where(p => p.Product_Id == ProdId)
                            .Select(x => x.VariationOptionIds)
                            .ToList();

            List<List<int>> ListIntIds = new List<List<int>>();
            foreach (var stringIds in ListStringIds)
            {
                ListIntIds.Add(Convertor.StringToList(stringIds));
            }

            var LongestListOfOptions = ListIntIds.OrderByDescending(list => list.Count).FirstOrDefault();
            if (LongestListOfOptions == null)
                return "this item doesn't have any variations";

            // get the variations ids for the variation options Ids
            var variationsIds = new List<int>();
            int? variationId = 0;
            foreach (var variationOptionId in LongestListOfOptions)
            {
                
                variationId = _db.VariationOptions
                       .FirstOrDefault(vo => vo.Id == variationId)?
                       .VariationId;
                
                if (variationId != null)
                    variationsIds.Add(variationOptionId);
            }

            var variations = _db.Variations
                .AsNoTracking()
                .Where(v => variationsIds.Contains(v.Id))
                .Select(variation => new
                {
                    variation.Id,
                    variation.Name
                })
                .ToList();

            return variations;
        }
        public bool IsVarOptionsValidForThisProd(int? ProdId, List<int>? ids)
        {


            return true;
        }

        public async Task<string> addProductItem(productItemModel model) //---------------------------
        {
            var message = "";
            if (_db.Products.FirstOrDefault(x => x.Id == model.ProductId) == null)
                return "there is no product with this product Id";

            // varOptionsIds and there validation isItIntList, IsItRightIds.... then convert them to string
            if (Convertor.IsValidListOfIntegers(model.variationOptions_Ids) || IsVarOptionsValidForThisProd(model.ProductId, model.variationOptions_Ids) )
                return "invalid List variationOptions_Ids for the product item";

            var stringVarOptionsIds = Convertor.ListToString(model.variationOptions_Ids);

            var imagesResultPaths = _ImageService.GetPhotosPath(model.imageFormFiles);
            if (!imagesResultPaths.Last().Contains("success"))
            {
                return imagesResultPaths[0];
            }
            var newProductItem = new ProductConfiguration
            {
                Product_Id = model.ProductId,
                Price = model.Price,
                VariationOptionIds = stringVarOptionsIds,
                QtyInStock = model.QtyInStock,
                SKU = model.Sku
            };
            _db.ProductConfigurations.Add(newProductItem);
            var rowsEffected = _db.SaveChanges();
            if (!(rowsEffected > 0))
            {
                return "some thing went wrong!";
            }

            var newProductItemPhotosList = new List<ProductConfigurationPhoto>();
            for (var i = 0; i < imagesResultPaths.Count - 1; i++)
            {
                newProductItemPhotosList.Add(new ProductConfigurationPhoto
                {
                    ImgUrl = imagesResultPaths[i],
                    productConfiguration_Id = newProductItem.Id
                });
            }
            _db.ProductItemPhoto.AddRange(newProductItemPhotosList);
            var rowsEffectedPhotos = _db.SaveChanges();
            if (!(rowsEffectedPhotos > 0))
            {
                return "some thing went wrong while adding this item's images!";
            }
            
            return message;
        }

        public async Task<string> addUserReviewForProduct(variationOptionModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<string> addVariation(variationModel model)
        {
            throw new NotImplementedException();
        }

        public Task<string> addVariationForProductItems(addVariationForProductItemsModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<string> addVariationOption(variationOptionModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<string> deleteProduct(int? productId)
        {
            var message = "";
            if (!productId.HasValue)
                return "you must enter product Id first";

            var delProduct = await _db.Products.FirstOrDefaultAsync(p => p.Id == productId);
            var delProductImage = delProduct.ProductImage;
            if (delProduct == null)
                return "already there is no product with this ID";

            // check if the product that we will del don't have productItem
            if ( await _db.ProductConfigurations.AnyAsync(i => i.Product_Id == productId))
                return "you must to delete first all product items for this product then delete it";

            // deleting the product
            _db.Products.Remove(delProduct);
            var rowsEffected = await _db.SaveChangesAsync();
            if (! (rowsEffected > 0) )
                return "something went wrong!";

            // delete the image for this product
            _ImageService.DeleteFile(delProductImage);

            return message;
        }

        public async Task<string> deleteProductItem(int? ProductItemId) //---------------------------
        {
            var message = "";
            if (!ProductItemId.HasValue)
                return "you must enter product item Id first";

            var delProductItem = await _db.ProductItems.FirstOrDefaultAsync(p => p.Id == ProductItemId);
            if (delProductItem == null)
                return "already there is no product with this ID";

            // getTheItemImages
            var itemImages = _db.ProductItemPhoto
                                .Where(i => i.ProductItemId == ProductItemId).ToList();

            // deleting the product
            _db.ProductItems.Remove(delProductItem);
            var rowsEffected = await _db.SaveChangesAsync();
            if (! (rowsEffected > 0))
                return "something went wrong!";
            
            // delete images for the productItem from it's table
            _db.ProductItemPhoto.RemoveRange(itemImages);

            var rowsEffectedImages = await _db.SaveChangesAsync();

            if( (rowsEffectedImages > 0) && (rowsEffected > 0))
            {
                foreach (var imagePath in itemImages)
                {
                    _ImageService.DeleteFile(imagePath.ImgUrl);
                }
            }

            return message;
        }

        public async Task<string> DeleteUserReview(int varOptionId)
        {
            throw new NotImplementedException();
        }

        public async Task<string> deleteVariation(int varId)
        {
            throw new NotImplementedException();
        }

        public async Task<string> deleteVariationOption(int varOptionId)
        {
            throw new NotImplementedException();
        }


        public async Task<List<productItemModel>> getAllForVariation(int varId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<variationModel>> getAllVariationsForCategory(int CatId)
        {
            throw new NotImplementedException();
        }

        public Task<List<AvailableVariationOptions>> getAvailableVariationOptions(selectedProductItemData model)
        {
            throw new NotImplementedException();
        }

        public Task<DynamicProductDetails> getDynamicProductDetails(selectedProductItemData model)
        {
            throw new NotImplementedException();
        }

        public async Task<List<reviewModel>> getProductReviews(int productId)
        {
            throw new NotImplementedException();
        }


        // keys for filteration sortBy = ["LowestPrice", "HighestPrice", "UsersReviews", "LatestProducts"]

        public async Task<List<ProductDTO>> GetProducts_Filteration(string search, //don't forget return SelectedVariationOptions_ids
            decimal? minPrice = 0,
            decimal? maxPrice = 1000000000,
            string sortBy = "UsersReviews",
            int page = 1,
            int limit = 10)
        {
            var query = _db.Products
            .Where(p => (search == null || p.Name.Contains(search) || p.Description.Contains(search))
                        && (minPrice == null || p.ProductConfigurations.Any(pi => pi.Price >= minPrice))
                        && (maxPrice == null || p.ProductConfigurations.Any(pi => pi.Price <= maxPrice)))
            .Select(x => new ProductDTO
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                ProductImage = x.ProductImage,
                MinPriceInProductItems = x.ProductItems.Min(pi => pi.Price),
                MaxPriceInProductItems = x.ProductItems.Max(pi => pi.Price),
                AverageRatesForProductItems = x.UserReviews.Average(ur => ur.RatingValue)
            });

            // Apply sorting based on the sortBy parameter.
            switch (sortBy)
            {
                case "LowestPrice":
                    query = query.OrderBy(p => p.MinPriceInProductItems);
                    break;
                case "HighestPrice":
                    query = query.OrderByDescending(p => p.MaxPriceInProductItems);
                    break;
                case "UsersReviews":
                    query = query.OrderByDescending(p => p.AverageRatesForProductItems);
                    break;
                case "LatestProducts":
                    query = query.OrderByDescending(p => p.Id); // Assuming higher IDs are newer. Adjust if there's a specific Date field.
                    break;
            }

            // Remember to materialize the query with ToListAsync or similar when you're ready to execute it.
            var result = await query.ToListAsync();

            return result;
        }

        public Task<staticProductDetails> getStaticProductDetails(int productId)
        {
            throw new NotImplementedException();
        }

        public async Task<string> modifyUserReview(reviewModel model)
        {
            throw new NotImplementedException();
        }

        public Task<string> removeVariationFromProduct(int productId)
        {
            throw new NotImplementedException();
        }

        public async Task<string> updateProduct(productUpdateModel model)
        {
            var message = "";

            var categoryExists = await _db.ProductCategories.AnyAsync(x => x.Id == model.CategoryId);
            if (!categoryExists)
                return "there is no category with this category Id";

            var product = await _db.Products.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (product == null)
                return "there is no product with this product Id";

            // Assuming GetPhotoPath and DeleteFile manage file storage correctly and don't interfere with EF tracking
            var oldImagePath = product.ProductImage;
            var newImagePath = _ImageService.GetPhotoPath(model.imageFormFile);
            if (newImagePath.StartsWith("error"))
            {
                return newImagePath;
            }

            // Update the product data
            product.ProductImage = newImagePath;
            product.CategoryId = model.CategoryId;
            product.Description = model.Description;
            product.Name = model.Name;

            // No need to call Update method explicitly when tracking is already in place
            var rowsEffected = await _db.SaveChangesAsync();
            if (!(rowsEffected > 0))
            {
                return "some thing went wrong";
            }

            // If the update is successful and the new image path is different, delete the old image.
            // Be cautious with this logic, as it might delete the image before ensuring the new image is successfully saved.
            if (oldImagePath != newImagePath)
            {
                _ImageService.DeleteFile(oldImagePath);
            }

            return message; // Ensure this returns a meaningful message or consider refactoring to void if not needed
        }


        public async Task<string> updateProductItem(productItemUpdateModel model) //---------------------------
        {
            var message = "";

            // check the product ID
            if (!(await _db.Products.AnyAsync(x => x.Id == model.ProductId)))
                return "there is not product with this product Id";

            // check product item Id
            var productItem = await _db.ProductItems.FirstOrDefaultAsync(p => p.Id == model.Id);
            if (productItem == null)
                return "invalid product item Id";

            // get old stored List of images in the DB
            var oldImages = _db.ProductItemPhoto.Where(p => p.ProductItemId == model.Id).ToList();

            // upadte item and store images
            var imagesResultPaths = _ImageService.GetPhotosPath(model.imageFormFiles);
            if (!imagesResultPaths.Last().Contains("success"))
            {
                return imagesResultPaths[0];
            }

            productItem.ProductId = model.ProductId;
            productItem.Price = model.Price;
            productItem.ProductImage = imagesResultPaths[0];
            productItem.QtyInStock = model.QtyInStock;
            productItem.Sku = model.Sku;
            
            var rowsEffectedUpdateItem = await _db.SaveChangesAsync();
            if (!(rowsEffectedUpdateItem > 0))
            {
                return "some thing went wrong!";
            }

            var newProductItemPhotosList = new List<ProductConfigurationPhoto>();
            for (var i = 0; i < imagesResultPaths.Count - 1; i++)
            {
                newProductItemPhotosList.Add(new ProductConfigurationPhoto
                {
                    ImgUrl = imagesResultPaths[i],
                    productConfiguration_Id = model.Id
                });
            }
            _db.ProductItemPhoto.AddRange(newProductItemPhotosList);
            var rowsEffectedPhotos = _db.SaveChanges();
            if (!(rowsEffectedPhotos > 0))
            {
                return "some thing went wrong while adding this item images!";
            }

            // romoving old stored images
            if((rowsEffectedPhotos > 0) && (rowsEffectedUpdateItem > 0))
            {
                _db.ProductItemPhoto.RemoveRange(oldImages);
                foreach (var image in oldImages)
                {
                    _ImageService.DeleteFile(image.ImgUrl);
                }
                _db.SaveChanges();
            }

            return message;
        }

        public async Task<string> updateVariation(int varId, variationModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<string> updateVariationOption(variationOptionModel model)
        {
            throw new NotImplementedException();
        }
    }
}
