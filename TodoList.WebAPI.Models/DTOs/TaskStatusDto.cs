using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoList.WebAPI.Models.DTOs
{
    public class TaskStatusDto
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }
}
