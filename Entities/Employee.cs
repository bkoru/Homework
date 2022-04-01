using Homework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public Company Company { get; set; }
        public decimal Salary { get; set; }

        public int Age
        {
            get
            {
                return CalculateAge(BirthDate);
            }
        }

        public Employee()
        {
        }

        public Employee(int id, string name, DateTime birthDate, decimal salary)
        {
            Id = id;
            Name = name;
            BirthDate = birthDate;
            Salary = salary;
        }

        public static int CalculateAge(DateTime birthDate)
        {
            int totalDays = (int)DateTime.Now.Subtract(birthDate).TotalDays;
            int age = totalDays / 365;
            return age;
        }

        public static void CreateEmployee(DataSlot dataSlot, int total, decimal salary)
        {
            int companiesCount = dataSlot.Companies.Count;
            var startDate = new DateTime(1950, 01, 01);
            var endDate = new DateTime(2004, 01, 01);
            var biggerId = 0;
            if (dataSlot.Employees.Count > 0)
            {
                biggerId = dataSlot.Employees.Max(e => e.Id);
            }

            for (int i = 0; i < total; i++)
            {
                var randomDate = RandomUtils.RandomDate(startDate, endDate);
                var employee = new Employee(++biggerId, "Emp-" + (i + 1).ToString(), randomDate, salary);
                employee.Company = dataSlot.Companies[RandomUtils.RandomIndex(companiesCount)];
                dataSlot.Employees.Add(employee);
            }
        }
    }
}
