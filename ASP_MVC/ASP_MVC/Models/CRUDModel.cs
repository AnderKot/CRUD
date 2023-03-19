using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using System.Data;
using System.Linq;

namespace ASP_MVC.Models
{
    public class CRUDModel
    {
        // -- поля
        List<string> orderNumber;
        List<string?> orderProviderName;
        List<DateBaseOrderModel> tableData; //Список Заказов

        // -- Свойства To-Do вынести в Интерфейс ?

        public List<string?> OrderNumber
        {
            get { return orderNumber; }
            set { orderNumber = value; }
        }

        public List<string?> OrderProviderName
        {
            get { return orderProviderName; }
            set { orderProviderName = value; }
        }

        public List<DateBaseOrderModel> TableData
        {
            get { return tableData; }
            set { tableData = value; }
        }
    }

    // DI контейнер 
    public interface ICRUDModelManager
    {
        CRUDModel GetCRUDModel();
    }

    //
    public class CRUDModelManager : ICRUDModelManager
    {
        public CRUDModel GetCRUDModel()
        {
            CRUDModel NewCRUDModel = new CRUDModel();

            DateTime dateFrom = DateTime.Today.AddMonths(-1); ;
            DateTime dateTo = DateTime.Today;

            // Выгрузка данных из базы
            using (DateBaseApplicationContext DB = new DateBaseApplicationContext())
            {
                // Список номеров заказов для выбора фильтра
                NewCRUDModel.OrderNumber = DB.Orders.Where(order => (order.Date >= dateFrom) & (order.Date <= dateTo)).Select(order => order.Number).Distinct().ToList();

                // Список имен поставщиков для выбора фильтра
                NewCRUDModel.OrderProviderName = DB.Orders.Include(order => order.Provider).Where(order => (order.Date >= dateFrom) & (order.Date <= dateTo)).Select(order => order.Provider.Name).Distinct().ToList();

                // Список заказов
                NewCRUDModel.TableData = DB.Orders.Include(order => order.Provider).Where(order => (order.Date >= dateFrom) & (order.Date <= dateTo)).ToList();
            }

            return NewCRUDModel;
        }
    }
}
