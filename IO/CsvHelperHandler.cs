using CsvHelper;
using CsvHelper.Configuration;
using Homework.Entities;
using Homework.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework.IO
{
    public class CsvHelperHandler : ICsvHandler
    {
        public List<Company> ReadCompany(string filePath)
        {
            if (!File.Exists(filePath))
                throw new Exception("File you entered doesn't exist");

            using (var reader = new StreamReader(filePath))
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                };

                using (var csv = new CsvReader(reader, config))
                {
                    csv.Context.RegisterClassMap<CompanyMap>();
                    var records = csv.GetRecords<Company>();
                    return records.ToList();
                }
            }
        }

        public List<Employee> ReadEmployee(string filePath)
        {
            if (!File.Exists(filePath))
                throw new Exception("File you entered doesn't exist");
            using (var reader = new StreamReader(filePath))
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                };

                using (var csv = new CsvReader(reader, config))
                {
                    csv.Context.RegisterClassMap<EmployeeMap>();
                    var records = csv.GetRecords<Employee>();
                    return records.ToList();
                }
            }
        }

        public void WriteCompany(string filePath, List<Company> content)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
            };
            using (var writer = new StreamWriter(filePath))

            using (var csv = new CsvWriter(writer, config))
            {
                csv.Context.RegisterClassMap<CompanyMap>();
                csv.WriteHeader<Company>();
                csv.NextRecord();

                foreach (var item in content)
                {
                    csv.WriteRecord(item);
                    csv.NextRecord();
                }
            }
        }

        public void WriteEmployee(string filePath, List<Employee> content)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
            };

            using (var writer = new StreamWriter(filePath))

            using (var csv = new CsvWriter(writer, config))
            {
                csv.Context.RegisterClassMap<EmployeeMap>();
                csv.WriteHeader<Employee>();
                csv.NextRecord();

                foreach (var item in content)
                {
                    csv.WriteRecord(item);
                    csv.NextRecord();
                }
            }
        }
    }
}
