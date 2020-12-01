using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atlas_WebAPI_V02
{
    class MyException : Exception
    {
        public MyException(string message) : base(message)
        {
        }
    }
}