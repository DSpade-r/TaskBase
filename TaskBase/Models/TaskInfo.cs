using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TaskBase.Models
{
    public enum StatusSet {
        [Description("Не начата")]noStart,
        [Description("В процессе")]inProcess,
        [Description("Завершена")]completed,
        [Description("Отложена")]postponed };
    public class TaskInfo
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime Stop { get; set; }
        [Required]
        public StatusSet Status { get; set; }
        [Required]
        public string Person { get; set; }
        [ScaffoldColumn(false)]
        public List<SelectListItem> StatusList { get; set; }
        [ScaffoldColumn(false)]
        public List<SelectListItem> Persons { get; set; }
    }
}