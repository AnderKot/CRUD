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
        string selectedDateFrom; //компонент фильтра Даты "От"
        string selectedDateTo;   //компонент фильтра Даты "До"
        List<DateTime> datelist; //Список дат для выбора в фильтрах
        List<DateBaseOrderModel> tableData; //Список Заказов

        // -- Свойства To-Do вынести в Интерфейс ?
        public string SelectedDateFrom
        {
            get { return selectedDateFrom; }
            set { selectedDateFrom = value; }
        }

        public string SelectedDateTo
        {
            get { return selectedDateTo; }
            set { selectedDateTo = value; }
        }

        public List<DateTime> Datelist
        {
            get { return datelist; }
            set { datelist = value; }
        }

        public List<DateBaseOrderModel> TableData
        {
            get { return tableData; }
            set { tableData = value; }
        }

        // -- Конструкторы
        public CRUDModel()
        {
            selectedDateFrom = "Не Выбран";
            selectedDateTo = "Не Выбран";
        }
    }

    // DI контейнер 
    public interface ICRUDModelManager
    {
        CRUDModel GetCRUDModel(string From, string To);
    }

    //
    public class CRUDModelManager : ICRUDModelManager
    {
        public CRUDModel GetCRUDModel(string From, string To)
        {
            CRUDModel NewCRUDModel = new CRUDModel();

            DateTime dateFrom = DateTime.Today.AddMonths(-1); ;
            DateTime dateTo = DateTime.Today;

            // определяем выбранные фильтры по дате
            if ((From != "Не Выбран") & (From != null))
            {
                dateFrom = DateTime.Parse(From);
                NewCRUDModel.SelectedDateFrom = From;
            }

            if ((To != "Не Выбран") & (To != null))
            {
                dateTo = DateTime.Parse(To);
                NewCRUDModel.SelectedDateTo = To;
            }

            // Выгрузка данных из базы
            using (DateBaseApplicationContext DB = new DateBaseApplicationContext())
            {
                // Список Дат для выбора фильтра
                NewCRUDModel.Datelist = DB.Orders.OrderBy(order => order.Id).Where(order => (order.Date > dateFrom) & (order.Date < dateTo)).Select(order => order.Date).Distinct().ToList();
                // Список заказов
                NewCRUDModel.TableData = DB.Orders.Include(order => order.Provider).Where(order => (order.Date >= dateFrom) & (order.Date <= dateTo)).ToList();
            }

            return NewCRUDModel;
        }

        //public CRUDModel GetCRUDModel()
        //{
        //    return GetCRUDModel(DateTime.Today.AddMonths(-1), DateTime.Today);
        //}
    }
}
