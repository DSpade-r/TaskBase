using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaskBase.DAL;
using TaskBase.Models;

namespace TaskBase.Infrastructure
{
    public static class Mapper
    {
        public static PersonInfo ConvertToPersonInfo(this Person person)
        {
            return new PersonInfo
            {
                Id = person.Id,
                FirstName = person.FirstName,
                LastName = person.LastName,
                MiddleName = person.MiddleName
            };
        }
        public static Person ConvertToPerson(this PersonInfo personInfo)
        {
            return new Person
            {
                Id = personInfo.Id,
                FirstName = personInfo.FirstName,
                LastName = personInfo.LastName,
                MiddleName = personInfo.MiddleName
            };
        }
        public static TaskInfo ConvertToTaskInfo(this Task task)
        {
            return new TaskInfo
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Start = task.Start,
                Stop = task.Stop,
                Status = (Models.StatusSet)task.Status,
                Person = task.Person,
                Persons = new List<System.Web.Mvc.SelectListItem>(),
                StatusList = new List<System.Web.Mvc.SelectListItem>()
            };
        }
        public static Task ConvertToTask(this TaskInfo taskInfo)
        {
            return new Task
            {
                Id = taskInfo.Id,
                Title = taskInfo.Title,
                Description = taskInfo.Description,
                Start = taskInfo.Start,
                Stop = taskInfo.Stop,
                Status = (DAL.StatusSet)taskInfo.Status,
                Person = taskInfo.Person
            };
        }
        public static List<PersonInfo> ConvertToListPersonInfo(this List<Person> persons)
        {
            List<PersonInfo> PersonsInfo = new List<PersonInfo>();
            foreach (var person in persons)
            {
                PersonsInfo.Add(person.ConvertToPersonInfo());
            }
            return PersonsInfo;
        }
        public static List<TaskInfo> ConvertToListTaskInfo(this List<Task> tasks)
        {
            List<TaskInfo> TasksInfo = new List<TaskInfo>();
            foreach (var task in tasks)
            {
                TasksInfo.Add(task.ConvertToTaskInfo());
            }
            return TasksInfo;
        }
    }
}