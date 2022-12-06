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
    public class AccountController : Controller
    {

        readonly SqlConnection con = new SqlConnection();
        readonly SqlCommand com = new SqlCommand();
        SqlDataReader dr;
        //int userId;

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        void connectionString()
        {
            con.ConnectionString = "Data Source=wsb2020.database.windows.net;Initial Catalog=PD2023;Persist Security Info=True;User ID=dyplom;Password=Dypl0m2022!@#";  //baza danych
        }
        //Logowanie
        [HttpPost]
        public ActionResult Verify(Models.AccountController acc)
        {
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "select * from PD2023.dbo.login where username='" + acc.Name + "' and password='" + acc.Password + "'";
            dr = com.ExecuteReader();
            if (dr.Read())
            {

                while (dr.HasRows)
                {
                    Models.AccountController.userId = dr.GetInt32(0);
                    ViewBag.userId = dr.GetInt32(0);
                    break;
                }
                con.Close();
                GetUserAddresses(ViewBag.userId);

                return View("Bindings");

            }
            else
            {
                con.Close();
                return View("Error");
            }

        }


        public void GetUserAddresses(int userId)
        {
            con.Open();
            //Console.WriteLine("ID: {0} ", userId);
            com.CommandText = "select * from PD2023.dbo.address_with_userid where user_id='" + userId + "'";
            List<List<String>> list = new List<List<String>>();
            using (SqlDataReader reader = com.ExecuteReader())
            {
                while (reader.Read())
                {
                    List<String> tempList = new List<String>();
                    tempList.Add(reader.GetString(1));
                    if (!reader.IsDBNull(2))
                    {
                        tempList.Add(reader.GetString(2));
                    }
                    else
                    {
                        tempList.Add("no details provided");
                    }
                    list.Add(tempList);
                    //Console.WriteLine("ILE: {0} ", reader.GetString(1));
                }
                ViewBag.addressesList = list;
            }
        }

        //Rejestracja
        [HttpPost]
        public ActionResult Sign(Models.AccountController acc)
        {
            try
            {
                connectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = "INSERT INTO PD2023.dbo.login(username, password, email) VALUES('" + acc.Name + "','" + acc.Password + "','" + acc.Email + "')";
                dr = com.ExecuteReader();
                con.Close();
                return View("RegisterSuccesfull");
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = JsonConvert.SerializeObject(e, Formatting.Indented);
                con.Close();
                return View("RegisterError");
            }
        }

        //Przypisanie adresow manualne

        [HttpPost]
        public ActionResult AddAddress(Models.AccountController acc)
        {
            try
            {
                connectionString();
                con.Open();
                com.Connection = con;
                //Console.WriteLine("K:  {0}" , Models.AccountController.userId);
                com.CommandText = "INSERT INTO PD2023.dbo.address_with_userid(user_id, miejscowosc, ulica) VALUES('" + Models.AccountController.userId + "','" + acc.miejscowosc + "','" + acc.ulica + "')";
                dr = com.ExecuteReader();
                con.Close();
                GetUserAddresses(Models.AccountController.userId);
                ViewBag.userId = Models.AccountController.userId;

                return View("Bindings");

            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = JsonConvert.SerializeObject(e, Formatting.Indented);
                con.Close();
                return View("Error");
            }
        }
        //
        public IActionResult Register()
        {
            return View("Register");
        }
        public IActionResult ResetPassword()
        {
            return View("ResetPassword");
        }
        public IActionResult Create()
        {
            return View("Create");
        }
        public IActionResult Details(string miejscowosc, string ulica)
        {
            //System.Diagnostics.Debug.WriteLine("K:  {0}", (object)test);
            //System.Diagnostics.Debug.WriteLine("J:  {0}", (object)testA);
            List<String> tempList = new List<String>();
            tempList.Add(miejscowosc);
            tempList.Add(ulica);
            ViewBag.addressesDetails = tempList;
            return View("Details");
        }
        // przypisanie adresów klikając save przy mapie

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
