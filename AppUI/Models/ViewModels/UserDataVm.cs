﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AppUI.Validators;
using AppUI.Models.ViewModels;
namespace AppUI.Models.ViewModels;
public class UserDataVm
{
    public int UserId { get; set; }
    public string User { get; set; } = string.Empty;
    public int GenderId { get; set; }
    public string Gender { get; set; } = string.Empty;
    [DataType(DataType.Date), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
    public DateTime Birthday { get; set; }
}
