using Homework.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework.IO
{
    class CsvStandartHandler : ICsvHandler
    {
        public List<Company> ReadCompany(string filePath)
        {
            List<Company> result = new List<Company>();
            if (!File.Exists(filePath))
                throw new Exception("File you entered doesn't exist");
            var records = File.ReadAllText(filePath, Encoding.UTF8);
            var companyLines = records.Split(Environment.NewLine).Where(e => !string.IsNullOrWhiteSpace(e)).ToList();
            if (companyLines.Count > 0)
                companyLines.RemoveAt(0);
            foreach (var line in companyLines)
            {
                var fields = line.Split(';');
                result.Add(new Company() { 
                    Name = fields[0],
                    TaxNo = fields[1]
                });
            }
            return result;
        }

        public List<Employee> ReadEmployee(string filePath)
        {
            List<Employee> result = new List<Employee>();
            if (!File.Exists(filePath))
                throw new Exception("File you entered doesn't exist");
            var records = File.ReadAllText(filePath, Encoding.UTF8);
            var employeeLines = records.Split(Environment.NewLine).Where(e => !string.IsNullOrWhiteSpace(e)).ToList();
            if (employeeLines.Count > 0)
                employeeLines.RemoveAt(0);
            foreach (var line in employeeLines)
            {
                var fields = line.Split(';');
                result.Add(new Employee()
                {
                    Name = fields[0],
                    BirthDate = DateTime.ParseExact(fields[1], "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture),
                    Salary = decimal.Parse(fields[3]),
                    TrId = fields[4]
                });
            }
            return result;
        }

        public void WriteCompany(string filePath, List<Company> content)
        {
            var companyPath = filePath;
            var companyContent = "Name;TaxNo";
            foreach (var item in content)
            {
                companyContent += Environment.NewLine + string.Join(';', item.Name, item.TaxNo);
            }
            File.WriteAllText(companyPath, companyContent);
        }

        public void WriteEmployee(string filePath, List<Employee> content)
        {
            var employeePath = filePath;
            var employeeContent = "Name;BirthDate;CompanyName;Salary;TrId";
            foreach (var item in content)
            {
                employeeContent += Environment.NewLine + string.Join(';', item.Name, item.BirthDate.ToString("dd.MM.yyyy"), item.Company?.Name, item.Salary.ToString(), item.TrId);
            }
            File.WriteAllText(employeePath, employeeContent);
        }
    }
}
