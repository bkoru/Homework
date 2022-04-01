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
            var filePath2 = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Companies-" + DateTime.Now.ToString("yyyyMMddHHmm") + ".csv");
            var content2 = "Id;Name;";
            foreach (var item in _dataSlot.Companies)
            {
                content2 += Environment.NewLine + string.Join(';', item.Id.ToString(), item.Name);
            }

            await FileHandler.WriteAsync(filePath2, content2);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Employees-" + DateTime.Now.ToString("yyyyMMddHHmm") + ".csv");
            var content = "Id;Name;BirthDate;CompanyId;Salary";
            foreach (var item in _dataSlot.Employees)
            {
                content += Environment.NewLine + string.Join(';', item.Id.ToString(), item.Name, item.BirthDate.ToString("dd.MM.yyyy"), item.Company?.Id.ToString(), item.Salary.ToString());
            }

            await FileHandler.WriteAsync(filePath, content);
        }

        private static async void LoadFromFile()
        {
            var companyFilePaths = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "Data"), "Companies-*").OrderBy(e=>e).ToList();
            foreach (var path in companyFilePaths)
            {
                var cmpStr = await FileHandler.ReadAsync(path);
                var lines = cmpStr.Split(Environment.NewLine).ToList().Where(e => !string.IsNullOrWhiteSpace(e)).ToList();
                if (lines.Count > 0)
                    lines.RemoveAt(0);

                foreach (var line in lines)
                {
                    var fields = line.Split(';');
                    _dataSlot.Companies.Add(new Company()
                    {
                        Id = int.Parse(fields[0]),
                        Name = fields[1],
                    });
                }
            }

            var employeeFilePaths = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "Data"), "Employees-*").OrderBy(e => e).ToList();
            foreach (var path in employeeFilePaths)
            {
                var cmpStr = await FileHandler.ReadAsync(path);
                var lines = cmpStr.Split(Environment.NewLine).ToList().Where(e => !string.IsNullOrWhiteSpace(e)).ToList();
                if (lines.Count > 0)
                    lines.RemoveAt(0);

                foreach (var line in lines)
                {
                    var fields = line.Split(';');
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
