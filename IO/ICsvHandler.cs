using Homework.Entities;
using System.Collections.Generic;

namespace Homework.IO
{
    public interface ICsvHandler
    {
        List<Company> ReadCompany(string filePath);
        void WriteCompany(string filePath, List<Company> content);
        List<Employee> ReadEmployee(string filePath);
        void WriteEmployee(string filePath, List<Employee> content);
    }
}
