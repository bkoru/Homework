using System.Collections.Generic;

namespace Homework.Models
{
    public class ListResultDto<T>
    {
        public List<T> items { get; set; } 
        public int totalCount { get; set; }
    }
}
