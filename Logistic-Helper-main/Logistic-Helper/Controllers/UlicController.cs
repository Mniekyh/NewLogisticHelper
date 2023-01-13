using LogisticHelper.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServiceReference1;
using Microsoft.AspNetCore.Authorization;

namespace LogisticHelper.Controllers
{
    public class UlicController : Controller
    {
        public TerytWs1Client connection()
        {
            ServiceReference1.TerytWs1Client client = new ServiceReference1.TerytWs1Client();
            /*  serviceteryt.TerytWs1Client client = new serviceteryt.TerytWs1Client();*/


            client.ClientCredentials.UserName.UserName = "Mariusz.Sobota";
            client.ClientCredentials.UserName.Password = "so6QT8ahG";
            client.OpenAsync().Wait();
            return client;
        }
        // GET: UlicController
        public async Task<ActionResult> IndexAsync()
        {
            TerytWs1Client client = connection();
            string woj = TempData["WOJ"] as string;
            string pow = TempData["POW"] as string;
            string gmi = TempData["GMI"] as string;
            string rodz = TempData["RODZ"] as string;
            string sym = TempData["SYM"] as string;


            List<UlicaDrzewo[]> city = new List<UlicaDrzewo[]>();
            city.Add(await client.PobierzListeUlicDlaMiejscowosciAsync(woj, pow, gmi, rodz, sym, true, false, DateTime.Now));
            List<UlicaDrzewo> streets = new List<UlicaDrzewo>();
            foreach (var st in city)
            {
                for (int i = 0; i < st.Length; i++)
                {
                    streets.Add(st[i]);

                }

            }

            TempData["wojewodztwo2"] = TempData["wojewodztwo"];
            TempData["powiat2"] = TempData["powiat"];
            TempData["nazwa2"] = TempData["nazwa"];

            return View(streets);
        }


        // GET: UlicController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UlicController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UlicController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(IndexAsync));
            }
            catch
            {
                return View();
            }
        }

        // GET: UlicController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UlicController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(IndexAsync));
            }
            catch
            {
                return View();
            }
        }

        // GET: UlicController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UlicController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(IndexAsync));
            }
            catch
            {
                return View();
            }
        }
    }
}
