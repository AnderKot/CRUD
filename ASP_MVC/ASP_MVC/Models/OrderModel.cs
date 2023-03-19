using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ASP_MVC.Models
{
    public interface IOrderModelManager
    {
        OrderModel GetOrder(string id);

       string GetOrders(string DateFrom, string DateTo, string Number, string Provider);
    }

    public class OrderModel
    {
        // -- Свойства
        public DateBaseOrderModel Header { get; set; }
        public List<DateBaseOrderItemModel> Lines { get; set; }
    }

    public class OrderModelManager : IOrderModelManager
    {
        public OrderModel GetOrder(string srtId)
        {
            int id = Int32.Parse(srtId);

            OrderModel newOrderModel = new OrderModel();

            // Выгрузка данных из базы
            using (DateBaseApplicationContext DB = new DateBaseApplicationContext())
            {
                // Список Заголовок Заказа
                newOrderModel.Header = DB.Orders.Include(order => order.Provider).Where(order => order.Id == id).Single();
                // Список заказов
                newOrderModel.Lines = DB.OrderItems.Where(item => item.OrderId == id).ToList();
            }

            return newOrderModel;
        }

        public string GetOrders(string DateFrom, string DateTo, string Number, string Provider)
        {
            // определяем выбранные фильтры по дате
            DateTime dateFrom = DateTime.Parse(DateFrom);
            DateTime dateTo = DateTime.Parse(DateTo);
            
            string newJson;
            IEnumerable<DateBaseOrderModel> Orders;

            // Конвертер дат для 
            var options = new JsonSerializerOptions() { WriteIndented = true };
            options.Converters.Add(new CustomDateTimeConverter());

            using (DateBaseApplicationContext DB = new DateBaseApplicationContext())
            {
                //IEnumerable<DateBaseOrderModel> Orders;
                Orders = DB.Orders.Include(order => order.Provider).Where(order => (order.Date >= dateFrom) & (order.Date <= dateTo));
                if (Number != null)
                    Orders = Orders.Where(order => Number.Contains(order.Number));
                if (Provider != null)
                    Orders = Orders.Where(order => Provider.Contains(order.Provider.Name));
                //OrdersList = Orders.ToList();

                newJson = JsonSerializer.Serialize(Orders, options);
            }

            return newJson;
        }
    }


    public class CustomDateTimeConverter : JsonConverter<DateTime>
    {
        public CustomDateTimeConverter(){}

        public override void Write(Utf8JsonWriter writer, DateTime date, JsonSerializerOptions options)
        {
            writer.WriteStringValue(date.ToLongDateString());
        }
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString()); // Не стоит заморачиваться, все равно обратно преобразовавать не нужно.
        }
    }
}
