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
using TaskBase.Infrastructure;
using TaskBase.Models;

namespace TaskBase.Controllers
{    
    public class TaskController : Controller
    {

        public List<TaskInfo> GetListTasks()
        {
            string connectionString =
            WebConfigurationManager.ConnectionStrings["TaskBase"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            string sqlTask = "SELECT * FROM Tasks; SELECT * FROM Persons";
            SqlDataAdapter adapter = new SqlDataAdapter(sqlTask, connection);
            DataSet dataset = new DataSet();
            adapter.Fill(dataset, "Tasks");
            DataTable tasks = dataset.Tables[0];
            DataTable persons = dataset.Tables[1];
            List<TaskInfo> Tasks = new List<TaskInfo>();
            dataset.Relations.Add("Person_Task", persons.Columns["Id"], tasks.Columns["Person_Id"]);
            foreach (DataRow row in tasks.Rows)
            {
                TaskInfo item = new TaskInfo();
                item.Id = (int)row["Id"];
                item.Title = row["Title"].ToString();
                item.Description = row["Description"].ToString();
                item.Start = (DateTime)row["Start"];
                item.Stop = (DateTime)row["Stop"];
                item.Status = (StatusSet)row["Status"];
                var personRow = row.GetParentRow("Person_Task");
                item.Person = personRow[2] + " " + personRow[1] + " " + personRow[3];
                Tasks.Add(item);
            }
            return Tasks;
        }
        public List<PersonInfo> GetListPerson()
        {
            string connectionString =
            WebConfigurationManager.ConnectionStrings["TaskBase"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            string sqlPerson = "SELECT * FROM Persons";
            SqlDataAdapter adapter = new SqlDataAdapter(sqlPerson, connection);
            DataSet dataset = new DataSet();
            adapter.Fill(dataset, "Persons");
            DataTable persons = dataset.Tables[0];
            List<PersonInfo> Persons = new List<PersonInfo>();
            foreach (DataRow row in persons.Rows)
            {
                PersonInfo item = new PersonInfo();
                item.Id = (int)row["Id"];
                item.FirstName = row["FirstName"].ToString();
                item.LastName = row["LastName"].ToString();
                item.MiddleName = row["MiddleName"].ToString();
                Persons.Add(item);
            }
            return Persons;
        }
        public void AddTaskToDB(TaskInfo task)
        {
            string connectionString =
            WebConfigurationManager.ConnectionStrings["TaskBase"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            string sql = string.Format("Insert Into Tasks" +
                   "(Title, Description, Start, Stop, Status, Person_Id) Values(@Title, @Description, @Start, @Stop, @Status, @Person_Id)");

            using (SqlCommand cmd = new SqlCommand(sql, connection))
            {
                connection.Open();
                // Добавить параметры
                cmd.Parameters.AddWithValue("@Title", task.Title);
                cmd.Parameters.AddWithValue("@Description", task.Description);
                cmd.Parameters.AddWithValue("@Start", task.Start);
                cmd.Parameters.AddWithValue("@Stop", task.Stop);
                cmd.Parameters.AddWithValue("@Status", task.Status );
                cmd.Parameters.AddWithValue("@Person_Id", task.Person);

                cmd.ExecuteNonQuery();
                connection.Close();
            }
            using (SqlCommand cmd = new SqlCommand(sql, connection))
            {
                connection.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    Console.Write(ex.Message);
                }
                connection.Close();
            }
        }
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
        public void DelTaskFromDB(int id)
        {
            string connectionString =
            WebConfigurationManager.ConnectionStrings["TaskBase"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            string sql = string.Format("Delete from Tasks where Id = '{0}'", id);
            //connection.Open();
            using (SqlCommand cmd = new SqlCommand(sql, connection))
            {
                connection.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    Console.Write(ex.Message);
                }
                connection.Close();
            }
        }
        public void EditTaskInDB(TaskInfo task)
        {
            string connectionString =
                        WebConfigurationManager.ConnectionStrings["TaskBase"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            string sqlTask = "SELECT * FROM Tasks";
            SqlDataAdapter adapter = new SqlDataAdapter(sqlTask, connection);
            DataSet dataset = new DataSet();
            adapter.Fill(dataset, "Tasks");
            DataTable tasks = dataset.Tables[0];
            tasks.Rows[0].Table.PrimaryKey = new DataColumn[] { tasks.Columns[0] };
            DataRow row = tasks.Rows.Find(task.Id);
            row.BeginEdit();
            row["Title"] = task.Title;
            row["Description"] = task.Description;
            row["Start"] = task.Start;
            row["Stop"] = task.Stop;
            row["Status"] = task.Status;
            row["Person_Id"] = task.Person;
            row.EndEdit();
            SqlCommandBuilder objCommandBuilder = new SqlCommandBuilder(adapter);
            adapter.Update(dataset, "Tasks");            
        }
        public TaskController()
        {

        }
        public ActionResult Index()
        {
            return View(GetListTasks());
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(GetTaskFromList((int)id, GetListTasks()));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            DelTaskFromDB((int)id);
            return RedirectToAction("Index");
        }
        public ActionResult Create()
        {
            return View(new TaskInfo() { StatusList = StatusDropDownList(), Persons = PersonsDropDownList() });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TaskInfo task)
        {
            if (ModelState.IsValid)
            {
                AddTaskToDB(task);
                return RedirectToAction("Index");
            }
            return View(new TaskInfo() { StatusList = StatusDropDownList(), Persons = PersonsDropDownList() });
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaskInfo task = GetTaskFromList((int)id, GetListTasks());
            task.Persons = PersonsDropDownList();
            task.StatusList = StatusDropDownList();
            return View(task);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TaskInfo taskMod)
        {
            if (ModelState.IsValid)
            {
                EditTaskInDB(taskMod);
                return RedirectToAction("Index");
            }
            TaskInfo task = GetTaskFromList(taskMod.Id, GetListTasks());
            task.Persons = PersonsDropDownList();
            task.StatusList = StatusDropDownList();
            return View(task);
        }
        private List<SelectListItem> StatusDropDownList()
        {
            var enumData = from StatusSet e in Enum.GetValues(typeof(StatusSet))
                           select new { Name = e.ToStringX(), Id = e.ToString() };
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in enumData)
            {
                list.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }            
            return list;
        }
        private List<SelectListItem> PersonsDropDownList()
        {
            List<PersonInfo> persons = GetListPerson();
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in persons)
            {
                list.Add(new SelectListItem() { Text  = item.LastName + " " + item.FirstName + " " + item.MiddleName, Value = item.Id.ToString() });
            }
            return list;
        }
    }
}