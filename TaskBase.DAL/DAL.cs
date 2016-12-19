using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace TaskBase.DAL
{
    public class DataBase
    {
        private string connectionString;
        public DataBase(string connectionString)
        {
            this.connectionString = connectionString;
        }
        //Возвращает все задачи из базы данных ы виде списка
        public List<Task> GetListTasks()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string sqlTask = "SELECT * FROM Tasks; SELECT * FROM Persons";
            SqlDataAdapter adapter = new SqlDataAdapter(sqlTask, connection);
            DataSet dataset = new DataSet();
            adapter.Fill(dataset, "Tasks");
            DataTable tasks = dataset.Tables[0];
            DataTable persons = dataset.Tables[1];
            List<Task> Tasks = new List<Task>();
            dataset.Relations.Add("Person_Task", persons.Columns["Id"], tasks.Columns["Person_Id"]);
            foreach (DataRow row in tasks.Rows)
            {
                Task item = new Task();
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
        //Возвращает всех исполнителей из базы данных в виде списка
        public List<Person> GetListPerson()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string sqlPerson = "SELECT * FROM Persons";
            SqlDataAdapter adapter = new SqlDataAdapter(sqlPerson, connection);
            DataSet dataset = new DataSet();
            adapter.Fill(dataset, "Persons");
            DataTable persons = dataset.Tables[0];
            List<Person> Persons = new List<Person>();
            foreach (DataRow row in persons.Rows)
            {
                Person item = new Person();
                item.Id = (int)row["Id"];
                item.FirstName = row["FirstName"].ToString();
                item.LastName = row["LastName"].ToString();
                item.MiddleName = row["MiddleName"].ToString();
                Persons.Add(item);
            }
            return Persons;
        }
        //Добавляет новую задачу в базу данных
        public void AddTaskToDB(Task task)
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
        //Удаляет задачу из базы данных по идентификатору(первичному ключу)
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
        //Изменяет(редактирует) задачу в базе данных(идентификатор берется из сущности задачи)
        public void EditTaskInDB(Task task)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string sql = string.Format("Update Tasks Set Title = @Title, Description = @Description, Start = @Start, Stop = @Stop, Status = @Status, Person_Id = @Person_Id Where Id = '{0}'",
            task.Id);
            using (SqlCommand cmd = new SqlCommand(sql, connection))
            {
                connection.Open();
                cmd.Parameters.AddWithValue("@Title", task.Title);
                cmd.Parameters.AddWithValue("@Description", task.Description);
                cmd.Parameters.AddWithValue("@Start", task.Start);
                cmd.Parameters.AddWithValue("@Stop", task.Stop);
                cmd.Parameters.AddWithValue("@Status", task.Status);
                cmd.Parameters.AddWithValue("@Person_Id", task.Person);
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
        //Добавляет исполнителя в базу данных
        public void AddPersonToDB(Person person)
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
        //Удаляет исполнителя из базы данных
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
        //Изменяет(редактирует) задачу в базе данных
        public void EditPersonInDB(Person person)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string sql = string.Format("Update Persons Set FirstName = @FirstName, LastName = @LastName, MiddleName = @MiddleName Where Id = '{0}'",
            person.Id);
            using (SqlCommand cmd = new SqlCommand(sql, connection))
            {
                connection.Open();
                cmd.Parameters.AddWithValue("@FirstName", person.FirstName);
                cmd.Parameters.AddWithValue("@LastName", person.LastName);
                cmd.Parameters.AddWithValue("@MiddleName", person.MiddleName);
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
    }
}
