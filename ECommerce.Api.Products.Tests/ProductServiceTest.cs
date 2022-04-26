using AutoMapper;
using ECommerce.Api.Products.Db;
using ECommerce.Api.Products.Profiles;
using ECommerce.Api.Products.Providers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ECommerce.Api.Products.Tests
{
    public class ProductServiceTest
    {
        [Fact]
        public async Task GetProductsReturnAllProducts()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<ProductsDbContext>()
                .UseInMemoryDatabase(nameof(GetProductsReturnAllProducts)).Options;

            var productsDbContext = new ProductsDbContext(options);
            CreateProducts(productsDbContext);

            var productProfile = new ProductProfile();
            var configuration = new MapperConfiguration( config => config.AddProfile(productProfile));
            var mapper = new Mapper(configuration);

            var productProviders = new ProductsProvider(productsDbContext, null, mapper);


            //Act
            var products = await productProviders.GetProductsAsync();

            //Assert

            Assert.True(products.IsSuccess);
            Assert.True(products.Products.Any());
            Assert.Null(products.ErrorMessage);

        }


        [Fact]
        public async Task GetProductReturnProductUsingValidId()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<ProductsDbContext>()
                .UseInMemoryDatabase(nameof(GetProductReturnProductUsingValidId)).Options;

            var productsDbContext = new ProductsDbContext(options);
            CreateProducts(productsDbContext);

            var productProfile = new ProductProfile();
            var configuration = new MapperConfiguration(config => config.AddProfile(productProfile));
            var mapper = new Mapper(configuration);

            var productProviders = new ProductsProvider(productsDbContext, null, mapper);


            //Act
            var product = await productProviders.GetProductAsync(1);

            //Assert

            Assert.True(product.IsSuccess);
            Assert.NotNull(product.Product);
            Assert.True(product.Product.Id == 1);
            Assert.Null(product.ErrorMessage);

        }

        [Fact]
        public async Task GetProductDoNotReturnProductUsingInvalidId()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<ProductsDbContext>()
                .UseInMemoryDatabase(nameof(GetProductDoNotReturnProductUsingInvalidId)).Options;

            var productsDbContext = new ProductsDbContext(options);
            CreateProducts(productsDbContext);

            var productProfile = new ProductProfile();
            var configuration = new MapperConfiguration(config => config.AddProfile(productProfile));
            var mapper = new Mapper(configuration);

            var productProviders = new ProductsProvider(productsDbContext, null, mapper);


            //Act
            var product = await productProviders.GetProductAsync(-1);

            //Assert

            Assert.False(product.IsSuccess);
            Assert.Null(product.Product);
            Assert.NotNull(product.ErrorMessage);

        }

        private void CreateProducts(ProductsDbContext productsDbContext)
        {
            for (int i = 1; i <= 10; i++)
            {
                productsDbContext.Products.Add(new Product 
                {
                    Id = i,
                    Name = Guid.NewGuid().ToString(), 
                    Inventory = i + 10, 
                    Price = (decimal)(i * 3.14)  
                });
            }

            productsDbContext.SaveChanges();
        }
    }
}
