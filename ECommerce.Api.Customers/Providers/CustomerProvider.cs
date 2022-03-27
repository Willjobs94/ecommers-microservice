using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ECommerce.Api.Customers.Db;
using ECommerce.Api.Customers.Interfaces;
using ECommerce.Api.Customers.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerce.Api.Customers.Providers
{
    public class CustomerProvider : ICustomerProvider
    {

        private readonly CustomersDbContext dbContext;
        private readonly ILogger<CustomerProvider> logger;
        private readonly IMapper mapper;


        public CustomerProvider(CustomersDbContext dbContext, ILogger<CustomerProvider> logger, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.mapper = mapper;

            SeedData();
        }


        private void SeedData()
        {
            if (!dbContext.Customers.Any())
            {
                dbContext.Customers.Add(new Db.Customer() { Id = 1, Name = "Merav Natasja", Address = "Renfrew, Pennsylvania(PA), 16053" });
                dbContext.Customers.Add(new Db.Customer() { Id = 2, Name = "Ligaya Nedyalko", Address = "Gulf Breeze, Florida(FL), 32563" });
                dbContext.Customers.Add(new Db.Customer() { Id = 3, Name = "Dilip Keturah", Address = "Highlands, New Jersey(NJ), 07732" });
                dbContext.SaveChanges();
            }
        }
        public async Task<(bool IsSuccess, Models.Customer Customer, string errorMessage)> GetCustomerAsync(int id)
        {

            try
            {
            
                var customer = await dbContext.Customers.FirstOrDefaultAsync(x => x.Id == id);
                if(customer != null) {
                    var result = mapper.Map<Db.Customer, Models.Customer>(customer);
                    return (true, result, null);
                }
                return (false, null, "Not found");

            }
            catch (System.Exception e)
            {
                logger?.LogError(e.ToString());
                return (false, null, e.Message);
            }
        }

        public async Task<(bool IsSuccess, IEnumerable<Models.Customer> Customers, string ErrorMessage)> GetCustomersAsync()
        {
            try
            {
                    var customers = await dbContext.Customers.ToListAsync();
                if(customers != null && customers.Any()) {
                  
                  var result = mapper.Map<IEnumerable<Db.Customer>, IEnumerable<Models.Customer>>(customers);
                    return (true, result, null);
                }

                return (false, null, "Not customer found");
            }
            catch (System.Exception e)
            {
                logger?.LogError(e.ToString());
                return (false, null, e.Message);
            }
        }
    }
}