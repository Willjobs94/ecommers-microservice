using ECommerce.Api.Search.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Api.Search.Services
{
    public class SearchService : ISearchService
    {
        private readonly IOrderService orderService;
        private readonly IProductService productService;

        public SearchService(IOrderService orderService, IProductService productService)
        {
            this.orderService = orderService;
            this.productService = productService;
        }

        public async Task<(bool IsSuccess, dynamic SearchResult)> SearchAsync(int customerId)
        {
            var orderResult = await orderService.GetOrderAsync(customerId);
            var productsResult = await productService.GetProductsAsync();
 

            if (orderResult.IsSuccess)
            {
                foreach (var order in orderResult.Orders)
                {
                    foreach (var product in order.Items)
                    {
                        product.ProductName = productsResult.Products.FirstOrDefault(x => x.Id == product.Id)?.Name;
                    }
                }


                var result = new
                {
                    Orders = orderResult.Orders
                };

                return (true, result);
            }
            return (false, null);
        }
    }
}
