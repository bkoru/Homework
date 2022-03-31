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
        private static DataSlot _dataSlot = new DataSlot();

        private static AppSettings _appSettings;
        
        static void Main(string[] args)
        {
            ConfigureSettings();
            LoadFromFile();
            CompanyCreate();
            EmployeeCreate();
            SaveToFile();
            CompanyLists();
            EmployeeLists();

            Console.ReadLine();
        }

        private static void ConfigureSettings()
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false);
            var config = builder.Build();
            _appSettings = config.Get<AppSettings>();
        }

        private static void CompanyCreate()
        {
            int companyCount = 0;
            Console.Write("Eklemek istediğiniz şirket sayısını giriniz: ");
            while (!int.TryParse(Console.ReadLine(), out companyCount))
            {
                Console.Clear();
                Console.WriteLine("Lütfen geçerli bir sayı giriniz");
                Console.Write("Eklemek istediğiniz şirket sayısını giriniz: ");
            }

            Company.CreateCompany(_dataSlot, companyCount);

        }

        private static void EmployeeCreate()
        {
            int employeeCount = 0;
            Console.Write("Eklemek istediğiniz çalışan sayısını giriniz: ");

            while (!int.TryParse(Console.ReadLine(), out employeeCount))
            {
                Console.WriteLine("Lütfen geçerli bir sayı giriniz");
                Console.Write("Eklemek istediğiniz çalışan sayısını giriniz: ");
            }

            Employee.CreateEmployee(_dataSlot, employeeCount);
        }

        private static void LoadFromFile()
        {
            var _fileHandler = new FileHandler(_appSettings, _dataSlot);
            _fileHandler.LoadCompaniesFromCsv();
            _fileHandler.LoadEmployeesFromCsv();
        }

        private static void SaveToFile()
        {
            var _fileHandler = new FileHandler(_appSettings, _dataSlot);
            _fileHandler.SaveEmployeesToCsv();
            _fileHandler.SaveCompaniesToCsv();
        }

        private static void CompanyLists()
        {
            Console.WriteLine("----------------Get Company List----------------");

            foreach (var company in _dataSlot.Companies)
            {
                Console.WriteLine("{0} {1}", company.Id, company.Name);
            }
        }

        private static void EmployeeLists()
        {
            Console.WriteLine("----------------Get Employee List----------------");

            foreach (var employee in _dataSlot.Employees)
            {
                Console.WriteLine("Employee : Id:{0} Name:{1} BirthDate:{2} Company Name:{3} Company Id:{4} Salary:{5} TL Age:{6}",
                    employee.Id, employee.Name, employee.BirthDate.ToString("dd.MM.yyyy"), employee.Company?.Name, employee.Company?.Id, employee.Salary, employee.Age);
            }

            Console.WriteLine("----------------Employee list order by Salary----------------");
            var resultSalary = _dataSlot.Employees.OrderByDescending(e => e.Salary);

            foreach (Employee employee in resultSalary)
            {
                Console.WriteLine("{0}: {1} TL", employee.Name, employee.Salary);
            }
        }
    }
    public class DataSlot
    {
        public List<Company> Companies { get; set; }
        public List<Employee> Employees { get; set; }

        public DataSlot()
        {
            Companies = new List<Company>();
            Employees = new List<Employee>();
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

        public static void CreateCompany(DataSlot dataSlot, int total)
        {
            for (int i = 0; i < total; i++)
            {
                var randomId = RandomUtils.RandomCompanyId();
                var company = new Company(randomId, "Company" + (i + 1).ToString());
                dataSlot.Companies.Add(company);
            }
        }
    }

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
                var randomDate = RandomUtils.RandomDate(startDate, endDate);
                var employee = new Employee(++biggerId, "Emp-" + (i + 1).ToString(), randomDate, RandomUtils.RandomSalary(4200,10000));
                employee.Company = dataSlot.Companies[RandomUtils.RandomIndex(companiesCount)];
                dataSlot.Employees.Add(employee);
            }
        }
    }

    public class FileHandler
    {
        private readonly AppSettings _appSettings;
        private readonly DataSlot _dataSlot;

        public FileHandler(AppSettings appSettings, DataSlot dataSlot)
        {
            _appSettings = appSettings;
            _dataSlot = dataSlot;
        }

        public void LoadEmployeesFromCsv()
        {
            if (File.Exists(_appSettings.FilePaths.Employees))
            {
                var empStrList = ReadCsv(_appSettings.FilePaths.Employees);
                foreach (var item in empStrList)
                {
                    var fields = item.Split(';');
                    _dataSlot.Employees.Add(new Employee()
                    {
                        Id = int.Parse(fields[0]),
                        Name = fields[1],
                        BirthDate = DateTime.ParseExact(fields[2], "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture),
                        Company = _dataSlot.Companies.Where(e => e.Id == int.Parse(fields[3])).FirstOrDefault(),
                        Salary = decimal.Parse(fields[4]),
                    });
                }
            }
            else
            {
                SaveEmployeesToCsv();
            }
        }

        public void SaveEmployeesToCsv()
        {
            var lines = new List<string>();
            lines.Add("Id;Name;BirthDate;CompanyId;Salary;Age");

            foreach (var item in _dataSlot.Employees)
            {
                lines.Add(item.Id + ";" + item.Name + ";" + item.BirthDate.ToString("dd.MM.yyyy") + ";" + item.Company?.Id + ";" + item.Salary + ";" + item.Age);
            }

            string filePath = _appSettings.FilePaths.Employees + "" + DateTime.Now.ToString("dd-MM-yyyy") + ".csv";
            WriteCsv(filePath, lines);
        }

        public void LoadCompaniesFromCsv()
        {
            if (File.Exists(_appSettings.FilePaths.Companies))
            {
                var cmpStrList = ReadCsv(_appSettings.FilePaths.Companies);
                foreach (var item in cmpStrList)
                {
                    var fields = item.Split(';');
                    _dataSlot.Companies.Add(new Company()
                    {
                        Id = int.Parse(fields[0]),
                        Name = fields[1],
                    });
                }
            }
            else
            {
                SaveCompaniesToCsv();
            }
        }

        public void SaveCompaniesToCsv()
        {
            var lines = new List<string>();
            lines.Add("Id;Name");

            foreach (var item in _dataSlot.Companies)
            {
                lines.Add(item.Id + ";" + item.Name);
            }

            string filePath = _appSettings.FilePaths.Companies + "" + DateTime.Now.ToString("dd-MM-yyyy") + ".csv";
            WriteCsv(filePath, lines);
        }

        public List<string> ReadCsv(string filePath, bool hasHeader = true)
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

        public void WriteCsv(string filePath, List<string> lines)
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
