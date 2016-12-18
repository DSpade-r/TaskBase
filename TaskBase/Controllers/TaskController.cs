using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using TaskBase.DAL;
using TaskBase.Infrastructure;
using TaskBase.Models;

namespace TaskBase.Controllers
{    
    public class TaskController : Controller
    {
        private DataBase db;

        public TaskController()
        {
            string connectionString =
            WebConfigurationManager.ConnectionStrings["TaskBase"].ConnectionString;
            db = new DataBase(connectionString);    //инициализация DAL строкой подсключения из вэбконфига
        }

        //Получение задачи из списка задач по идентификатору
        public TaskInfo GetTaskFromList(int id, List<TaskInfo> tasks)
        {
            TaskInfo task = new TaskInfo();
            for (int i = 0; i < tasks.Count; i++)
            {
                if (tasks[i].Id == id)
                    task = tasks[i];
            }
            return task;
        }

        //Формирование списка SelectListItem из описаний Enum StatusSet
        private List<SelectListItem> StatusDropDownList()
        {
            var enumData = from Models.StatusSet e in Enum.GetValues(typeof(Models.StatusSet))
                           select new { Name = e.ToStringX(), Id = e.ToString() };
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in enumData)
            {
                list.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }            
            return list;
        }

        //Формирование SelectList из коллекции bcgjkybntktq(из базы данных) для DropDownList
        private List<SelectListItem> PersonsDropDownList()
        {
            List<PersonInfo> persons = db.GetListPerson().ConvertToListPersonInfo();
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in persons)
            {
                list.Add(new SelectListItem() { Text  = item.LastName + " " + item.FirstName + " " + item.MiddleName, Value = item.Id.ToString() });
            }
            return list;
        }

        //GET - выводит список задач
        public ActionResult Index()
        {
            return View(db.GetListTasks().ConvertToListTaskInfo());
        }

        //GET - форма для удаления задачи по идентификатору(подтверждение на удаление)
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(GetTaskFromList((int)id, db.GetListTasks().ConvertToListTaskInfo()));
        }

        //POST - удаление задачи из базы данных по идентификатору(он же первичный ключ)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            db.DelTaskFromDB((int)id);
            return RedirectToAction("Index");
        }

        //GET - форма для добавления задачи
        public ActionResult Create()
        {
            //через модель передаю ListItem для DropDownList
            return View(new TaskInfo() { StatusList = StatusDropDownList(), Persons = PersonsDropDownList() });
        }

        //POST - создание(добавление) новой задачи в базе данных
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TaskInfo task)
        {
            if (ModelState.IsValid)
            {
                db.AddTaskToDB(task.ConvertToTask());
                return RedirectToAction("Index");
            }
            //через модель передаю ListItem для DropDownList
            return View(new TaskInfo() { StatusList = StatusDropDownList(), Persons = PersonsDropDownList() });
        }

        //GET - форма для изменения(редактирования) полей задачи
        public ActionResult Edit(int? id) //id из представления Index указывает на выбранную задачу
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaskInfo task = GetTaskFromList((int)id, db.GetListTasks().ConvertToListTaskInfo());
            task.Persons = PersonsDropDownList();
            task.StatusList = StatusDropDownList();
            return View(task);
        }

        //POST -  внесение измененных полей в базу данных(идентификатор берется из сущности TaskInfo)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TaskInfo taskMod)
        {
            if (ModelState.IsValid)
            {
                db.EditTaskInDB(taskMod.ConvertToTask());
                return RedirectToAction("Index");
            }
            TaskInfo task = GetTaskFromList(taskMod.Id, db.GetListTasks().ConvertToListTaskInfo());
            task.Persons = PersonsDropDownList();
            task.StatusList = StatusDropDownList();
            return View(task);
        }
       
    }
}