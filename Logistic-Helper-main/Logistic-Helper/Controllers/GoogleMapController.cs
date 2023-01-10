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
namespace LogisticHelper.Controllers
{
    public class GoogleMapController : Controller
    { 
        public IActionResult Index()
        {
            return View();
        }
    }
}