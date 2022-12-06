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
    readonly SqlConnection con = new SqlConnection();
    readonly SqlCommand com = new SqlCommand();
    SqlDataReader dr;

    void connectionString()
    {
        con.ConnectionString = "Data Source=wsb2020.database.windows.net;Initial Catalog=PD2023;Persist Security Info=True;User ID=dyplom;Password=Dypl0m2022!@#";  //baza danych
    }

    
        public IActionResult GoogleMap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SaveAddress(Models.GoogleMap acc)
        {
            try
            {
                connectionString();
                con.Open();
                com.Connection = con;
                //Console.WriteLine("K:  {0}" , Models.AccountController.userId);
                com.CommandText = "INSERT INTO PD2023.dbo.address_with_userid(user_id, miejscowosc, ulica) VALUES('1','" + acc.txtCon + "','" + acc.city + "')";
                dr = com.ExecuteReader();
                con.Close();

                return View("Bindings");

            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = JsonConvert.SerializeObject(e, Formatting.Indented);
                con.Close();
                return View("Error");
            }

        }
    }
}