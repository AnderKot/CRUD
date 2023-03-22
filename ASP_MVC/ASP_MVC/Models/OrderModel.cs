using Microsoft.EntityFrameworkCore;
using System;
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

        int SaveOrder(OrderJSON json);

        string GetOrders(string DateFrom, string DateTo, string Number, string Provider);

        string GetOrderItems(string id,string Name, string Quantity, string Unit);
    }

    // Модел для представления Заказа
    public record OrderModel
    {
        // -- Свойства
        public DateBaseOrderModel Header { get; set; }
        public List<DateBaseOrderItemModel> Lines { get; set; }
        public List<DateBaseProviderModel> Providers { get; set; }
        public List<string> NameFilterOptions { get; set; }
        public List<string> QuantityFilterOptions { get; set; }
        public List<string> UnitFilterOptions { get; set; }
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

        public ItemJSON() { }

        public ItemJSON(DateBaseOrderItemModel DBItem)
        {
            id = DBItem.Id.ToString();
            Name = DBItem.Name;
            Quantity = DBItem.Quantity.ToString();
            Unit = DBItem.Unit;
        }

        public static explicit operator ItemJSON(DateBaseOrderItemModel DBItem) => new ItemJSON(DBItem);
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
                IEnumerable<DateBaseOrderItemModel> LinesBuf = DB.OrderItems.Where(item => item.OrderId == id);
                newOrderModel.Lines = LinesBuf.ToList();

                newOrderModel.NameFilterOptions = new List<string>();
                newOrderModel.NameFilterOptions.AddRange(LinesBuf.Select(line => line.Name).Distinct().ToList());
                
                newOrderModel.QuantityFilterOptions = new List<string>();
                newOrderModel.QuantityFilterOptions.AddRange(LinesBuf.Select(line => line.Quantity.ToString()).Distinct().ToList());
                
                newOrderModel.UnitFilterOptions = new List<string>();
                newOrderModel.UnitFilterOptions.AddRange(LinesBuf.Select(line => line.Unit).Distinct().ToList());

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
                newOrderModel.NameFilterOptions = new List<string>();
                newOrderModel.QuantityFilterOptions = new List<string>();
                newOrderModel.UnitFilterOptions = new List<string>();
            }

            return newOrderModel;
        }

        public string GetOrders(string DateFrom, string DateTo, string Number, string Provider)
        {
            CRUDModel newCRUDModel = new CRUDModel();

            // определяем выбранные фильтры по дате
            DateTime dateFrom;
            if (DateFrom != null)
                dateFrom = DateTime.Parse(DateFrom);
            else
                dateFrom = DateTime.Today.AddMonths(-1);

            DateTime dateTo;
            if (DateTo != null)
                dateTo = DateTime.Parse(DateTo);
            else
                dateTo = DateTime.Today.AddMonths(-1);

            string newJson;
            IEnumerable<DateBaseOrderModel> Orders;

            // Конвертер дат для 
            var options = new JsonSerializerOptions() { WriteIndented = true };
            options.Converters.Add(new CustomDateTimeConverter());

            using (DateBaseApplicationContext DB = new DateBaseApplicationContext())
            {
                // Список заказов по фильтрам
                Orders = DB.Orders.Include(order => order.Provider).Where(order => (order.Date >= dateFrom) & (order.Date <= dateTo));
                if (Number != null)
                    Orders = Orders.Where(order => Number.Contains(order.Number));
                if (Provider != null)
                    Orders = Orders.Where(order => Provider.Contains(order.Provider.Name));
                //OrdersList = Orders.ToList();

                // Список номеров заказов для выбора фильтра
                newCRUDModel.OrderNumber = DB.Orders.Where(order => (order.Date >= dateFrom) & (order.Date <= dateTo)).Select(order => order.Number).Distinct().ToList();

                // Список имен поставщиков для выбора фильтра
                newCRUDModel.OrderProviderName = DB.Orders.Include(order => order.Provider).Where(order => (order.Date >= dateFrom) & (order.Date <= dateTo)).Select(order => order.Provider.Name).Distinct().ToList();

                // Список заказов
                newCRUDModel.TableData = DB.Orders.Include(order => order.Provider).Where(order => (order.Date >= dateFrom) & (order.Date <= dateTo)).ToList();

                newCRUDModel.TableData = Orders.ToList();
                
                newJson = JsonSerializer.Serialize(newCRUDModel, options);
            }

            Console.Write(newJson);

            return newJson;
        }

        public bool DeleteOrder(string strID)
        {
            int id = Int32.Parse(strID);

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


        // Выгрузка строк заказа по фильтрам
        public string GetOrderItems(string strID, string Name, string Quantity, string Unit)
        {
            OrderModel newOrderModel = new OrderModel();

            int id = Int32.Parse(strID);
            decimal quantity = 0;
            if (Quantity != null)
                quantity = decimal.Parse(Quantity);

            using (DateBaseApplicationContext DB = new DateBaseApplicationContext())
            {
                IEnumerable<DateBaseOrderItemModel> DBItems = DB.OrderItems.Where(item => item.OrderId == id);
                
                if (Name != null)
                    DBItems.Where(item => Name.Contains(item.Name));
                
                if (Quantity != null)
                    DBItems.Where(item => quantity.Equals(item.Quantity));
                
                if (Unit != null)
                    DBItems.Where(item => Unit.Contains(item.Unit));

                DBItems.ToList().ForEach(DBItem => newOrderModel.Lines.Add(DBItem));
            }

            string newJson = JsonSerializer.Serialize(newOrderModel);

            return newJson;
        }

        // Сохранение (Создание) заказа
        public int SaveOrder(OrderJSON orderJSON)
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
                    return -1;
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
                    DB.Orders.Entry(newOrder).State = EntityState.Modified;
                }

                // Сначала сохраняем заголовок заказа
                try
                {
                    DB.SaveChanges();
                }
                catch
                {
                    return -1;
                }

                // Если есть строки добавляем\обновляем\удаляем
                if (orderJSON.Items.Any())
                {
                    List<int> IdList = new List<int>();

                    foreach (ItemJSON item in orderJSON.Items)
                    {
                        IdList.Add(Int32.Parse(item.id));
                    }

                    DateBaseOrderItemModel newItem;
                    // Удаляем 
                    IEnumerable<DateBaseOrderItemModel> ItemToDelete = DB.OrderItems.Where(item => (item.OrderId == newOrder.Id));
                    ItemToDelete.Where(item => !IdList.Contains(item.Id));
                    if (ItemToDelete.Any())
                        DB.OrderItems.RemoveRange(ItemToDelete);

                    // Обновляем
                    foreach (ItemJSON item in orderJSON.Items.Where(item => item.id != "-1"))
                    {
                        newItem = new DateBaseOrderItemModel();
                        newItem.Id = Int32.Parse(item.id);
                        newItem.Name = item.Name;
                        if (newOrder.Number == newItem.Name)
                            return -1;
                        try
                        {
                            newItem.Quantity = Decimal.Parse(item.Quantity.Replace('.', ','));
                        }
                        catch
                        {
                            return -1;
                        }
                        newItem.Unit = item.Unit;
                        newItem.OrderId = newOrder.Id;
                        DB.OrderItems.Entry(newItem).State = EntityState.Modified;
                    }

                    // Добавляем
                    foreach (ItemJSON item in orderJSON.Items.Where(item => item.id == "-1"))
                    {
                        newItem = new DateBaseOrderItemModel();
                        newItem.Name = item.Name;
                        if (newOrder.Number == newItem.Name)
                            return -1;
                        try
                        {
                            newItem.Quantity = Decimal.Parse(item.Quantity.Replace('.', ','));
                        }
                        catch
                        {
                            return -1;
                        }
                        newItem.Unit = item.Unit;
                        newItem.OrderId = newOrder.Id;
                        DB.OrderItems.Add(newItem);
                    }
                }

                // Пытаемся сохранить изменения
                try
                {
                    DB.SaveChanges();
                    return newOrder.Id; // Если изменения были проведены возвраящаем актуальный номер заказа
                }
                catch
                {
                    return -1;
                }
            }


            //OrderJSON Order = JsonSerializer.Deserialize<OrderJSON> (json);
            return -1;
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
