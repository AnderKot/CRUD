using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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

            List<DateBaseOrderModel> OrdersList;
            using (DateBaseApplicationContext DB = new DateBaseApplicationContext())
            {
                OrdersList  = DB.Orders.Where(order =>
                                                (order.Date >= dateFrom) & (order.Date <= dateTo) &
                                                (Number.Contains(order.Number) | Number == "Не выбран") &
                                                (Provider.Contains(order.Provider.Name) | Number == "Не выбран")
                                             ).ToList();
            }

            string newJson = JsonSerializer.Serialize(OrdersList);

            return newJson;
        }
    }
}
