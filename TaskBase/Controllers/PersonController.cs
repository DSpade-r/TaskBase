using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using TaskBase.DAL;
using TaskBase.Infrastructure;
using TaskBase.Models;

namespace TaskBase.Controllers
{
    public class PersonController : Controller
    {
        private DataBase db;    //Подключение DAL-уровня 
        public PersonController()
        {
            string connectionString =
            WebConfigurationManager.ConnectionStrings["TaskBase"].ConnectionString;
            db = new DataBase(connectionString);    //инициализация DAL строкой подсключения из вэбконфига
        }

        //метод получения информациюю об исполнителе из списка исполнителей по идентификатору
        public PersonInfo GetPersonFromList(int id, List<PersonInfo> persons)
        {
            PersonInfo person = new PersonInfo();
            for (int i = 0; i < persons.Count; i++)
            {
                if (persons[i].Id == id)
                    person = persons[i];
            }
            return person;
        }
        
        //GET - выводит список исполнителей
        public ActionResult Index()
        {
            return View(db.GetListPerson().ConvertToListPersonInfo());
        }

        //GET - форма для создания нового исполнителя
        public ActionResult Create()
        {
            return View();
        }

        //POST - создание нового исполнителя в БД по переданной форме
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PersonInfo person)
        {
            if (ModelState.IsValid)
            {
                db.AddPersonToDB(person.ConvertToPerson());
                return RedirectToAction("Index");
            }
            return View();
        }

        //GET - форма для удаления исполнителя из списка(детальная информация и подтверждение)
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(GetPersonFromList((int)id, db.GetListPerson().ConvertToListPersonInfo()));
        }

        //POST - удаление исполнителя из базы данных по идентификатору(первичному ключу)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            db.DelPersonFromDB((int)id);
            return RedirectToAction("Index");
        }

        //GET - форма для изменения исполнителя
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(GetPersonFromList((int)id, db.GetListPerson().ConvertToListPersonInfo()));
        }

        //POST - изменение характеристик исполнителя в базе данных
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PersonInfo personMod)
        {
            if (ModelState.IsValid)
            {
                db.EditPersonInDB(personMod.ConvertToPerson());
                return RedirectToAction("Index");
            }
            return View(GetPersonFromList(personMod.Id, db.GetListPerson().ConvertToListPersonInfo()));
        }
    }
}