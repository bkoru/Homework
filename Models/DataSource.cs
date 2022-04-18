using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Homework.Models
{
    public class DataSource
    {
        public async Task<List<CompanyDto>> GetAllCompanies()
        {
            string url = "https://demo.restback.io/api/GetAllCompanies";
            var client = new RestClient(url);
            var request = new RestRequest();
            var responseDto = await client.GetAsync<ResponseDto<ListResultDto<CompanyDto>>>(request);

            return responseDto.result.items;
        }

        public async Task<List<EmployeeDto>> GetAllEmployees()
        {
            string url = "https://demo.restback.io/api/GetAllEmployees";
            var client = new RestClient(url);
            var request = new RestRequest();
            var responseDto = await client.GetAsync<ResponseDto<ListResultDto<EmployeeDto>>>(request);

            return responseDto.result.items;
        }
    }
}
