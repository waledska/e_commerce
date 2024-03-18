using e_commerce.e_commerceData.Models;
using e_commerce.vModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace e_commerce.Services
{
    public interface IProductService
    {
        // products
        Task<string> addProduct(productModel model);
        Task<string> updateProduct(productUpdateModel model);
        Task<string> deleteProduct(int? productId);
        Task<List<ProductDTO>> GetProducts_Filteration(string search,
            decimal? minPrice = 0,
            decimal? maxPrice = 1000000000,
            string sortBy = "UsersReviews",
            int page = 1,
            int limit = 10);
        

        // ProductItems
        Task<string> addProductItem(productItemModel model);
        Task<string> updateProductItem(productItemUpdateModel model); // not allowed to add or remove variation only modify in the current variation's options
        Task<string> addVariationForProductItems(addVariationForProductItemsModel model); // add
        Task<string> removeVariationFromProduct(int productId); // remove
        Task<string> deleteProductItem(int? ProductItemId);

        // getProductDetails =>
        Task<staticProductDetails> getStaticProductDetails(int productId);
        Task<DynamicProductDetails> getDynamicProductDetails(selectedProductItemData model); // i feel this need also varOpIds as input
        Task<List<AvailableVariationOptions>> getAvailableVariationOptions(selectedProductItemData model);

        // Variation
        Task<string> addVariation(variationModel model);
        Task<string> updateVariation(int varId, variationModel model);
        Task<string> deleteVariation(int varId);
        Task<List<variationModel>> getAllVariationsForCategory(int CatId);
        // Task<List<variationModel>> getAllVariationsForProduct(int ProductId); xx

        // VariationOption
        Task<string> addVariationOption(variationOptionModel model);
        Task<string> updateVariationOption(variationOptionModel model);
        Task<string> deleteVariationOption(int varOptionId);
        Task<List<productItemModel>> getAllForVariation(int varId);

        // productReviews
        Task<string> addUserReviewForProduct(variationOptionModel model);
        Task<string> modifyUserReview(reviewModel model);
        Task<string> DeleteUserReview(int varOptionId);
        Task<List<reviewModel>> getProductReviews(int productId);
    }
}
