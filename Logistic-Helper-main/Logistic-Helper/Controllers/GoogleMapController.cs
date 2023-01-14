using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using LogisticHelper.Models;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using NuGet.Protocol.Plugins;
using Microsoft.AspNetCore.Authorization;
namespace LogisticHelper.Controllers
{
    public class GoogleMapController : Controller
    {
        public IActionResult Index()
        {
            if (@ViewBag.userId > 0)
            {
                return View();
            }
            else
            {
                ViewBag.ErrorMessage = "Dostęp tylko dla zalogowanych użytkowników!";
                return RedirectToAction("Login", "Account");
            }
        }
    }
}