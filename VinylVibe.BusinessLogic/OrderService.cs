using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinylVibe.BusinessLogic.Interfaces;
using VinylVibe.DataAccess.Repository.IRepository;
using VinylVibe.Models.ViewModel;
using VinylVibe.Models;
using VinylVibe.Utility;

namespace VinylVibe.BusinessLogic
{
	public class OrderService :IOrderService
	{
		private readonly IUnitOfWork _unitOfWork;

		public OrderService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public OrderViewModel GetOrderDetails(int orderId)
		{
			var orderVM = new OrderViewModel
			{
				OrderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == orderId, includeProperties: "User"),
				OrderDetails = _unitOfWork.OrderDetails.GetAll(x => x.OrderHeaderId == orderId, includeProperties: "Product")
			};

			return orderVM;
		}

		public void UpdateOrder(OrderViewModel orderVM)
		{
			var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(x => x.Id == orderVM.OrderHeader.Id);

			orderHeaderFromDb.Name = orderVM.OrderHeader.Name;
			orderHeaderFromDb.PhoneNumber = orderVM.OrderHeader.PhoneNumber;
			orderHeaderFromDb.City = orderVM.OrderHeader.City;
			orderHeaderFromDb.Country = orderVM.OrderHeader.Country;
			orderHeaderFromDb.StreetAddress = orderVM.OrderHeader.StreetAddress;
			orderHeaderFromDb.PostalCode = orderVM.OrderHeader.PostalCode;

			if (!string.IsNullOrEmpty(orderVM.OrderHeader.Carrier))
			{
				orderHeaderFromDb.Carrier = orderVM.OrderHeader.Carrier;
			}
			if (!string.IsNullOrEmpty(orderVM.OrderHeader.TrackingNumber))
			{
				orderHeaderFromDb.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
			}
			_unitOfWork.OrderHeader.Update(orderHeaderFromDb);
			_unitOfWork.Save();
		}

		public void StartProcessingOrder(int orderId)
		{
			_unitOfWork.OrderHeader.UpdateStatus(orderId, VinylVibe.Utility.Details.StatusInProcess);
			_unitOfWork.Save();
		}

		public void ShipOrder(int orderId, string trackingNumber, string carrier)
		{
			var orderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == orderId);
			orderHeader.TrackingNumber = trackingNumber;
			orderHeader.Carrier = carrier;
			orderHeader.OrderStatus = VinylVibe.Utility.Details.StatusShipped;
			orderHeader.ShippingDate = DateTime.Now;

			if (orderHeader.PaymentStatus == VinylVibe.Utility.Details.PaymentStatusDelayedPayment)
			{
				orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(45));
			}

			_unitOfWork.OrderHeader.Update(orderHeader);
			_unitOfWork.Save();
		}

		public void CancelOrder(int orderId)
		{
			var orderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == orderId);

			if (orderHeader.PaymentStatus == VinylVibe.Utility.Details.PaymentStatusApproved)
			{
				_unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, VinylVibe.Utility.Details.StatusCancelled, VinylVibe.Utility.Details.StatusRefunded);
			}
			else
			{
				_unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, VinylVibe.Utility.Details.StatusCancelled, VinylVibe.Utility.Details.StatusCancelled);
			}

			_unitOfWork.Save();
		}

		public void CompletePayment(int orderId)
		{
			var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(x => x.Id == orderId);
			orderHeaderFromDb.PaymentDate = DateTime.Now;
			orderHeaderFromDb.PaymentStatus = VinylVibe.Utility.Details.PaymentStatusApproved;
			_unitOfWork.OrderHeader.Update(orderHeaderFromDb);
			_unitOfWork.Save();
		}

		public IEnumerable<OrderHeader> GetOrdersByStatus(string status, string userId, bool isAdmin)
		{
			
			IEnumerable<OrderHeader> objOrderList;

			
			if (isAdmin)
			{
				objOrderList = _unitOfWork.OrderHeader.GetAll(includeProperties: "User").ToList();
			}
			else
			{
				
				objOrderList = _unitOfWork.OrderHeader.GetAll(x => x.UserId == userId, includeProperties: "User").ToList();
			}

		
			switch (status)
			{
				case "inprocess":
					objOrderList = objOrderList.Where(x => x.OrderStatus == VinylVibe.Utility.Details.StatusInProcess).ToList();
					break;
				case "completed":
					objOrderList = objOrderList.Where(x => x.OrderStatus == VinylVibe.Utility.Details.StatusShipped).ToList();
					break;
				case "approved":
					objOrderList = objOrderList.Where(x => x.OrderStatus == VinylVibe.Utility.Details.StatusApproved).ToList();
					break;
				default:
					break;
			}

			return objOrderList;
		}


	}
}
