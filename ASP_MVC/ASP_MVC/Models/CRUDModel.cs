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
        List<DateTime> toDatelist;
        List<DateTime> fromDatelist;
        List<DateBaseOrderModel> tableData;

        // -- Свойства
        public List<DateTime> ToDatelist
        {
            get { return toDatelist; }
            set { toDatelist = value; }
        }

        public List<DateTime> FromDatelist
        {
            get { return fromDatelist; }
            set { fromDatelist = value; }
        }

        public List<DateBaseOrderModel> TableData
        {
            get { return tableData; }
            set { tableData = value; }
        }

        // -- Конструкторы
        public CRUDModel() {}
    }


    public interface ICRUDModelManager
    {
        //CRUDModel GetCRUDModel();

        CRUDModel GetCRUDModel(DateTime from, DateTime to);
    }

    public class CRUDModelManager : ICRUDModelManager
    {
        public CRUDModel GetCRUDModel(DateTime dateFrom, DateTime dateTo)
        {
            CRUDModel NewCRUDModel = new CRUDModel();
            using (DateBaseApplicationContext DB = new DateBaseApplicationContext())
            {
                List<DateTime> NewDateList = DB.Orders.OrderBy(order => order.Id).Where(order => (order.Date >= dateFrom) & (order.Date <= dateTo)).Select(order => order.Date).Distinct().ToList();
                
                NewCRUDModel.TableData = DB.Orders.Include(order => order.Provider).Where(order => (order.Date >= dateFrom) & (order.Date <= dateTo)).ToList();
                NewCRUDModel.ToDatelist = NewDateList;
                NewCRUDModel.FromDatelist = NewDateList;

            }
            

            return NewCRUDModel;
        }

        //public CRUDModel GetCRUDModel()
        //{
        //    return GetCRUDModel(DateTime.Today.AddMonths(-1), DateTime.Today);
        //}
    }
}
