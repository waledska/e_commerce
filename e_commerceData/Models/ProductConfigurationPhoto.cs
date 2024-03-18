using System.ComponentModel.DataAnnotations.Schema;

namespace e_commerce.e_commerceData.Models
{
    public class ProductConfigurationPhoto
    {
        public int Id { get; set; }
        public int? productConfiguration_Id { get; set; }
        public string? ImgUrl { get; set; }
        public virtual ProductConfiguration? ProductConfiguration { get; set; }
    }
}
