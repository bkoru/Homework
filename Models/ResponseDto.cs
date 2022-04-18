using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework.Models
{
    public class ResponseDto<T>
    {
        public bool success { get; set; }
        public T result { get; set; }
    }
}
