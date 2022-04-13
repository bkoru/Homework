using CsvHelper.Configuration;
using Homework.Entities;
using System.Globalization;

namespace Homework.CsvHelper
{
    class EmployeeMap : ClassMap<Employee>
    {
        public EmployeeMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.Company.Id).Ignore();
            Map(m => m.Company.TaxNo).Ignore();
            Map(m => m.Company.Name).Ignore();
        }
    }
}
