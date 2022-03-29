using Homework.Settings;
using Homework.Utils;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Homework
{
    class Program
    {
        static DataSlot _dataSlot;
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false);
            var config = builder.Build();
            var appSettings = config.Get<AppSettings>();

            _dataSlot = new DataSlot(appSettings);

            _dataSlot.LoadFromCsv();

            int companyCount = 0;
            Console.Write("Eklemek istediğiniz şirket sayısını giriniz: ");
            while (!int.TryParse(Console.ReadLine(), out companyCount))
            {
                Console.Clear();
                Console.WriteLine("Lütfen geçerli bir sayı giriniz");
                Console.Write("Eklemek istediğiniz şirket sayısını giriniz: ");
            }

            Company.CreateCompany(_dataSlot, companyCount);

            Console.WriteLine("----------------Get Company List----------------");

            foreach (var company in _dataSlot.Companies)
            {
                Console.WriteLine("{0} {1}", company.Id, company.Name);
            }

            Console.WriteLine("Create Employee from Class void");

            int employeeCount = 0;
            Console.Write("Eklemek istediğiniz çalışan sayısını giriniz: ");

            while (!int.TryParse(Console.ReadLine(), out employeeCount))
            {
                Console.WriteLine("Lütfen geçerli bir sayı giriniz");
                Console.Write("Eklemek istediğiniz çalışan sayısını giriniz: ");
            }

            Employee.CreateEmployee(_dataSlot, employeeCount);
            _dataSlot.SaveToCsv();


            Console.WriteLine("----------------Get Employee List----------------");

            foreach (var employee in _dataSlot.Employees)
            {
                Console.WriteLine("Employee : Id:{0} Name:{1} BirthDate:{2} Company Name:{3} Company Id:{4} Salary:{5} TL Age:{6}",
                    employee.Id, employee.Name, employee.BirthDate.ToString("dd.MM.yyyy"), employee.Company?.Name, employee.Company?.Id, employee.Salary, employee.Age);
            }

            Console.WriteLine("----------------Company list with Odd ID----------------");
            var resultOddId = _dataSlot.Companies.Where(c => c.Id % 2 == 1);
            foreach (var company in resultOddId)
            {
                Console.WriteLine(company.Name);
            }

            Console.WriteLine("----------------Employee list order by Salary----------------");
            var resultSalary = _dataSlot.Employees.OrderByDescending(e => e.Salary);

            foreach (Employee employee in resultSalary)
            {
                Console.WriteLine("{0}: {1} TL", employee.Name, employee.Salary);
            }

            Console.ReadLine();
        }
    }
    public class DataSlot
    {
        private readonly AppSettings _appSettings;
        public List<Company> Companies { get; set; }
        public List<Employee> Employees { get; set; }

        public DataSlot(AppSettings appSettings)
        {
            Companies = new List<Company>();
            Employees = new List<Employee>();
            _appSettings = appSettings;
        }

        private void LoadCompaniesFromCsv()
        {
        }

        private void LoadEmployeesFromCsv()
        {
            var empStrList = CsvHandler.ReadCsv(_appSettings.FilePaths.Employees);
            foreach (var item in empStrList)
            {
                var fields = item.Split(';');
                Employees.Add(new Employee()
                {
                    Id = int.Parse(fields[0]),
                    Name = fields[1],
                    BirthDate = DateTime.ParseExact(fields[2], "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture),
                    Company = Companies.Where(e => e.Id == int.Parse(fields[3])).FirstOrDefault(),
                    Salary = decimal.Parse(fields[4]),
                    Age = int.Parse(fields[5])
                });
            }
        }

        public void LoadFromCsv()
        {
            LoadCompaniesFromCsv();
            LoadEmployeesFromCsv();
        }

        private void SaveEmployeesToCsv()
        {
            var lines = new List<string>();
            lines.Add("Id;Name;BirthDate;CompanyId;Salary;Age");

            foreach (var item in Employees)
            {
                lines.Add(item.Id + ";" + item.Name + ";" + item.BirthDate.ToString("dd.MM.yyyy") + ";" + item.Company?.Id + ";" + item.Salary + ";" + item.Age);
            }
            if (Directory.Exists(_appSettings.FilePaths.Employees))
            {
                CsvHandler.WriteCsv(_appSettings.FilePaths.Employees, lines);
            }
            else
            {
                Console.WriteLine("Dosya yolu bulunamadı");
            }
        }

        private void SaveCompaniesToCsv()
        {

        }

        public void SaveToCsv()
        {
            SaveEmployeesToCsv();
            SaveCompaniesToCsv();
        }
    }

    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Company()
        {
        }
        public Company(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public Company(DataSlot dataSlot, List<Company> companies)
        {
            dataSlot.Companies = companies;
        }

        public static void CreateCompany(DataSlot dataSlot, int total)
        {
            for (int i = 0; i < total; i++)
            {
                var company = new Company(i + 1, "Company" + (i + 1).ToString());
                dataSlot.Companies.Add(company);
            }
        }
    }

    public class Employee
    {
        public int Id;
        public string Name;
        public DateTime BirthDate;
        public Company Company;
        public decimal Salary;
        public int Age;

        public Employee()
        {
        }

        public Employee(int id, string name, DateTime birthDate, decimal salary, int age)
        {
            Id = id;
            Name = name;
            BirthDate = birthDate;
            Salary = salary;
            Age = age;
        }

        public static void CreateEmployee(DataSlot dataSlot, int total)
        {
            int companiesCount = dataSlot.Companies.Count;
            var startDate = new DateTime(1950, 01, 01);
            var endDate = new DateTime(2004, 01, 01);
            for (int i = 0; i < total; i++)
            {
                var randomId = RandomUtils.RandomId();
                while (dataSlot.Employees.Where(e => e.Id == randomId).FirstOrDefault() != null)
                {
                    randomId = RandomUtils.RandomId();
                }
                var randomDate = RandomUtils.RandomDate(startDate, endDate);
                var employee = new Employee(randomId, "Emp-" + (i + 1).ToString(), randomDate, RandomUtils.RandomSalary(4200,10000), RandomUtils.AgeCalculate(randomDate));
                employee.Company = dataSlot.Companies[RandomUtils.RandomIndex(companiesCount)];
                dataSlot.Employees.Add(employee);
            }
        }
    }
    public class CsvHandler
    {
        public static List<string> ReadCsv(string filePath, bool hasHeader = true)
        {
            List<string> lines = new List<string>();

            if (!File.Exists(filePath))
            {
                return lines;
            }

            lines = File.ReadAllLines(filePath).ToList();

            if (hasHeader)
            {
                lines.RemoveAt(0);
            }
            return lines.Where(e => !String.IsNullOrEmpty(e)).ToList();
        }

        public static void WriteCsv(string filePath, List<string> lines)
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                foreach (var item in lines)
                {
                    sw.WriteLine(item);
                }
            }
        }
    }
    
}
