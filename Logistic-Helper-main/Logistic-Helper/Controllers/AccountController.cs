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
            //var identity = new ClaimsIdentity("Custom");
            //HttpContext.User = new ClaimsPrincipal(identity);
            //Console.WriteLine("Zalogowano {0}", HttpContext.User.Identity.IsAuthenticated);
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
        //Console.WriteLine("ID: {0} ", userId);
        com.CommandText = "select * from PD2023.dbo.address_with_userid where user_id='" + userId + "'";
        List<List<String>> list = new List<List<String>>();
        using (SqlDataReader reader = com.ExecuteReader())
        {
            while (reader.Read())
            {
                List<String> tempList = new List<String>();
                tempList.Add(reader.GetString(2));
                if (!reader.IsDBNull(3))
                {
                    tempList.Add(reader.GetString(3));
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

    //mailer
    //[HttpPost] - psulo
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
        //return new EmptyResult();
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

//Console.WriteLine("STREET:  {0}" , street);
//Console.WriteLine("City:  {0}", city);
//var test = 'a';
//return Json(test);

public IActionResult Register()
    {
        Console.WriteLine("Hej {0}", HttpContext.User.Identity.IsAuthenticated);
        return View("Register");
    }
    //[Authorize]

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
    public IActionResult AddAttachment(IFormFile formFile)
    {
        Console.WriteLine("AddAttachment start");
        try
        {
            ViewBag.userId = Models.AccountController.userId;
            string fileName = @ViewBag.userId +"att"+ Path.GetFileName(formFile.FileName);
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



    //[HttpPost]
    //[Obsolete]
    //public ActionResult AddAttachment(IFormFile file)
    //{
    //System.Diagnostics.Debug.WriteLine("K:  JESTEM TUTAJ {0}");
    //    if (file != null)
    //    {
    //        try
    //        {
    //            string filepath = Path.Combine(("~/Attachments"),Path.GetFileName(file.FileName));
    //            //file.SaveAs(filepath);
    //            System.Diagnostics.Debug.WriteLine("K:  {0}", file);
    //            var fileName = Path.GetFileName(file.FileName);
    //            System.Diagnostics.Debug.WriteLine("Z:  {0}", fileName);
    //            file.SaveAs(filepath);
    //            ViewBag.Message = "File uploaded successfully";
    //            }
    //        catch (Exception ex)
    //        {
    //            ViewBag.Message = "ERROR:" + ex.Message.ToString();
    //        }
    //    }
    //    else
    //    {
    //        ViewBag.Message = "You have not specified a file.";
    //    }
    //    return View("Details");
    //}

    //[HttpPost]
    //public async Task<IActionResult> AddAttachment(IList<IFormFile> files)
    //{
    //    Console.WriteLine("AddAttachment start");
    //    string uploads = Path.Combine("~/Attachments");
    //    foreach (IFormFile file in files)
    //    {
    //        if (file.Length > 0)
    //        {
    //            string filePath = Path.Combine(uploads, file.FileName);
    //            using Stream fileStream = new FileStream(filePath, FileMode.Create);
    //            await file.CopyToAsync(fileStream);
    //        }
    //    }
    //    Console.WriteLine("AddAttachment ending");
    //    return View("Details");
    //}

    public IActionResult Create()
    {
        return View("Create");
    }
    public IActionResult Details(string miejscowosc, string ulica)
    {
        //Directory.GetFiles(Directory.GetCurrentDirectory(), "Attachments");
        //System.Diagnostics.Debug.WriteLine("K:  {0}", (object)test);
        //System.Diagnostics.Debug.WriteLine("J:  {0}", (object)testA);
        List<String> tempList = new List<String>();
        tempList.Add(miejscowosc);
        tempList.Add(ulica);
        ViewBag.addressesDetails = tempList;
        return View("Details");
    }


    //[HttpGet]
    //public ActionResult ReadAttachment(Models.AccountController acc)
    //{
    //    Console.WriteLine("ReadAttachment start");
    //    try
    //    {
    //        string downloadpath = Path.Combine(Directory.GetCurrentDirectory(), "Attachments");
    //        var stream = new FileStream(downloadpath, FileMode.Create);
    //        //formFile.CopyToAsync(stream);
    //        Console.WriteLine(ViewBag.Message);
    //    }
    //    catch
    //    {
    //        ViewBag.Message = "Wystąpił błąd podczas odczytu załączników";
    //    }
    //    Console.WriteLine(ViewBag.Message);
    //    return (RedirectToAction("Details"));
    //}
}
