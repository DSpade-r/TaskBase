using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using TaskBase.Models;

namespace TaskBase.Controllers
{
    public class PersonController : Controller
    {
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
        public void AddPersonToDB(PersonInfo person)
        {
            string connectionString =
            WebConfigurationManager.ConnectionStrings["TaskBase"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            string sql = string.Format("Insert Into Persons" +
                   "(FirstName, LastName, MiddleName) Values(@FirstName, @LastName, @MiddleName)");

            using (SqlCommand cmd = new SqlCommand(sql, connection))
            {
                connection.Open();
                // Добавить параметры
                cmd.Parameters.AddWithValue("@FirstName", person.FirstName);
                cmd.Parameters.AddWithValue("@LastName", person.LastName);
                cmd.Parameters.AddWithValue("@MiddleName", person.MiddleName);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }
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
        public void DelPersonFromDB(int id)
        {
            string connectionString =
            WebConfigurationManager.ConnectionStrings["TaskBase"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            string sql = string.Format("Delete from Persons where Id = '{0}'", id);
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
        public void EditPersonInDB(PersonInfo person)
        {
            string connectionString =
                        WebConfigurationManager.ConnectionStrings["TaskBase"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            string sqlPerson = "SELECT * FROM Persons";
            SqlDataAdapter adapter = new SqlDataAdapter(sqlPerson, connection);
            DataSet dataset = new DataSet();
            adapter.Fill(dataset, "Persons");
            DataTable persons = dataset.Tables[0];
            persons.Rows[0].Table.PrimaryKey = new DataColumn[] { persons.Columns[0] };
            DataRow row = persons.Rows.Find(person.Id);
            row.BeginEdit();
            row["FirstName"] = person.FirstName;
            row["LastName"] = person.LastName;
            row["MiddleName"] = person.MiddleName;
            row.EndEdit();
            SqlCommandBuilder objCommandBuilder = new SqlCommandBuilder(adapter);
            adapter.Update(dataset, "Persons");
        }
        // GET: Person
        public ActionResult Index()
        {
            return View(GetListPerson());
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PersonInfo person)
        {
            if (ModelState.IsValid)
            {
                AddPersonToDB(person);
                return RedirectToAction("Index");
            }
            return View();
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(GetPersonFromList((int)id, GetListPerson()));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            DelPersonFromDB((int)id);
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(GetPersonFromList((int)id, GetListPerson()));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PersonInfo personMod)
        {
            if (ModelState.IsValid)
            {
                EditPersonInDB(personMod);
                return RedirectToAction("Index");
            }
            return View(GetPersonFromList(personMod.Id, GetListPerson()));
        }
    }
}