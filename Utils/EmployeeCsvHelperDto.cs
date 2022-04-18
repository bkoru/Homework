using CsvHelper.Configuration.Attributes;
using System;

namespace Homework.Utils
{
    class EmployeeCsvHelperDto
    {
        public string Name { get; set; }
        public string TrId { get; set; }
        [Format("dd/MM/yyyy", "dd/M/yyyy", "d/M/yyyy", "d/MM/yyyy",
                "dd/MM/yy", "dd/M/yy", "d/M/yy", "d/MM/yy", "d.MM.yyyy")]
        public DateTime BirthDate { get; set; }
        public decimal Salary { get; set; }
        public int Id { get; set; }
    }
}
