using LogisticHelper.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using LogisticHelper.DataAccess;
using LogisticHelper.Models;
using Microsoft.AspNetCore.Authorization;

namespace LogisticHelper.Controllers
{
    public class UserController : Controller
    {

        // GET: UserController
        private readonly IUnitOfWork _unitOfWork;
        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        // GET: BookCoverController

        public IActionResult Index()
        {

            IEnumerable<User> objUserList = _unitOfWork.User.GetAll();
            return View(objUserList);
        }
        // GET: UserController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User obj)
        {
            /*Some errors for fun*/
            if (obj.LAST_NAME == obj.NAME)
            {
                ModelState.AddModelError("IdenticalNameToCustomName", "Rodzina bardzo kogoś nie kocha");


            }
            if (obj.NAME == null)
            {
                ModelState.AddModelError("name", "Nazwa nie może być pusta! :/");
            }

            /*if everything correct, create row and go to index*/
            if (ModelState.IsValid)
            {
                _unitOfWork.User.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "User created successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        // GET: UserController/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var getUser = _unitOfWork.User.GetFirstOrDefault(u => u.ID == id);

            if (getUser == null)
            {
                return NotFound();
            }
            return View(getUser);
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User entity)
        {
            if (entity.NAME == null)
            {
                ModelState.AddModelError("NullName", "Name cannot be empty :(");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.User.Update(entity);
                _unitOfWork.Save();
                TempData["success"] = "User got edited";
                return RedirectToAction("Index");
            }
            return View(entity);


        }

        // GET: UserController/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            var getUserToDelete = _unitOfWork.User.GetFirstOrDefault(u => u.ID == id);

            if (getUserToDelete == null)
                return NotFound();


            return View(getUserToDelete);
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var obj = _unitOfWork.User.GetFirstOrDefault(u => u.ID == id); ;
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.User.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "User deleted successfully";

            return RedirectToAction("Index");
        }

    }
}