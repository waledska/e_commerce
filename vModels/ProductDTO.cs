namespace e_commerce.vModels
{
    public class ProductDTO
    {
        public ProductDTO()
        {
            AverageRatesForProductItems = 0.00;
            SelectedVariationOptions_ids = new List<int>();
        }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductImage { get; set; }
        public decimal? MinPriceInProductItems { get; set; }
        public decimal? MaxPriceInProductItems { get; set; }
        public List<int>? SelectedVariationOptions_ids { get; set; }
        public double? AverageRatesForProductItems { get; set; } // Optional, make sure to handle nulls
        public int selectedProdItem_Id { get; set; }
    }
}
