using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using e_commerce.e_commerceData.Models;

namespace e_commerce.e_commerceData
{
    public partial class businessContext : DbContext
    {
        public businessContext()
        {
        }

        public businessContext(DbContextOptions<businessContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Address> Addresses { get; set; } = null!;
        public virtual DbSet<Country> Countries { get; set; } = null!;
        public virtual DbSet<OrderLine> OrderLines { get; set; } = null!;
        public virtual DbSet<OrderStatus> OrderStatuses { get; set; } = null!;
        public virtual DbSet<PaymentType> PaymentTypes { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductCategory> ProductCategories { get; set; } = null!;
        public virtual DbSet<ProductConfiguration> ProductConfigurations { get; set; } = null!;
        public virtual DbSet<ProductConfigurationPhoto> ProductPhotos { get; set; } = null!;
        public virtual DbSet<Promotion> Promotions { get; set; } = null!;
        public virtual DbSet<PromotionCategory> PromotionCategories { get; set; } = null!;
        public virtual DbSet<ShippingMethod> ShippingMethods { get; set; } = null!;
        public virtual DbSet<ShopOrder> ShopOrders { get; set; } = null!;
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; } = null!;
        public virtual DbSet<ShoppingCartItem> ShoppingCartItems { get; set; } = null!;
        public virtual DbSet<UserAddress> UserAddresses { get; set; } = null!;
        public virtual DbSet<UserPaymentMethod> UserPaymentMethods { get; set; } = null!;
        public virtual DbSet<UserReview> UserReviews { get; set; } = null!;
        public virtual DbSet<Variation> Variations { get; set; } = null!;
        public virtual DbSet<VariationOption> VariationOptions { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=SAKSOOOK;Database=e_commerce;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("address", "users");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.City)
                    .HasMaxLength(50)
                    .HasColumnName("city");

                entity.Property(e => e.CountryId).HasColumnName("country_id");

                entity.Property(e => e.PostalCode)
                    .HasMaxLength(100)
                    .HasColumnName("postal_code");

                entity.Property(e => e.Region)
                    .HasMaxLength(50)
                    .HasColumnName("region");

                entity.Property(e => e.StreetNumber)
                    .HasMaxLength(50)
                    .HasColumnName("street_number");

                entity.Property(e => e.UnitNumber)
                    .HasMaxLength(50)
                    .HasColumnName("unit_number");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.Addresses)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("FK_address_country");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("country", "users");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CountyName)
                    .HasMaxLength(100)
                    .HasColumnName("county_name");
            });

            modelBuilder.Entity<OrderLine>(entity =>
            {
                entity.ToTable("order_line", "orders");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.Price).HasColumnName("price");

                entity.Property(e => e.ProductConfigurationId).HasColumnName("product_configuration_id");

                entity.Property(e => e.Qty).HasColumnName("qty");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderLines)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_order_line_shop_order");

                entity.HasOne(d => d.ProductConfiguration)
                    .WithMany(p => p.OrderLines)
                    .HasForeignKey(d => d.ProductConfigurationId)
                    .HasConstraintName("FK_order_line_product_configuration");
            });

            modelBuilder.Entity<OrderStatus>(entity =>
            {
                entity.ToTable("order_status", "orders");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Status)
                    .HasMaxLength(200)
                    .HasColumnName("status");
            });

            modelBuilder.Entity<PaymentType>(entity =>
            {
                entity.ToTable("payment_type", "payment");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ImgUrl).HasColumnName("img_url");

                entity.Property(e => e.Value).HasColumnName("value");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("product", "products");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.ProductImage).HasColumnName("product_image");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_product_product_category");
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.ToTable("product_category", "products");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(100)
                    .HasColumnName("category_name");

                entity.Property(e => e.ImgUrl).HasColumnName("img_url");

                entity.Property(e => e.ParentCategoryId).HasColumnName("parent_category_id");

                entity.HasOne(d => d.ParentCategory)
                    .WithMany(p => p.InverseParentCategory)
                    .HasForeignKey(d => d.ParentCategoryId)
                    .HasConstraintName("FK_product_category_product_category");
            });

            modelBuilder.Entity<ProductConfiguration>(entity =>
            {
                entity.ToTable("product_configuration", "products");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Product_Id).HasColumnName("product_id");

                entity.Property(e => e.VariationOptionIds).HasColumnName("variation_option_ids");

                entity.Property(e => e.SKU).HasColumnName("SKU");

                entity.Property(e => e.QtyInStock).HasColumnName("qty_in_stock");

                entity.Property(e => e.Price).HasColumnType("decimal(10, 2)").HasColumnName("price");

                entity.HasOne(d => d.product)
                    .WithMany(p => p.ProductConfigurations)
                    .HasForeignKey(d => d.Product_Id)
                    .HasConstraintName("FK_product_configuration_product");

            });

            ////////////////
            modelBuilder.Entity<ProductConfigurationPhoto>(entity =>
            {
                entity.ToTable("Product_Photos", "products");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ImgUrl).HasColumnName("img_url");

                entity.Property(e => e.productConfiguration_Id).HasColumnName("product_configuration_id");

                entity.HasOne(d => d.ProductConfiguration)
                    .WithMany(p => p.ProductConfigurationPhotos)
                    .HasForeignKey(d => d.productConfiguration_Id)
                    .HasConstraintName("FK_Product_Photos_product_configuration");
            });
            ///////////////

            modelBuilder.Entity<Promotion>(entity =>
            {
                entity.ToTable("promotion", "products");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.DiscountRate).HasColumnName("discount_rate");

                entity.Property(e => e.EndDate)
                    .HasColumnType("date")
                    .HasColumnName("end_date");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("start_date");
            });

            modelBuilder.Entity<PromotionCategory>(entity =>
            {
                entity.ToTable("promotion_category", "products");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.PromotionId).HasColumnName("promotion_id");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.PromotionCategories)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_promotion_category_product_category");

                entity.HasOne(d => d.Promotion)
                    .WithMany(p => p.PromotionCategories)
                    .HasForeignKey(d => d.PromotionId)
                    .HasConstraintName("FK_promotion_category_promotion");
            });

            modelBuilder.Entity<ShippingMethod>(entity =>
            {
                entity.ToTable("shipping_method", "orders");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ImgUrl).HasColumnName("img_url");

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .HasColumnName("name");

                entity.Property(e => e.Price).HasColumnName("price");
            });

            modelBuilder.Entity<ShopOrder>(entity =>
            {
                entity.ToTable("shop_order", "orders");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.OrderDate)
                    .HasColumnType("date")
                    .HasColumnName("order_date");

                entity.Property(e => e.OrderStatusId).HasColumnName("order_status_id");

                entity.Property(e => e.OrderTotal).HasColumnName("order_total");

                entity.Property(e => e.PaymentMethodId).HasColumnName("payment_method_id");

                entity.Property(e => e.ShippingAddressId).HasColumnName("shipping_address_id");

                entity.Property(e => e.ShippingMethodId).HasColumnName("shipping_method_id");

                entity.Property(e => e.UserId)
                    .HasMaxLength(450)
                    .HasColumnName("user_id");

                entity.HasOne(d => d.OrderStatus)
                    .WithMany(p => p.ShopOrders)
                    .HasForeignKey(d => d.OrderStatusId)
                    .HasConstraintName("FK_shop_order_order_status");

                entity.HasOne(d => d.PaymentMethod)
                    .WithMany(p => p.ShopOrders)
                    .HasForeignKey(d => d.PaymentMethodId)
                    .HasConstraintName("FK_shop_order_user_payment_method");

                entity.HasOne(d => d.ShippingAddress)
                    .WithMany(p => p.ShopOrders)
                    .HasForeignKey(d => d.ShippingAddressId)
                    .HasConstraintName("FK_shop_order_address");

                entity.HasOne(d => d.ShippingMethod)
                    .WithMany(p => p.ShopOrders)
                    .HasForeignKey(d => d.ShippingMethodId)
                    .HasConstraintName("FK_shop_order_shipping_method");
            });

            modelBuilder.Entity<ShoppingCart>(entity =>
            {
                entity.ToTable("shopping_cart", "carts");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.UserId)
                    .HasMaxLength(450)
                    .HasColumnName("user_id");
            });

            modelBuilder.Entity<ShoppingCartItem>(entity =>
            {
                entity.ToTable("shopping_cart_item", "carts");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CartId).HasColumnName("cart_id");

                entity.Property(e => e.ProductConfigurationId).HasColumnName("product_item_id");

                entity.Property(e => e.Qty).HasColumnName("qty");

                entity.HasOne(d => d.Cart)
                    .WithMany(p => p.ShoppingCartItems)
                    .HasForeignKey(d => d.CartId)
                    .HasConstraintName("FK_shopping_cart_item_shopping_cart");

                entity.HasOne(d => d.ProductConfiguration)
                    .WithMany(p => p.ShoppingCartItems)
                    .HasForeignKey(d => d.ProductConfigurationId)
                    .HasConstraintName("FK_shopping_cart_item_product_configuration");
            });

            modelBuilder.Entity<UserAddress>(entity =>
            {
                entity.ToTable("user_address", "users");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AddressId).HasColumnName("address_id");

                entity.Property(e => e.IsDefault).HasColumnName("is_default");

                entity.Property(e => e.UserId)
                    .HasMaxLength(450)
                    .HasColumnName("user_id");

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.UserAddresses)
                    .HasForeignKey(d => d.AddressId)
                    .HasConstraintName("FK_user_address_address");
            });

            modelBuilder.Entity<UserPaymentMethod>(entity =>
            {
                entity.ToTable("user_payment_method", "payment");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccountNumber)
                    .HasMaxLength(450)
                    .HasColumnName("account_number");

                entity.Property(e => e.ExpiryDate)
                    .HasMaxLength(20)
                    .HasColumnName("expiry_date");

                entity.Property(e => e.IsDefault).HasColumnName("is_default");

                entity.Property(e => e.PaymentTypeId).HasColumnName("payment_type_id");

                entity.Property(e => e.Provider).HasColumnName("provider");

                entity.Property(e => e.UserId)
                    .HasMaxLength(450)
                    .HasColumnName("user_id");

                entity.HasOne(d => d.PaymentType)
                    .WithMany(p => p.UserPaymentMethods)
                    .HasForeignKey(d => d.PaymentTypeId)
                    .HasConstraintName("FK_user_payment_method_payment_type");
            });

            modelBuilder.Entity<UserReview>(entity =>
            {
                entity.ToTable("user_review", "users");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Comment).HasColumnName("comment");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.RatingValue).HasColumnName("rating_value");

                entity.Property(e => e.UserId)
                    .HasMaxLength(450)
                    .HasColumnName("user_id");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.UserReviews)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_user_review_product");
            });

            modelBuilder.Entity<Variation>(entity =>
            {
                entity.ToTable("variation", "products");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.Name)
                    .HasMaxLength(200)
                    .HasColumnName("name");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Variations)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_variation_product_category");
            });

            modelBuilder.Entity<VariationOption>(entity =>
            {
                entity.ToTable("variation_option", "products");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Value)
                    .HasMaxLength(200)
                    .HasColumnName("value");

                entity.Property(e => e.VariationId).HasColumnName("variation_id");

                entity.HasOne(d => d.Variation)
                    .WithMany(p => p.VariationOptions)
                    .HasForeignKey(d => d.VariationId)
                    .HasConstraintName("FK_variation_option_variation");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
