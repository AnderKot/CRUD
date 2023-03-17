using Microsoft.EntityFrameworkCore;

namespace ASP_MVC.Models
{
    public interface IOrderModelManager
    {
        OrderModel GetOrder(string id);
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
                // Список Дат для выбора фильтра
                newOrderModel.Header = DB.Orders.Include(order => order.Provider).Where(order => order.Id == id).Single();
                // Список заказов
                newOrderModel.Lines = DB.OrderItems.Where(item => item.OrderId == id).ToList();
            }

            return newOrderModel;
        }
    }
}
