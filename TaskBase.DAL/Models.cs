using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskBase.DAL
{
    public enum StatusSet
    {
        noStart,
        inProcess,
        completed,
        postponed
    };
    public class Person
    {
        public int Id { get; set; }
        public string MiddleName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime Stop { get; set; }
        public StatusSet Status { get; set; }
        public string Person { get; set; }
    }
}
