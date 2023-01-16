using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using System.Security.Claims;
using System.Net;
using System.Net.Mail;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using static LogisticHelper.Models.AccountController;
using System.Data;
using System.Drawing.Drawing2D;
using Microsoft.OData.Edm;
using NuGet.Packaging;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using ServiceReference1;
using System.Drawing;
using System.Reflection;
using Microsoft.AspNetCore.Session;
using System.Collections.Generic;
using System.Collections;
using NPOI.SS.Formula.Functions;
//using System.Web.Mvc;
//using Chilkat;

namespace LogisticHelper.Controllers;
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
    }    [HttpGet]
    public ActionResult Logout()
    {
        ViewBag.userId = null;
        TempData["userId"] = null;
        //Console.WriteLine(ViewBag.userId);
        ViewBag.LoginMessage = "Wylogowano";
        return View("Login");
    }
    void connectionString()
    {
        con.ConnectionString = "Data Source=wsb2020.database.windows.net;Initial Catalog=PD2023;Persist Security Info=True;User ID=dyplom;Password=Dypl0m2022!@#";  //baza danych
    }
    //hashowanie stringa
    static string ComputeSHA512(string s)
    {
        StringBuilder sb = new StringBuilder();
        using (SHA512 sha512 = SHA512.Create())
        {
            byte[] hashValue = sha512.ComputeHash(Encoding.UTF8.GetBytes(s));
            foreach (byte b in hashValue)
            {
                sb.Append($"{b:X2}");
            }
        }
        return sb.ToString();
    }
    //Logowanie
    [HttpPost]
    public ActionResult Verify(Models.AccountController acc, string ReturnUrl)
    {
        string hashValue = ComputeSHA512(acc.Password);
        connectionString();
        con.Open();
        com.Connection = con;
        com.CommandText = "select * from PD2023.dbo.login where username='" + acc.Name + "' and password='" + hashValue + "'";
        dr = com.ExecuteReader();
        if (dr.Read())
        {
            while (dr.HasRows)
            {
                Models.AccountController.userId = dr.GetInt32(0);
                TempData["userId"] = dr.GetInt32(0);
                ViewBag.userId = dr.GetInt32(0);
                break;
            }
            con.Close();
            Console.WriteLine(TempData.Peek("userId"));
            GetUserAddresses(ViewBag.userId);
        return View("Bindings");
        }
        else
        {
            con.Close();
            ViewBag.ErrorMessage = ("Błędne hasło lub konto nie istnieje");
            return View("Error");
        }
    }
    public void GetUserAddresses(int userId)
    {
        con.Open();
        com.CommandText = "select * from PD2023.dbo.address_with_userid where user_id='" + userId + "'";
        List<List<String>> list = new List<List<String>>();
        using (SqlDataReader reader = com.ExecuteReader())
        {
            while (reader.Read())
            {
                try
                {
                    List<String> myList = new List<String>();

                    myList.Add(reader.GetString(2));
                    if (!reader.IsDBNull(3) && !reader.IsDBNull(0))
                    {
                        myList.Add(reader.GetString(3));
                        string id = Convert.ToString(reader.GetInt32(0));
                        myList.Add(id);
                    }
                    else
                    {
                        myList.Add("no details provided");
                    }
                    list.Add(myList);
                }
                catch (Exception e) { }
            }
            TempData["addressesList"] = list;
            }
    }

    //mailer
    public ActionResult SendEmail(Models.AccountController acc)
    {
        Console.WriteLine("Mailer start");
        Console.WriteLine(acc.EmailToResetPass);

        var mailAddress = acc.EmailToResetPass;
        ViewBag.EmailConfirm = mailAddress;
        int _min = 1000;
        int _max = 9999;
        Random _rdm = new Random();
        var mailContent = _rdm.Next(_min, _max);
        ViewBag.EmailContent = mailContent;
        Console.WriteLine(mailContent);

        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("new.logistic.helper@gmail.com", "ennwpkkqnvrnzkbt"),
            EnableSsl = true,
        };

        smtpClient.Send("new.logistic.helper@gmail.com", mailAddress, "Token do zmiany hasła", mailContent.ToString());
        Console.WriteLine("Wyslano wiadomosc");
        return View("ResetPasswordConfirm");
    }
    //Rejestracja
    [HttpPost]
    public ActionResult Sign(Models.AccountController acc)
    {
        try
        {

            string hashValue = ComputeSHA512(acc.Password);
            Console.WriteLine(hashValue);
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "INSERT INTO PD2023.dbo.login(username, password, email) VALUES('" + acc.Name + "','" + hashValue + "','" + acc.Email + "')";
            dr = com.ExecuteReader();
            con.Close();
            return View("RegisterSuccesfull");
        }
        catch (Exception e)
        {
            ViewBag.ErrorMessage = JsonConvert.SerializeObject(e, Formatting.Indented);
            ViewBag.RegisterError = ("Podana nazwa użytkownika lub adres E-mail już istnieje, wprowadź inne dane");
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
    //Removing user
   // [HttpPost]
    public ActionResult Remove(int? id)
    {
        try
        {
            connectionString();
            con.Open();
            com.Connection = con;
           
            com.CommandText = "DELETE FROM dbo.address_with_userid WHERE id =  " + id;
            dr = com.ExecuteReader();
            con.Close();
           /* GetUserAddresses(Models.AccountController.userId);
            ViewBag.userId = Models.AccountController.userId;*/
            return View("Remove");
        }
        catch (Exception e)
        {
            ViewBag.ErrorMessage = JsonConvert.SerializeObject(e, Formatting.Indented);
            con.Close();
            return View("Error");
        }
    }
    // przypisanie adresów klikając save przy mapie
    [HttpPost]
    public ActionResult GetDataFromView(Models.AccountController acc, string street, string city)
    {
        try
            {
                connectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = "INSERT INTO PD2023.dbo.address_with_userid(user_id, miejscowosc, ulica) VALUES('" + Models.AccountController.userId + "','" + city + "','" + street + "')";
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
public IActionResult Register()
    {
        Console.WriteLine("Hej {0}", HttpContext.User.Identity.IsAuthenticated);
        return View("Register");
    }
    public IActionResult ResetPassword()
    {
        return View("ResetPassword");
    }
    //Resetowanie hasla
    [HttpPost]
    public ActionResult ResetPasswordConfirm(Models.AccountController acc)
    {
        Console.WriteLine("Reset Password Start");
        //@ViewBag.EmailContent = ViewBag.EmailContent;
        if (acc.token != acc.WrittenToken)
        {
            ViewBag.ErrorMessage = ("Wprowadzony token jest nieprawidłowy");
            return View("Error");
        }
        else
        {
            try
            {
                connectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = "UPDATE PD2023.dbo.login set password='" + acc.NewPassword + "' where email='" + acc.mailConfirmed + "'";
                dr = com.ExecuteReader();
                con.Close();
                return View("Login");
            }
            catch (Exception e)
            {

                ViewBag.ErrorMessage = JsonConvert.SerializeObject(e, Formatting.Indented);
                con.Close();
                return View("RegisterError");
            }
        }
    }

    //dodawanie załączników
    [HttpPost]
    public IActionResult AddAttachment(IFormFile formFile, int addressId)
    {
        Console.WriteLine("AddAttachment start");
        try
        {
            ViewBag.userId = Models.AccountController.userId;
            string fileName = @ViewBag.userId +"att"+addressId+ Path.GetFileName(formFile.FileName);
            string uploadpath = Path.Combine(Directory.GetCurrentDirectory(), "Attachments", fileName);
            var stream = new FileStream(uploadpath, FileMode.Create);
            formFile.CopyToAsync(stream);
            ViewBag.Message = "Zapisano załącznik";
            Console.WriteLine(ViewBag.Message);
        }
        catch
        {
            ViewBag.Message = "Wystąpił błąd podczas zapisywania załącznika";
        }
        Console.WriteLine("AddAttachment ending");
        Console.WriteLine(ViewBag.Message);
        return (RedirectToAction("Details"));
    }
    public IActionResult Create()
    {
        return View("Create");
    }
    public IActionResult Bindings()
    {
        return View("Bindings");
    }
    public IActionResult Details(Models.AccountController acc, string miejscowosc, string ulica)
    {
           //Console.WriteLine(@ViewBag.userId);
            List<String> tempList = new List<String>();
            tempList.Add(miejscowosc);
            tempList.Add(ulica);
            ViewBag.addressesDetails = tempList;
        ViewBag.userId = Models.AccountController.userId;
        DirectoryInfo dirInfo = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "Attachments"));
        List<FileInfo> files = dirInfo.GetFiles(Models.AccountController.userId + "*").ToList();
        return View(files);
    }
    public FileResult Download(string filePath, string fileName)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "Attachments", fileName);

        //Read the File data into Byte Array.
        byte[] bytes = System.IO.File.ReadAllBytes(path);

        //Send the File to Download.
        return File(bytes, "application/octet-stream", fileName);

        Console.WriteLine(path, "path");
        Console.WriteLine(fileName, "name");
        var file = File(path, System.Net.Mime.MediaTypeNames.Image.Jpeg, fileName);
        return file;
    }
}