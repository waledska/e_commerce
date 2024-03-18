namespace e_commerce.vModels
{
    public class selectedProductItemData
    {
        public selectedProductItemData()
        {
            selectedVariationOptions_Ids = new List<int>();
        }
        public int productId { get; set; }
        public List<int> selectedVariationOptions_Ids { get; set; }
    }
}
