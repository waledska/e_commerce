namespace e_commerce.vModels
{
    public class variationOption
    {
        public variationOption()
        {
            variationOptionName = "";
        }
        public int? variationOptionId { get; set; }
        public string? variationOptionName { get; set; }
        public bool? IsAvailable { get; set; }
    }
    
    public class AvailableVariationOptions
    {
        public AvailableVariationOptions()
        {
            variationName = "";
            variationOptions = new List<variationOption>();
        }
        public int? variationId { get; set; }
        public string? variationName { get; set; }
        public List<variationOption> variationOptions { get; set; }
    }
}
