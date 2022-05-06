using Homework.Entities;
using Homework.IO;
using Homework.Models;
using Homework.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Homework
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ServiceCollection services = new ServiceCollection();
            services.AddScoped<App>();
            services.AddScoped<DataSlot>();
            services.AddScoped<DataSource>();
            services.AddScoped<ICsvHandler, CsvHelperHandler>();
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            var app = serviceProvider.GetService<App>();

            await app.Run();
        }
    }
    public class App
    {
        ICsvHandler _csvHandler;
        DataSlot _dataSlot;
        DataSource _dataSource;
        public App(ICsvHandler csvHandler, DataSlot dataSlot, DataSource dataSource)
        {
            _csvHandler = csvHandler;
            _dataSlot = dataSlot;
            _dataSource = dataSource;
        }

        private static AppSettings _appSettings;

        public async Task Run()
        {
            await Menu();
        }
        private async Task Menu()
        {
            ReadCsv();
            ConsoleKeyInfo cki;
            do
            {
                Console.Clear();
                Console.WriteLine("### Main Menu ###");
                Console.WriteLine("[1] Create random company");
                Console.WriteLine("[2] Create random employee");
                Console.WriteLine("[3] Create a new employee");
                Console.WriteLine("[4] Load Company from Csv");
                Console.WriteLine("[5] Load Employee from Csv");
                Console.WriteLine("[6] List of companies");
                Console.WriteLine("[7] List of employees");
                Console.WriteLine("[8] Employees salary filter");
                Console.WriteLine("[9] Load Company from Web API");
                Console.WriteLine("[0] Load Employee from Web API");
                Console.WriteLine("[C] Clear All Data");
                Console.WriteLine("[ESC] Exit");
                cki = Console.ReadKey();

                if (cki.Key == ConsoleKey.D1 || cki.Key == ConsoleKey.NumPad1)
                    CreateRandomCompany();
                else if (cki.Key == ConsoleKey.NumPad1 || cki.Key == ConsoleKey.NumPad1)
                    CreateRandomCompany();
                else if (cki.Key == ConsoleKey.D2 || cki.Key == ConsoleKey.NumPad2)
                    CreateRandomEmployee();
                else if (cki.Key == ConsoleKey.D3 || cki.Key == ConsoleKey.NumPad3)
                    CreateEmployee();
                else if (cki.Key == ConsoleKey.D4 || cki.Key == ConsoleKey.NumPad4)
                    LoadCompany();
                else if (cki.Key == ConsoleKey.D5 || cki.Key == ConsoleKey.NumPad5)
                    LoadEmployee();
                else if (cki.Key == ConsoleKey.D6 || cki.Key == ConsoleKey.NumPad6)
                    CompanyLists();
                else if (cki.Key == ConsoleKey.D7 || cki.Key == ConsoleKey.NumPad7)
                    EmployeeLists();
                else if (cki.Key == ConsoleKey.D8 || cki.Key == ConsoleKey.NumPad8)
                    FilterSalaryCsvHelper();
                else if (cki.Key == ConsoleKey.D9 || cki.Key == ConsoleKey.NumPad9)
                    await LoadCompanyFromApi();
                else if (cki.Key == ConsoleKey.D0 || cki.Key == ConsoleKey.NumPad0)
                    await LoadEmployeeFromApi();
                else if (cki.Key == ConsoleKey.C)
                    ClearData();
            }
            while (cki.Key != ConsoleKey.Escape);
        }

        public void ReadCsv()
        {
            string companyPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Companies-Sample.csv");
            var companyRecords = _csvHandler.ReadCompany(companyPath);
            var nextCmpId = 1;
            if (_dataSlot.Companies.Count > 0)
            {
                nextCmpId = _dataSlot.Companies.Max(c => c.Id);
                nextCmpId++;
            }

            foreach (var company in companyRecords)
            {
                var found = _dataSlot.Companies.Find(c => c.TaxNo == company.TaxNo || c.Name == company.Name);
                if (found == null)
                {
                    _dataSlot.Companies.Add(new Company()
                    {
                        Id = nextCmpId++,
                        Name = company.Name,
                        TaxNo = company.TaxNo
                    });
                }
            }

            string employeePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Employees-Sample.csv");
            var employeeRecords = _csvHandler.ReadEmployee(employeePath);


            foreach (var employee in employeeRecords)
            {
                var nextEmpId = 1;

                if (_dataSlot.Employees.Count > 0)
                {
                    nextEmpId = _dataSlot.Employees.Max(e => e.Id);
                    nextEmpId++;
                }

                var found = _dataSlot.Employees.Find(e => e.TrId == employee.TrId);
                if (found == null)
                {
                    _dataSlot.Employees.Add(new Employee()
                    {
                        Name = employee.Name,
                        BirthDate = employee.BirthDate,
                        Salary = employee.Salary,
                        TrId = employee.TrId,
                        Id = nextEmpId++
                    });
                }
            }

        }

        public void LoadCompany()
        {
            string filePath = null;
            Console.Clear();
            Console.WriteLine("Please enter valid path");
            filePath = Console.ReadLine();
            try
            {
                var records = _csvHandler.ReadCompany(filePath);
                var nextId = 1;

                if (_dataSlot.Companies.Count > 0)
                {
                    nextId = _dataSlot.Companies.Max(c => c.Id);
                    nextId++;
                }

                foreach (var company in records)
                {
                    var found = _dataSlot.Companies.Find(c => c.TaxNo == company.TaxNo || c.Name == company.Name);

                    if (found == null)
                    {
                        _dataSlot.Companies.Add(new Company()
                        {
                            Id = nextId++,
                            Name = company.Name,
                            TaxNo = company.TaxNo
                        });
                    }
                    else
                    {
                        Console.WriteLine("Company could not be added. Because the tax number " + company.TaxNo + " or the company name " + company.Name + " already exists in the system.");
                    }
                }
                Console.ReadKey();
                CompanyLists();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("\r\n Press Enter to return to Main Menu");
                Console.ReadKey();
                return;
            } 
        }

        public void LoadEmployee()
        {
            string filePath = null;
            Console.Clear();
            Console.WriteLine("Please enter valid path");
            filePath = Console.ReadLine();

            try
            {
                var records = _csvHandler.ReadEmployee(filePath);
                var nextId = 1;

                if (_dataSlot.Employees.Count > 0)
                {
                    nextId = _dataSlot.Employees.Max(e => e.Id);
                    nextId++;
                }

                foreach (var employee in records)
                {
                    var found = _dataSlot.Employees.Find(e => e.TrId == employee.TrId);

                    if (found == null)
                    {
                        _dataSlot.Employees.Add(new Employee()
                        {
                            Name = employee.Name,
                            BirthDate = employee.BirthDate,
                            Salary = employee.Salary,
                            TrId = employee.TrId,
                            Id = nextId++
                        });
                    }
                    else
                    {
                        Console.WriteLine("Employee could not be added. Because TR Id: " + employee.TrId + " already exists in the system.");
                    }
                }
                Console.ReadKey();
                EmployeeLists();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("\r\n Press Enter to return to Main Menu");
                Console.ReadKey();
                return;
            }
        }

        public void FilterSalaryCsvHelper()
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
                _csvHandler.WriteEmployee(Path.Combine(Directory.GetCurrentDirectory(), "Data", "Employees-" + DateTime.Now.ToString("yyyyMMddHHmm") + ".csv"), resultSalary.ToList());
            }
            else if (cki.Key == ConsoleKey.N)
            {
                Console.Write("\r\nPress Enter to return to Main Menu");
                Console.ReadLine();
            }
        }

        private async Task LoadCompanyFromApi()
        {
            Console.Clear();
            Console.WriteLine("Please Wait");
            var companiesDto = await _dataSource.GetAllCompanies();

            foreach (var company in companiesDto)
            {
                var nextId = 1;
                if (_dataSlot.Companies.Count > 0)
                {
                    nextId = _dataSlot.Companies.Max(c => c.Id);
                    nextId++;
                }

                var found = _dataSlot.Companies.Find(c => c.TaxNo == company.taxNo || c.Name == company.name);
                if (found == null)
                {
                    _dataSlot.Companies.Add(new Company()
                    {
                        Name = company.name,
                        TaxNo = company.taxNo,
                        Id = nextId++
                    });
                }
                else
                {
                    Console.WriteLine("Company could not be added. Because the tax number " + company.taxNo + " or the company name " + company.name + " already exists in the system.");
                    Console.Write("Press any key");
                    Console.ReadKey();
                }
            }
            CompanyLists();
        }

        private async Task LoadEmployeeFromApi()
        {
            Console.Clear();
            Console.WriteLine("Please Wait");
            var employeesDto = await _dataSource.GetAllEmployees();

            foreach (var employee in employeesDto)
            {
                var nextId = 1;
                if (_dataSlot.Employees.Count > 0)
                {
                    nextId = _dataSlot.Employees.Max(e => e.Id);
                    nextId++;
                }

                var found = _dataSlot.Employees.Find(e => e.TrId == employee.tckn);
                if (found == null)
                {
                    _dataSlot.Employees.Add(new Employee()
                    {
                        Name = employee.name,
                        BirthDate = employee.birthDate,
                        Salary = employee.salary,
                        TrId = employee.tckn,
                        Id = nextId++
                    });
                }
                else
                {
                    Console.WriteLine("The employee named " + employee.name + " could not be added. Because the ID number " + employee.tckn + " already exists in the system.");
                    Console.Write("Press any key");
                    Console.ReadKey();
                }
            }
            EmployeeLists();
        }

        private void CreateRandomCompany()
        {
            Console.Clear();
            int companyCount;
            Console.Write("Please enter the number of companies you want to add: ");

            while (!int.TryParse(Console.ReadLine(), out companyCount))
            {
                Console.WriteLine("Please enter a valid number");
                Console.Write("Please enter the number of companies you want to add: ");
            }

            Company.CreateCompany(_dataSlot, companyCount);
            SaveChanges();
        }

        private void CreateRandomEmployee()
        {
            Console.Clear();
            int employeeCount;
            Console.Write("Please enter the number of employees you want to add: ");

            while (!int.TryParse(Console.ReadLine(), out employeeCount))
            {
                Console.WriteLine("Please enter a valid number");
                Console.Write("Please enter the number of employees you want to add: ");
            }

            Employee.CreateEmployee(_dataSlot, employeeCount);
            SaveChanges();
        }

        public void CompanyLists()
        {
            Console.Clear();
            Console.WriteLine("----------------Get Company List----------------");

            foreach (var company in _dataSlot.Companies)
            {
                Console.WriteLine("Name:{0} Tax No:{1} Id:{2}", company.Name, company.TaxNo, company.Id);
            }

            SaveChanges();
            Console.Write("Press any key to back");
            Console.ReadKey();
        }

        private void EmployeeLists()
        {
            Console.Clear();
            Console.WriteLine("----------------Get Employee List----------------");

            foreach (var employee in _dataSlot.Employees)
            {
                Console.WriteLine("Id:{0} Name:{1} BirthDate:{2} Salary:{3} TL Age:{4} TR Id:{5}",
                    employee.Id, employee.Name, employee.BirthDate.ToString("dd.MM.yyyy"), employee.Salary, employee.Age, employee.TrId);
            }

            Console.WriteLine("----------------Employee list order by Salary----------------");
            var resultSalary = _dataSlot.Employees.OrderByDescending(e => e.Salary);

            foreach (Employee employee in resultSalary)
            {
                Console.WriteLine("{0}: {1} TL", employee.Name, employee.Salary);
            }

            SaveChanges();
            Console.Write("Press any key to back");
            Console.ReadKey();
        }

        public void CreateEmployee()
        {
            Console.Clear();
            Console.WriteLine("### Create a new user ###");

            string nameInput;
            if (!ReadInputFromConsole("Name", e => new Regex(@"^[a-zA-Z]{2,16}$").IsMatch(e), out nameInput))
                return;

            string trIdInput;
            if (!ReadInputFromConsole("TR ID", e => new Regex(@"^[1-9]{1}[0-9]{10}$").IsMatch(e), out trIdInput))
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

            Employee user = new Employee
            {
                Salary = salaryInput,
                BirthDate = birthDate,
                Name = nameInput,
                TrId = trIdInput
            };

            var nextId = 1;
            if (_dataSlot.Employees.Count > 0)
            {
                nextId = _dataSlot.Employees.Max(e => e.Id);
                nextId++;
            }
            user.Id = nextId++;

            var found = _dataSlot.Employees.Find(e => e.TrId == trIdInput);

            if (found == null)
            {
                _dataSlot.Employees.Add(user);
                Console.WriteLine("User created. " + user.Name + " " + user.TrId);
            }
            else
            {
                Console.WriteLine("The employee named " + user.Name + " could not be added. Because the ID number " + user.TrId + " already exists in the system.");
            }

            SaveChanges();
            Console.Write("Press any key to back");
            Console.ReadKey();
        }

        static bool ReadInputFromConsole(string label, Func<string, bool> onValidate, out string value)
        {
            value = null;
            bool isValid = false;
            while (!isValid)
            {
                Console.WriteLine();
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

        public void SaveChanges()
        {
            _csvHandler.WriteEmployee((Path.Combine(Directory.GetCurrentDirectory(), "Data", "Employees-Sample.csv")), _dataSlot.Employees);
            _csvHandler.WriteCompany((Path.Combine(Directory.GetCurrentDirectory(), "Data", "Companies-Sample.csv")), _dataSlot.Companies);
        }

        public void ClearData()
        {
            Console.Clear();
            Console.WriteLine("Are you sure you want to CLEAR all data? (y/n)");
            ConsoleKeyInfo cki = Console.ReadKey();
            if (cki.Key == ConsoleKey.Y)
            {
                _dataSlot.Employees.Clear();
                _dataSlot.Companies.Clear();
                SaveChanges();
                Console.Clear();
                Console.Write("All data cleared. Press any key to back");
                Console.ReadKey();
            }

            else if (cki.Key == ConsoleKey.N)
            {
                Console.Clear();
                Console.Write("Press any key to back");
                Console.ReadKey();
            }
        }

        private void ConfigureSettings()
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false);
            var config = builder.Build();
            _appSettings = config.Get<AppSettings>();
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
}
