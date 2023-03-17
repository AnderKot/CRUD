using ASP_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ASP_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICRUDModelManager _CRUDModelManager;
        private readonly IOrderModelManager _OrderModelManager;

        public HomeController(ILogger<HomeController> logger, ICRUDModelManager CRUDModelManager, IOrderModelManager OrderModelManager)
        {
            _logger = logger;
            _CRUDModelManager = CRUDModelManager;
            _OrderModelManager = OrderModelManager;
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
            return View(_CRUDModelManager.GetCRUDModel(From, To));
        }

        [HttpGet]
        public IActionResult Order(string id)
        {
            return PartialView(_OrderModelManager.GetOrder(id));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}