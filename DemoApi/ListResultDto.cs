using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework.DemoApi
{
    public class ListResultDto<T>
    {
        public List<T> items { get; set; } 
        public int totalCount { get; set; }
    }
}
