using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class SQLDB
    {
        private static string connectionString;
        public SQLDB(string connection)
        {
            connectionString = connection;
        }
        public List<PersonInfoDB> GetListPerson()
        {            
            SqlConnection connection = new SqlConnection(connectionString);
            string sqlPerson = "SELECT * FROM Persons";
            SqlDataAdapter adapter = new SqlDataAdapter(sqlPerson, connection);
            DataSet dataset = new DataSet();
            adapter.Fill(dataset, "Persons");
            DataTable persons = dataset.Tables[0];
            List<PersonInfoDB> Persons = new List<PersonInfoDB>();
            foreach (DataRow row in persons.Rows)
            {
                PersonInfoDB item = new PersonInfoDB();
                item.Id = (int)row["Id"];
                item.FirstName = row["FirstName"].ToString();
                item.LastName = row["LastName"].ToString();
                item.MiddleName = row["MiddleName"].ToString();
                Persons.Add(item);
            }
            return Persons;
        }
        public static void AddPersonToDB(PersonInfoDB person)
        {
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
        public PersonInfoDB GetPersonFromList(int id, List<PersonInfoDB> persons)
        {
            PersonInfoDB person = new PersonInfoDB();
            for (int i = 0; i < persons.Count; i++)
            {
                if (persons[i].Id == id)
                    person = persons[i];
            }
            return person;
        }
        public void DelPersonFromDB(int id)
        {            
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
        public void EditPersonInDB(PersonInfoDB person)
        {
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
        public List<TaskInfoDB> GetListTasks()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string sqlTask = "SELECT * FROM Tasks; SELECT * FROM Persons";
            SqlDataAdapter adapter = new SqlDataAdapter(sqlTask, connection);
            DataSet dataset = new DataSet();
            adapter.Fill(dataset, "Tasks");
            DataTable tasks = dataset.Tables[0];
            DataTable persons = dataset.Tables[1];
            List<TaskInfoDB> Tasks = new List<TaskInfoDB>();
            dataset.Relations.Add("Person_Task", persons.Columns["Id"], tasks.Columns["Person_Id"]);
            foreach (DataRow row in tasks.Rows)
            {
                TaskInfoDB item = new TaskInfoDB();
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
        public void AddTaskToDB(TaskInfoDB task)
        {
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
                cmd.Parameters.AddWithValue("@Status", task.Status);
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
        public TaskInfoDB GetTaskFromList(int id, List<TaskInfoDB> tasks)
        {
            TaskInfoDB task = new TaskInfoDB();
            for (int i = 0; i < tasks.Count; i++)
            {
                if (tasks[i].Id == id)
                    task = tasks[i];
            }
            return task;
        }
        public void DelTaskFromDB(int id)
        {
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
        public void EditTaskInDB(TaskInfoDB task)
        {            
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
    }
}
