using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TaskBase.Models
{
    public class PersonInfo
    {
        public int Id { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}