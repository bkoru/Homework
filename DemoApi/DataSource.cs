using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework.DemoApi
{
    public class DataSource
    {
        public async Task<List<CompanyDto>> GetAllCompanies()
        {
            string url = "https://demo.restback.io/api/GetAllCompanies";
            var client = new RestClient(url);
            var request = new RestRequest();
            var response = await client.GetAsync(request);
            Console.WriteLine(response.Content.ToString());
            Console.Read();
            throw new NotImplementedException();
        }

        public async Task<List<EmployeeDto>> GetAllEmployees()
        {
            throw new NotImplementedException();
        }
    }
}
