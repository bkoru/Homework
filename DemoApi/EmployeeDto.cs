using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework.DemoApi
{
    public class EmployeeDto
    {
        public string name { get; set; }
        public string tckn{ get; set; }
        public DateTime birthDate { get; set; }
        public decimal salary { get; set; }
    }
}
