﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using VinylVibe.BusinessLogic.Interfaces;
using VinylVibe.DataAccess.Repository.IRepository;
using VinylVibe.Models;
using VinylVibe.Models.ViewModel;
using VinylVibe.Utility;

namespace VinylVibeWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        

       
        [BindProperty]
    public OrderViewModel OrderVM { get; set; }

    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Details(int orderId)
    {
        var orderVM = _orderService.GetOrderDetails(orderId);
        return View(orderVM);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult UpdateOrder(int orderId)
    {
        _orderService.UpdateOrder(OrderVM);
        TempData["Success"] = "Order updated successfully!";
        return RedirectToAction("Details", new { orderId = OrderVM.OrderHeader.Id });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult StartProcessing()
    {
        _orderService.StartProcessingOrder(OrderVM.OrderHeader.Id);
        TempData["Success"] = "Order updated successfully!";
        return RedirectToAction("Details", new { orderId = OrderVM.OrderHeader.Id });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult ShipOrder()
    {
        _orderService.ShipOrder(OrderVM.OrderHeader.Id, OrderVM.OrderHeader.TrackingNumber, OrderVM.OrderHeader.Carrier);
        TempData["Success"] = "Order shipped successfully!";
        return RedirectToAction("Details", new { orderId = OrderVM.OrderHeader.Id });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult CancelOrder()
    {
        _orderService.CancelOrder(OrderVM.OrderHeader.Id);
        TempData["Success"] = "Order cancelled successfully!";
        return RedirectToAction("Details", new { orderId = OrderVM.OrderHeader.Id });
    }

    [HttpPost]
    [ActionName("Details")]
    public IActionResult DetailsPayNow()
    {
        _orderService.CompletePayment(OrderVM.OrderHeader.Id);
        TempData["Success"] = "Order payment completed successfully!";
        return RedirectToAction("Details", new { orderId = OrderVM.OrderHeader.Id });
    }

    [HttpGet]
    public IActionResult GetAll(string status)
    {
			var claimIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			bool isAdmin = User.IsInRole("Admin");
			var orders = _orderService.GetOrdersByStatus(status, userId, isAdmin);
			
			

			return Json(new { data = orders });
		}
    }
}
