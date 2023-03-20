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
        
        OrderModel GetEmptyOrder();

        bool DeleteOrder(string id);

        bool SaveOrder(OrderJSON json);

        string GetOrders(string DateFrom, string DateTo, string Number, string Provider);
    }

    // Модел для представления Заказа
    public record OrderModel
    {
        // -- Свойства
        public DateBaseOrderModel Header { get; set; }
        public List<DateBaseOrderItemModel> Lines { get; set; }
        public List<DateBaseProviderModel> Providers { get; set; }
    }

    // Формат заказа для POST запроса
    public class OrderJSON
    {
        public string id { get; set; }
        public string Number { get; set; }
        public string Date { get; set; }
        public string Provider { get; set; }
        public List<ItemJSON> Items { get; set; }
    }

    // Формат товара для POST запроса
    public class ItemJSON
    {
        public string id { get; set; }
        public string Name { get; set; }
        public string Quantity { get; set; }
        public string Unit { get; set; }
    }

    public class OrderModelManager : IOrderModelManager
    {
        public OrderModel GetOrder(string strId)
        {
            int id = Int32.Parse(strId);

            OrderModel newOrderModel = new OrderModel();

            // Выгрузка данных из базы
            using (DateBaseApplicationContext DB = new DateBaseApplicationContext())
            {
                // Список Заголовок Заказа
                newOrderModel.Header = DB.Orders.Include(order => order.Provider).Where(order => order.Id == id).Single();
                // Список заказов
                newOrderModel.Lines = DB.OrderItems.Where(item => item.OrderId == id).ToList();

                newOrderModel.Providers = DB.Providers.Distinct().ToList();
            }

            return newOrderModel;
        }

        public OrderModel GetEmptyOrder()
        {
            OrderModel newOrderModel = new OrderModel();
            newOrderModel.Header = new DateBaseOrderModel();
            newOrderModel.Header.Id = -1;
            newOrderModel.Header.Number = "";
            newOrderModel.Header.Date = DateTime.Today;
            newOrderModel.Header.Provider = new DateBaseProviderModel("");
            
            newOrderModel.Lines = new List<DateBaseOrderItemModel>();

            using (DateBaseApplicationContext DB = new DateBaseApplicationContext())
            {
                newOrderModel.Providers = DB.Providers.Distinct().ToList();
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

            Console.Write(newJson);

            return newJson;
        }

        public bool DeleteOrder(string strid)
        {
            int id = Int32.Parse(strid);

            using (DateBaseApplicationContext DB = new DateBaseApplicationContext())
            {
                DateBaseOrderModel Order = new DateBaseOrderModel(id);
                try
                {
                    DB.Orders.Attach(Order);
                    DB.Orders.Remove(Order); // Строки удаляться каскадом
                    DB.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }

                //DB.OrderItems.Where(item => item.OrderId == id);
            }
            return false;
        }

        public bool SaveOrder(OrderJSON orderJSON)
        {
            using (DateBaseApplicationContext DB = new DateBaseApplicationContext())
            {
                // Работаем с Заказом
                DateBaseOrderModel newOrder = new DateBaseOrderModel();
                newOrder.Number = orderJSON.Number;
                newOrder.Date = DateTime.Parse(orderJSON.Date);
                try
                {
                    newOrder.ProviderId = DB.Providers.Where(provider => provider.Name == orderJSON.Provider).Distinct().Single().Id;
                }
                catch
                {
                    return false;
                }

                if (orderJSON.id == "-1")
                {
                    // Новый Заказ Добавляем
                    DB.Orders.Add(newOrder);
                }
                else
                {
                    // Старый заказ Обновляем
                    newOrder.Id =  Int32.Parse(orderJSON.id);
                    DB.Entry(newOrder).State = EntityState.Modified;
                }


                // Если есть строки добавляем\обновляем\удаляем
                if (orderJSON.Items.Count == 0)
                    return true;
                else
                {
                    DateBaseOrderItemModel newItem;
                    foreach (ItemJSON item in orderJSON.Items)
                    {

                        if (item.id == "-1")
                        {
                            newItem = new DateBaseOrderItemModel();
                            newItem.Name = item.Name;
                            newItem.Quantity = Decimal.Parse(item.Quantity);
                            newItem.Unit = item.Unit;
                            newItem.OrderId = newOrder.Id;
                            DB.OrderItems.Add(newItem);
                        }
                        else
                        {

                        }
                    }

                }

                // Пытаемся сохранить изменения
                try
                {
                    DB.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }


            //OrderJSON Order = JsonSerializer.Deserialize<OrderJSON> (json);
            return false;
        }
    }

    // Кастомный сериализатор в удобоворимую форму даты для таблицы
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
