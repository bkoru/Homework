using CsvHelper.Configuration;
using Homework.Entities;
using System.Globalization;

namespace Homework.CsvHelper
{
    class FilterSalaryMap : ClassMap<Employee>
    {
        public FilterSalaryMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.BirthDate).Ignore();
            Map(m => m.TrId).Ignore();
            Map(m => m.Age).Ignore();
            Map(m => m.Company.Id).Ignore();
            Map(m => m.Company.TaxNo).Ignore();
            Map(m => m.Company.Name).Ignore();
        }
    }
}
