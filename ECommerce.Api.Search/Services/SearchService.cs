using ECommerce.Api.Search.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Api.Search.Services
{
    public class SearchService : ISearchService
    {
        private readonly IOrderService orderService;
        private readonly IProductService productService;
        private readonly ICustomerService customerService;

        public SearchService(IOrderService orderService, IProductService productService, ICustomerService customerService)
        {
            this.orderService = orderService;
            this.productService = productService;
            this.customerService = customerService;
        }

        public async Task<(bool IsSuccess, dynamic SearchResult)> SearchAsync(int customerId)
        {
            var orderResult = await orderService.GetOrderAsync(customerId);
            var customerResult = await customerService.GetCustomerAsync(customerId);
            var productsResult = await productService.GetProductsAsync();


            if (orderResult.IsSuccess)
            {
                foreach (var order in orderResult.Orders)
                {
                    foreach (var product in order.Items)
                    {
                        product.ProductName = productsResult.IsSuccess 
                            ?  productsResult.Products.FirstOrDefault(x => x.Id == product.Id)?.Name
                            : "Product information is not available";
                    }
                }


                var result = new
                {
                    Customer = customerResult.IsSuccess ? customerResult.Customer : new { Name = "Customer information is not available" },
                    Orders = orderResult.Orders
                };

                return (true, result);
            }
            return (false, null);
        }
    }
}
