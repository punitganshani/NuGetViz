using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NuGetViz.Core
{
    public class UserActionException : Exception
    {
        public string ErrorCode { get; set; }
        public UserActionException(string message, string errorCode, Exception actualException) : base(message, actualException)
        {
            ErrorCode = errorCode;
        }
    }
}