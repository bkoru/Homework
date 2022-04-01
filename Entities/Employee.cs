using System;
using System.Linq;

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


        private static Random rnd = new Random();
        public static int CalculateAge(DateTime birthDate)
        {
            int totalDays = (int)DateTime.Now.Subtract(birthDate).TotalDays;
            int age = totalDays / 365;
            return age;
        }

        public static DateTime RandomDate(DateTime startDate, DateTime endDate)
        {
            int range = (endDate - startDate).Days;
            int randomDays = rnd.Next(range);
            return startDate.AddDays(randomDays);
        }

        public static void CreateEmployee(DataSlot dataSlot, int total)
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
                var randomDate = RandomDate(startDate, endDate);
                var employee = new Employee(++biggerId, "Emp-" + (i + 1).ToString(), randomDate, rnd.Next(4200,10000));
                employee.Company = dataSlot.Companies[rnd.Next(companiesCount)];
                dataSlot.Employees.Add(employee);
            }
        }
    }
}
