﻿using LogisticHelper.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ServiceModel;
using System.Linq;


namespace LogisticHelper.Controllers
{
    public class TerytController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public TerytController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        // GET: TerytController
        public ActionResult Index()
        {
          /*  serviceteryt.TerytWs1Client client = new serviceteryt.TerytWs1Client();*/


           



           //var cos =  client.PobierzListeMiejscowosciWGminieAsync( "śląskie", "gliwicki","gierałtowice",DateTime.Now).Result;
            //LINQ To get correct data
           
            
            return View();



        }

        // GET: TerytController/Details/5
        public ActionResult Details(int id)
        {
           
            return View();
        }

        // GET: TerytController/Create
        public ActionResult Search()
        {
          
            return View();
        }

        // POST: TerytController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        //Trzeba zrobić listę ze wszystkimi miastami :/
       

        // GET: TerytController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TerytController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TerytController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TerytController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
