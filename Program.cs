using CsvHelper;
using CsvHelper.Configuration;
using Homework.Utils;
using Homework.Models;
using Homework.Entities;
using Homework.Settings;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        private static DataSource _dataSource = new DataSource();
        
        static async Task Main(string[] args)
        {
            await Menu();
        }

        private static async Task Menu()
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
                Console.WriteLine("[A] Save Changes");
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
                    LoadCompanyCsvHelper();
                else if (cki.Key == ConsoleKey.D5 || cki.Key == ConsoleKey.NumPad5)
                    LoadEmployeeCsvHelper();
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
                else if (cki.Key == ConsoleKey.A)
                    SaveChanges();
            }
            while (cki.Key != ConsoleKey.Escape);
        }

        public static void ReadCsv()
        {
            string companyPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Companies-Sample.csv");

            using (var reader = new StreamReader(companyPath))
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                };

                    using (var csv = new CsvReader(reader, config))
                    {
                        var records = csv.GetRecords<Company>();
                        csv.Context.RegisterClassMap<CompanyMap>();

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
                        }
                    }
            }

            string employeePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Employees-Sample.csv");

            using (var reader = new StreamReader(employeePath))
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                };

                using (var csv = new CsvReader(reader, config))
                {
                    var records = csv.GetRecords<Employee>();
                    csv.Context.RegisterClassMap<EmployeeMap>();

                    foreach (var employee in records)
                    {
                        var nextId = 1;

                        if (_dataSlot.Employees.Count > 0)
                        {
                            nextId = _dataSlot.Employees.Max(e => e.Id);
                            nextId++;
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
                                Id = nextId++
                            });
                        }
                    }
                }
            }
        }

        public static void LoadCompanyCsvHelper()
        {
            string filePath = null;
            Console.Clear();
            Console.WriteLine("Please enter valid path");
            filePath = Console.ReadLine();

            if (!File.Exists(filePath))
            {
                Console.WriteLine("File you entered doesn't exist");
                Console.WriteLine("\r\n Press Enter to return to Main Menu");
                Console.ReadKey();
                return;
            }

            if (filePath != null)
            {
                using (var reader = new StreamReader(filePath))
                {
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        Delimiter = ";",
                    };

                    try
                    {
                        using (var csv = new CsvReader(reader, config))
                        {
                            var records = csv.GetRecords<Company>();
                            csv.Context.RegisterClassMap<CompanyMap>();

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
                    }
                    catch (Exception ex)
                    {
                        if (ex is FieldValidationException)
                        {
                            Console.WriteLine("Invalid file format");
                        }
                        else
                        {
                            Console.WriteLine(ex.Message);
                        }
                        Console.ReadKey();
                    }
                }
            }
        }

        public static void LoadEmployeeCsvHelper()
        {
            string filePath = null;
            Console.Clear();
            Console.WriteLine("Please enter valid path");
            filePath = Console.ReadLine();

            if (!File.Exists(filePath))
            {
                Console.WriteLine("File you entered doesn't exist");
                Console.WriteLine("\r\n Press Enter to return to Main Menu");
                Console.ReadKey();
                return;
            }

            if (filePath != null)
            {
                using (var reader = new StreamReader(filePath))
                {
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        Delimiter = ";",
                    };

                    try
                    {
                        using (var csv = new CsvReader(reader, config))
                        {
                            csv.Context.RegisterClassMap<EmployeeMap>();
                            var records = csv.GetRecords<Employee>();

                            foreach (var employee in records)
                            {
                                var nextId = 1;

                                if (_dataSlot.Employees.Count > 0)
                                {
                                    nextId = _dataSlot.Employees.Max(e => e.Id);
                                    nextId++;
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
                    }
                    catch (Exception ex)
                    {
                        if (ex is FieldValidationException)
                        {
                            Console.WriteLine("Invalid file format");
                        }
                        else
                        {
                            Console.WriteLine(ex.Message);
                        }
                        Console.ReadKey();
                    }
                }
            }
        }

        public static void FilterSalaryCsvHelper()
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
                using (var writer = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), "Data", "Employees-" + DateTime.Now.ToString("yyyyMMddHHmm") + ".csv")))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.Context.RegisterClassMap<EmployeeMap>();
                    csv.WriteHeader<Employee>();
                    csv.NextRecord();

                    foreach (var item in resultSalary)
                    {
                        csv.WriteRecord(item);
                        csv.NextRecord();
                    }
                }
            }
            else if (cki.Key == ConsoleKey.N)
            {
                Console.Write("\r\nPress Enter to return to Main Menu");
                Console.ReadLine();
            }
        }

        private async static Task LoadCompanyFromApi()
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

        private async static Task LoadEmployeeFromApi()
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

        private static void CreateRandomCompany()
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

        private static void CreateRandomEmployee()
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


        private static void CompanyLists()
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

        private static void EmployeeLists()
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

        static void CreateEmployee()
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

        public static void SaveChanges()
        {
            if (_dataSlot.Employees.Any() == true)
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                };

                using (var writer = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), "Data", "Employees-Sample.csv")))

                using (var csv = new CsvWriter(writer, config))
                {
                    csv.Context.RegisterClassMap<EmployeeMap>();
                    csv.WriteHeader<Employee>();
                    csv.NextRecord();

                    foreach (var item in _dataSlot.Employees)
                    {
                        csv.WriteRecord(item);
                        csv.NextRecord();
                    }
                }
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Failed to save Employee List. Because the list is empty.");
                Console.Write("Press any key to continue...");
                Console.ReadKey();
            }

            if (_dataSlot.Companies.Any() == true)
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        Delimiter = ";",
                    };

                using (var writer = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), "Data", "Companies-Sample.csv")))

                using (var csv = new CsvWriter(writer, config))
                {
                    csv.Context.RegisterClassMap<CompanyMap>();
                    csv.WriteHeader<Company>();
                    csv.NextRecord();

                    foreach (var item in _dataSlot.Companies)
                    {
                        csv.WriteRecord(item);
                        csv.NextRecord();
                    }
                }
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Failed to save Company List. Because the list is empty.");
                Console.Write("Press any key to continue...");
                Console.ReadKey();
            }
        }

        static async Task FilterSalary()
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

        private static async Task SaveToFile()
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

        private static async Task LoadCompany()
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

            var nextId = 1;
            if (_dataSlot.Companies.Count > 0)
            {
                nextId = _dataSlot.Companies.Max(e => e.Id);
            }

            foreach (var line in companyLines)
            {
                var fields = line.Split(';');
                var found = _dataSlot.Companies.Find(e => e.TaxNo == fields[1]);
                if (found == null)
                {
                    _dataSlot.Companies.Add(new Company()
                    {
                        Id = nextId++,
                        Name = fields[0],
                        TaxNo = fields[1]
                    });
                }
            }
            CompanyLists();
        }

        private static void ConfigureSettings()
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false);
            var config = builder.Build();
            _appSettings = config.Get<AppSettings>();
        }

        public static async Task LoadEmployee()
        {
            string empStr = null;
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
