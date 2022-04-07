using Homework.Entities;
using Homework.Settings;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Homework
{
    class Program
    {
        private static DataSlot _dataSlot = new DataSlot();

        private static AppSettings _appSettings;
        
        static void Main(string[] args)
        {
            Menu();

            Console.ReadLine();
        }

        private static void Menu()
        {
            ConsoleKeyInfo cki;
            do
            {
                Console.Clear();
                Console.WriteLine("### Main Menu ###");
                Console.WriteLine("[0] Create random company");
                Console.WriteLine("[1] Create random employee");
                Console.WriteLine("[2] Create a new employee");
                Console.WriteLine("[3] Load Company from Csv");
                Console.WriteLine("[4] Load Employee from Csv");
                Console.WriteLine("[5] List of companies");
                Console.WriteLine("[6] List of employees");
                Console.WriteLine("[7] Employees salary filter");
                Console.WriteLine("[8] Load Company from Web API");
                Console.WriteLine("[9] Load Employee from Web API");
                Console.WriteLine("[ESC] Exit");
                cki = Console.ReadKey();

                if (cki.Key == ConsoleKey.D0)
                    CreateCompany();
                else if (cki.Key == ConsoleKey.D1)
                    CreateRandomEmployee();
                else if (cki.Key == ConsoleKey.D2)
                    CreateEmployee();
                else if (cki.Key == ConsoleKey.D3)
                    LoadCompany();
                else if (cki.Key == ConsoleKey.D4)
                    LoadEmployee();
                else if (cki.Key == ConsoleKey.D5)
                    CompanyLists();
                else if (cki.Key == ConsoleKey.D6)
                    EmployeeLists();
                else if (cki.Key == ConsoleKey.D7)
                    FilterSalary();
                else if (cki.Key == ConsoleKey.D8)
                    LoadCompanyFromApi();
                else if (cki.Key == ConsoleKey.D9)
                    LoadEmployeeFromApi();
            }
            while (cki.Key != ConsoleKey.Escape);
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

        private static async void LoadCompany()
        {
            string cmpStr = null;
            Console.WriteLine(" Do you want to use default file? (y/n)");
            ConsoleKeyInfo cki = Console.ReadKey();
            if (cki.Key == ConsoleKey.N)
            {
                Console.WriteLine("Please enter valid path");
                string filePath = Console.ReadLine();
                if (File.Exists(filePath))
                {
                    cmpStr = await FileHandler.ReadAsync(filePath);
                }
                else
                {
                    Console.WriteLine("The path is invalid");
                    Console.Write("Press any key to back");
                    Console.ReadKey();
                }
            }
            else if (cki.Key == ConsoleKey.Y)
            {
                cmpStr = await FileHandler.ReadAsync(Path.Combine(Directory.GetCurrentDirectory(), "Data", "Companies-Sample.csv"));
            }

            var companyLines = cmpStr.Split(Environment.NewLine).ToList().Where(e => !string.IsNullOrWhiteSpace(e)).ToList();
            if (companyLines.Count > 0)
                companyLines.RemoveAt(0);

            foreach (var line in companyLines)
            {
                var fields = line.Split(';');
                var found = _dataSlot.Companies.Find(e => e.TaxNo == fields[1]);
                if (found == null)
                {
                    _dataSlot.Companies.Add(new Company()
                    {
                        Name = fields[0],
                        TaxNo = fields[1]
                    });
                }
            }
            CompanyLists();
        }

        public static async void LoadEmployee()
        {
            var empStr = await FileHandler.ReadAsync(Path.Combine(Directory.GetCurrentDirectory(), "Data", "Employees-Sample.csv"));
            Console.WriteLine(" Do you want to use default file? (y/n)");
            ConsoleKeyInfo cki = Console.ReadKey();

            if (cki.Key == ConsoleKey.N)
            {
                Console.WriteLine("Please enter valid path");
                string filePath = Console.ReadLine();
                if (File.Exists(filePath))
                {
                    empStr = await FileHandler.ReadAsync(filePath);
                }
                else
                {
                    Console.WriteLine("The path is invalid");
                    Console.Write("Press any key to back");
                    Console.ReadKey();
                }
            }
            else if (cki.Key == ConsoleKey.Y)
            {
                empStr = await FileHandler.ReadAsync(Path.Combine(Directory.GetCurrentDirectory(), "Data", "Employees-Sample.csv"));
            }

            var employeeLines = empStr.Split(Environment.NewLine).ToList().Where(e => !string.IsNullOrWhiteSpace(e)).ToList();
            if (employeeLines.Count > 0)
            {
                employeeLines.RemoveAt(0);
            }

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
            EmployeeLists();
        }

        private async static void LoadCompanyFromApi()
        {
            var dataSource = new DemoApi.DataSource();
            var companiesDto = await dataSource.GetAllCompanies();

            foreach (var company in companiesDto)
            {
                _dataSlot.Companies.Add(new Company()
                {
                    Name = company.name,
                    TaxNo = company.taxNo
                });
            }
            CompanyLists();
        }

        private async static void LoadEmployeeFromApi()
        {
            var dataSource = new DemoApi.DataSource();
            var employeesDto = await dataSource.GetAllEmployees();
            
            foreach (var employee in employeesDto)
            {
                _dataSlot.Employees.Add(new Employee()
                {
                    Name = employee.name,
                    BirthDate = employee.birthDate,
                    Salary = employee.salary,
                    TrId = employee.tckn
                });
            }
            EmployeeLists();
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
            Console.Clear();
            int companyCount = 0;
            Console.Write("Eklemek istediğiniz şirket sayısını giriniz: ");
            while (!int.TryParse(Console.ReadLine(), out companyCount))
            {
                Console.WriteLine("Lütfen geçerli bir sayı giriniz");
                Console.Write("Eklemek istediğiniz şirket sayısını giriniz: ");
            }

            Company.CreateCompany(_dataSlot, companyCount);
        }

        private static void CreateRandomEmployee()
        {
            Console.Clear();
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
                Console.WriteLine("Name:{0} Tax No:{1}", company.Name, company.TaxNo);
            }

            if (true)
            {

            }

            Console.Write("\r\nPress Enter to return to Main Menu");
            Console.ReadLine();
        }

        private static void EmployeeLists()
        {
            Console.Clear();
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

            Console.Write("\r\nPress Enter to return to Main Menu");
            Console.ReadLine();
        }

        static void CreateEmployee()
        {
            Console.Clear();
            Console.WriteLine("### Create a new user ###");

            string nameInput;
            if (!ReadInputFromConsole("Name", e => new Regex(@"^[a-zA-Z]{2,16}$").IsMatch(e), out nameInput))
                return;

            string trIdInput;
            if (!ReadInputFromConsole("TR ID", e => new Regex(@"^[0-9]{11}$").IsMatch(e), out trIdInput))
                return;

            decimal salaryInput;
            if (!ReadInputFromConsole("Salary", e => new Regex(@"^[0-9]{4,5}$").IsMatch(e), out salaryInput))
                return;

            DateTime birthDate;
            Console.WriteLine("Date of birth");

            string[] formats = { "dd/MM/yyyy", "dd/M/yyyy", "d/M/yyyy", "d/MM/yyyy",
                "dd/MM/yy", "dd/M/yy", "d/M/yy", "d/MM/yy", "d.MM.yyyy"};

            while (!DateTime.TryParseExact(Console.ReadLine(), formats,
                 System.Globalization.CultureInfo.InvariantCulture,
                 System.Globalization.DateTimeStyles.None,
                 out birthDate))
            {
                Console.WriteLine("Your input is incorrect. Please input again.");
            }

            Employee user = new Employee();
            user.Salary = salaryInput;
            user.BirthDate = birthDate;
            user.Name = nameInput;
            user.TrId = trIdInput;

            var found = _dataSlot.Employees.Find(e => e.TrId == trIdInput);

            if (found == null)
            {
                _dataSlot.Employees.Add(user);
                Console.WriteLine("User created. " + user.Name + " " + user.TrId);
            }
            else
            {
                Console.WriteLine("The ID number you entered already exists in the system.");
            }

            Console.Write("Press any key to back");
            Console.ReadKey();
        }
        static async void FilterSalary()
        {
            Console.Clear();
            Console.WriteLine("Please enter min salary");
            decimal min = Convert.ToDecimal(Console.ReadLine());
            Console.WriteLine("Please enter max salary");
            decimal max = Convert.ToDecimal(Console.ReadLine());

            var resultSalary = _dataSlot.Employees.Where(e => e.Salary >= min && e.Salary <= max);

            foreach (Employee employee in resultSalary)
            {
                Console.WriteLine("{0}: {1} TL", employee.Name, employee.Salary.ToString());
            }

            Console.WriteLine(" Do you want to write this data to file? (y/n)");
            ConsoleKeyInfo cki = Console.ReadKey();
            if (cki.Key == ConsoleKey.Y)
            {
                var employeeContent = "Name;Salary;TrId";
                var employeePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Employees-" + DateTime.Now.ToString("yyyyMMddHHmm") + ".csv");

                foreach (var item in resultSalary)
                {
                    employeeContent += Environment.NewLine + string.Join(';', item.Name, item.Salary.ToString(), item.TrId);
                }
                await FileHandler.WriteAsync(employeePath, employeeContent);
            }
            else if (cki.Key == ConsoleKey.N)
            {
                Console.Write("\r\nPress Enter to return to Main Menu");
                Console.ReadLine();
            }
        }

        static bool ReadInputFromConsole(string label, Func<string, bool> onValidate, out string value)
        {
            value = null;
            bool isValid = false;
            while (!isValid)
            {
                Console.Write(label + ": ");
                var input = Console.ReadLine();
                isValid = onValidate(input);

                if (isValid)
                {
                    value = input;
                    return true;
                }

                Console.WriteLine("Invalid input.");
                Console.WriteLine("Press ESC to cancel or press any key to try again");
                ConsoleKeyInfo cki = Console.ReadKey();
                if (cki.Key == ConsoleKey.Escape)
                    break;
            }

            return false;
        }

        static bool ReadInputFromConsole(string label, Func<string, bool> onValidate, out decimal value)
        {
            value = 0;
            bool isValid = false;
            while (!isValid)
            {
                Console.Write(label + ": ");
                var input = Console.ReadLine();
                isValid = onValidate(input);

                if (isValid)
                {
                    value = Convert.ToDecimal(input);
                    return true;
                }

                Console.WriteLine("Invalid input.");
                Console.WriteLine("Press ESC to cancel or press any key to try again");
                ConsoleKeyInfo cki = Console.ReadKey();
                if (cki.Key == ConsoleKey.Escape)
                    break;
            }

            return false;
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
