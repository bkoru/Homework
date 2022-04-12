using System.Collections.Generic;

namespace Homework.DemoApi
{
    public class ListResultDto<T>
    {
        public List<T> items { get; set; } 
        public int totalCount { get; set; }
    }
}
