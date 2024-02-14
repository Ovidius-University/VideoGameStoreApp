﻿using System.ComponentModel.DataAnnotations;
using AppUI.Validators;
namespace AppUI.Areas.Admin.Models.DTOs;

public class ExistentOrderDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string Customer { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    [Display(Name = "Delivery Worker")]
    public int DeliveryId { get; set; }
    public string DeliveryName { get; set; } = string.Empty;
    [Display(Name = "Is it a delivery?")]
    public bool IsDelivery { get; set; } = true;
    [Required(AllowEmptyStrings = true)]
    public string Address { get; set; } = string.Empty;
    [Tip]
    public decimal Tip { get; set; }
    [Display(Name = "Paying Method")]
    public int PayingMethodId { get; set; }
    public string PayingMethod { get; set; } = string.Empty;
    [Display(Name = "Order Time")]
    public DateTime OrderTime { get; set; }
    [Display(Name = "Picked Arrival Time")]
    public DateTime ArrivalTime { get; set; }
    [Display(Name = "Delivery Time")]
    public DateTime DeliveryTime { get; set; }
    [Display(Name = "Was the order received?")]
    public bool IsFinal { get; set; }
    public void ToEntity(ref Order ExistentOrder)
    {
        if (ExistentOrder.Id == Id)
        {
            ExistentOrder.Name = Name;
            ExistentOrder.IsDelivery = IsDelivery;
            if (IsDelivery ==  false)
            {
                ExistentOrder.DeliveryId = null;
                ExistentOrder.Address = string.Empty;
            }
            else
            {
                ExistentOrder.DeliveryId = DeliveryId != 0 ? DeliveryId : null;
                ExistentOrder.Address = Address;
            }
            ExistentOrder.Tip = Tip;
            ExistentOrder.PayingMethodId = PayingMethodId;
            ExistentOrder.ArrivalTime = ArrivalTime;
            if (IsFinal == true && ExistentOrder.IsFinal == false || IsFinal == false && ExistentOrder.IsFinal == true)
            {
                ExistentOrder.DeliveryTime = DeliveryTime;
            }
            ExistentOrder.IsFinal = IsFinal;
        }
    }
}
