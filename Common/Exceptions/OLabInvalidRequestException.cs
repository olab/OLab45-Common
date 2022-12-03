using System;
using System.Diagnostics.CodeAnalysis;

namespace OLabWebAPI.Common.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class OLabInvalidRequestException : Exception
    {
        public OLabInvalidRequestException(Exception exception)
            : base("Invalid request", exception)
        { }

        public OLabInvalidRequestException(string message)
            : base(message)
        { }
    }
}