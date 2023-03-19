using ASP_MVC.Models;
using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ASP_MVC
{
    [Index(nameof(Number), nameof(ProviderId), IsUnique = true, Name = "Index_NP")]
    public class DateBaseOrderModel //: IOrder
    {
        public int Id { get; set; }
        public string? Number { get; set; }
        public DateTime Date { get; set; }
        public int ProviderId { get; set; }
        public DateBaseProviderModel? Provider { get; set; }
    }

    //public interface IOrder
    //{
    //    int Id { get; set; }
    //    string Number { get; set;}
    //    DateTime Date { get; set; }
    //    int ProviderId { get; set; }
    //}

    public class DateBaseOrderItemModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Quantity { get; set; }
        public string? Unit { get; set; }

        public int OrderId { get; set; }
        public DateBaseOrderModel? Order { get; set; }
    }

    public class DateBaseProviderModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    public class DateBaseApplicationContext : DbContext
    {
        public DbSet<DateBaseOrderModel> Orders => Set<DateBaseOrderModel>();
        public DbSet<DateBaseOrderItemModel> OrderItems => Set<DateBaseOrderItemModel>();
        public DbSet<DateBaseProviderModel> Providers => Set<DateBaseProviderModel>();

        //public DateBaseApplicationContext() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=crudapp.db");
            
        }

    }
}
