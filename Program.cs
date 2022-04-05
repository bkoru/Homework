using Homework.Entities;
using Homework.Settings;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework
{
    class Program
    {
        private static DataSlot _dataSlot = new DataSlot();

        private static AppSettings _appSettings;
        
        static void Main(string[] args)
        {
            LoadFromFile();
            CompanyLists();
            EmployeeLists();
            CreateCompany();
            CreateEmployee();
            CompanyLists();
            EmployeeLists();
            SaveToFile();

            Console.ReadLine();
        }

        private static async void SaveToFile()
        {
            var CompanyPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Companies-" + DateTime.Now.ToString("yyyyMMddHHmm") + ".csv");
            var companyContent = "Name;";
            foreach (var item in _dataSlot.Companies)
            {
                companyContent += Environment.NewLine + string.Join(';', item.Name);
            }

            await FileHandler.WriteAsync(CompanyPath, companyContent);

            var employeePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Employees-" + DateTime.Now.ToString("yyyyMMddHHmm") + ".csv");
            var employeeContent = "Name;BirthDate;CompanyName;Salary;TrId";
            foreach (var item in _dataSlot.Employees)
            {
                employeeContent += Environment.NewLine + string.Join(';', item.Name, item.BirthDate.ToString("dd.MM.yyyy"), item.Company?.Name, item.Salary.ToString(), item.TrId);
            }

            await FileHandler.WriteAsync(employeePath, employeeContent);
        }

        private static async void LoadFromFile()
        {
                var cmpStr = await FileHandler.ReadAsync(Path.Combine(Directory.GetCurrentDirectory(), "Data", "Companies-Sample.csv"));
                var companyLines = cmpStr.Split(Environment.NewLine).ToList().Where(e => !string.IsNullOrWhiteSpace(e)).ToList();
                if (companyLines.Count > 0)
                companyLines.RemoveAt(0);

                foreach (var line in companyLines)
            {
                    var fields = line.Split(';');
                    var found = _dataSlot.Companies.Find(e =>e.Name == fields[1]);
                    if (found == null)
                    {
                        var biggerId = 0;
                        if (_dataSlot.Companies.Count > 0)
                        {
                            biggerId = _dataSlot.Companies.Max(e => e.Id);
                        }
                        _dataSlot.Companies.Add(new Company()
                        {
                            Id = ++biggerId,
                            Name = fields[1],
                        });
                    }
                }

                var empStr = await FileHandler.ReadAsync(Path.Combine(Directory.GetCurrentDirectory(), "Data", "Employees-Sample.csv"));
                var employeeLines = empStr.Split(Environment.NewLine).ToList().Where(e => !string.IsNullOrWhiteSpace(e)).ToList();
                if (employeeLines.Count > 0)
                employeeLines.RemoveAt(0);

                foreach (var line in employeeLines)
                {
                    var fields = line.Split(';');
                var found = _dataSlot.Employees.Find(e => e.TrId == fields[4]);
                if (found == null)
                {
                    _dataSlot.Employees.Add(new Employee()
                    {
                        Name = fields[0],
                        BirthDate = DateTime.ParseExact(fields[1], "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture),
                        Company = _dataSlot.Companies.Where(e => e.Name == fields[2]).FirstOrDefault(),
                        Salary = decimal.Parse(fields[3]),
                        TrId = fields[4]
                    });
                }
            }
                
        }

        private static void ConfigureSettings()
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false);
            var config = builder.Build();
            _appSettings = config.Get<AppSettings>();
        }

        private static void CreateCompany()
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

        private static void CreateEmployee()
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


        private static void CompanyLists()
        {
            Console.Clear();
            Console.WriteLine("----------------Get Company List----------------");

            foreach (var company in _dataSlot.Companies)
            {
                Console.WriteLine("{0}", company.Name);
            }
        }

        private static void EmployeeLists()
        {
            Console.WriteLine("----------------Get Employee List----------------");

            foreach (var employee in _dataSlot.Employees)
            {
                Console.WriteLine("Name:{0} BirthDate:{1} Company Name:{2} Company Id:{3} Salary:{4} TL Age:{5} TR Id:{6}",
                    employee.Name, employee.BirthDate.ToString("dd.MM.yyyy"), employee.Company?.Name, employee.Company?.Id, employee.Salary, employee.Age, employee.TrId);
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

    public class FileHandler
    {
        public static async Task WriteAsync(string filePath, string content)
        {
            await File.WriteAllTextAsync(filePath, content, Encoding.UTF8);
        }

        public static async Task<string> ReadAsync(string filepath)
        {
            return await File.ReadAllTextAsync(filepath, Encoding.UTF8);
        }
    }
}
