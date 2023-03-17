using ASP_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ASP_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICRUDModelManager _CRUDModelManager;

        public HomeController(ILogger<HomeController> logger, ICRUDModelManager CRUDModelManager)
        {
            _logger = logger;
            _CRUDModelManager = CRUDModelManager;

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }



        public IActionResult CRUD(string From, string To)
        {
            DateTime dateFrom = DateTime.Today.AddMonths(-1); ;
            DateTime dateTo = DateTime.Today;

            if ((From != "Не Выбран")&(From != null))
            {
                dateFrom = DateTime.Parse(From);
            }

            if ((To != "Не Выбран")&(To != null))
            {
                dateTo = DateTime.Parse(To);
            }
            
            return View(_CRUDModelManager.GetCRUDModel(dateFrom, dateTo));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}