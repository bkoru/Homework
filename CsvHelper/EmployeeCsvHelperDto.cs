using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework.CsvHelper
{
    class EmployeeCsvHelperDto
    {
        public string Name { get; set; }
        public string TrId { get; set; }
        [Format("dd/MM/yyyy", "dd/M/yyyy", "d/M/yyyy", "d/MM/yyyy",
                "dd/MM/yy", "dd/M/yy", "d/M/yy", "d/MM/yy", "d.MM.yyyy")]
        public DateTime BirthDate { get; set; }
        public decimal Salary { get; set; }
    }
}
