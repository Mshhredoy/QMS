using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using QmsApp.Models;

namespace QmsApp.Controllers
{
    public class ServiceController : Controller
    {
        //
        // GET: /Service/
        QmsDbContext db = new QmsDbContext();
        public ActionResult Index()
        {

            return View(db.Services.ToList());
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Service model)
        {
            if (ModelState.IsValid)
            {
                model.Status = 1;
                model.CreateBy = Convert.ToInt32(User.Identity.GetUserName().Split('|')[0]);
                model.CreateTime = DateTime.Now;
                db.Services.Add(model);
                db.SaveChanges(User.Identity.GetUserName().Split('|')[0]);
            }
            return RedirectToAction("Index");
        } 
        public ActionResult Edit(int id)
        {
            Service model = db.Services.Find(id);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Service model)
        {
            if (ModelState.IsValid)
            {
                var objService = db.Services.Find(model.ServiceId);
                objService.ServiceName = model.ServiceName;
                objService.PossibleServiceTime = model.PossibleServiceTime;
                objService.Details = model.Details;
                objService.UpdateBy = Convert.ToInt32(User.Identity.GetUserName().Split('|')[0]);
                objService.UpdateTime = DateTime.Now;
                db.Entry(objService).State = EntityState.Modified;
                db.SaveChanges(User.Identity.GetUserName().Split('|')[0]);
            }
            return RedirectToAction("Index");
        }

        public ActionResult StatusChange(int id, int status)
        {
            var service = db.Services.Find(id);
            service.Status = status;
            db.Entry(service).State = EntityState.Modified;
            db.SaveChanges(User.Identity.GetUserName().Split('|')[0]);
            return RedirectToAction("Index");
        }
	}
}