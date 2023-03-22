using ASP_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.Json;

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

        // Отгрузка всей страницы приложения
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult CRUD()
        {
            return View(_CRUDModelManager.GetCRUDModel());
        }

        // Модальное окно для заказа
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Order(string id)
        {
            OrderModel model;

            if (string.IsNullOrEmpty(id))
            {
                model = _OrderModelManager.GetEmptyOrder();
                return PartialView(model);
            }

            model = _OrderModelManager.GetOrder(id);
            if (model == null)
                return NotFound();

            return PartialView(model);
        }

        // Добавление/обновление заказа
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<string> OrderPost([FromBody] OrderJSON json)
        {
            if (json == null)
                return BadRequest();
            if (json.id == null)
                return BadRequest();

            int OrderId = _OrderModelManager.SaveOrder(json);
            if (OrderId != -1)
            {
                return JsonSerializer.Serialize(OrderId);
            }
            else
            {
                return BadRequest();
            }

            return BadRequest();
        }

        // Удаление заказа
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult OrderDelete(string id)
        {
            if (id == "-1") 
                return Ok();

            if (string.IsNullOrEmpty(id))
                return BadRequest();

            bool ok = _OrderModelManager.DeleteOrder(id);
            if (ok)
                return Ok();
            else
                return NotFound();

            return BadRequest();
        }

        // Отправка заказов для перерисовки таблицы по фильтрам
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<string> Orders(string DateFrom, string DateTo, string Number, string Provider)
        {
            string JsonOrders = _OrderModelManager.GetOrders(DateFrom, DateTo, Number, Provider);
            
            if (JsonOrders == null)
                return NotFound();
            
            return JsonOrders;
        }

        // Отправка строк заказа для перерисовки таблицы по фильтрам
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<string> Items(string id, string Name, string Quantity, string Unit)
        {

            string JsonOrders = _OrderModelManager.GetOrderItems(id, Name, Quantity, Unit);

            if (JsonOrders == null)
                return NotFound();

            return JsonOrders;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}