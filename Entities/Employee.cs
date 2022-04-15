﻿using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Homework.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Format("dd-MM-yyyy")]
        public DateTime BirthDate { get; set; }
        public Company Company { get; set; }
        public decimal Salary { get; set; }
        public string TrId { get; set; }

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

        public Employee(int id, string name, DateTime birthDate, decimal salary, string trId)
        {
            Id = id;
            Name = name;
            BirthDate = birthDate;
            Salary = salary;
            TrId = trId;
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

        public static string RandomTrId()
        {
            string trId = null;

            for (int i = 0; i < 1; i++)
            {
                trId += rnd.Next(1, 9);
            }

            for (int i = 1; i <= 10; i++)
            {
                trId += rnd.Next(0, 9);
            }

            return trId;
        }

        public static void CreateEmployee(DataSlot dataSlot, int total)
        {
            int companiesCount = dataSlot.Companies.Count;
            var startDate = new DateTime(1950, 01, 01);
            var endDate = new DateTime(2004, 01, 01);
            var nextId = 1;
            if (dataSlot.Employees.Count > 0)
            {
                nextId = dataSlot.Employees.Max(e => e.Id);
            }

            for (int i = 0; i < total; i++)
            {
                var trId = RandomTrId();

                while (dataSlot.Employees.Where(e => e.TrId == trId).FirstOrDefault() != null)
                {
                    trId = RandomTrId();
                }

                var randomDate = RandomDate(startDate, endDate);
                var employee = new Employee(nextId++, "Emp-" + nextId, randomDate, rnd.Next(4200,10000),trId);
                dataSlot.Employees.Add(employee);
            }
        }
    }
}
