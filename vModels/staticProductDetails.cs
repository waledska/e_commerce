namespace e_commerce.vModels
{
    public class staticProductDetails
    {
        public staticProductDetails()
        {
            AverageRatesForProductItems = 0.00;
        }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductImage { get; set; }
        public double? AverageRatesForProductItems { get; set; } // Optional, make sure to handle nulls
    }
}
