using System;

namespace OLabWebAPI.Common.Exceptions
{
    public class OLabGeneralException : Exception
    {
        public OLabGeneralException(string message) : base(message)
        {
        }
    }
}