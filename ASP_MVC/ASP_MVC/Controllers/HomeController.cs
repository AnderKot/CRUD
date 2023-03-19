﻿using ASP_MVC.Models;
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

        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult CRUD()
        {
            return View(_CRUDModelManager.GetCRUDModel());
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Order(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            OrderModel model = _OrderModelManager.GetOrder(id);
            if (model == null)
                return NotFound();

            return PartialView(model);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<string> Orders(string DateFrom, string DateTo, string Number, string Provider)
        {
            if ((DateFrom == null) | (DateTo == null))
                return BadRequest();
            
            string JsonOrders = _OrderModelManager.GetOrders(DateFrom, DateTo, Number, Provider);
            
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