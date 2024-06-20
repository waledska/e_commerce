namespace e_commerce.vModels
{
    public class DynamicProductDetails
    {
        public DynamicProductDetails()
        {
            selectedVariationOptions = new List<int>();
            SKU = "";
            productItemImages = new List<string>();
        }
        // product_config_ID
        public int? productItemId { get; set; }
        public int? productId { get; set; }
        public List<int>? selectedVariationOptions { get; set; }
        public string? SKU { get; set; }
        public int? QtyInStock { get; set; }
        public decimal? price { get; set; }
        public List<string>? productItemImages { get; set; }
    }
}
