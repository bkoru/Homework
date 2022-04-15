using CsvHelper.Configuration;
using Homework.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework.CsvHelper
{
    class FilterMap : ClassMap<Employee>
    {
        public FilterMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.Company.Id).Ignore();
            Map(m => m.Company.TaxNo).Ignore();
            Map(m => m.Company.Name).Ignore();
        }
    }
}

