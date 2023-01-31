using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services
{
    public class ResultDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public bool Flag1 { get; set; }
    }
    public class ResultDto<T> : ResultDto
    {
        public T Result { get; set; }
    }
}