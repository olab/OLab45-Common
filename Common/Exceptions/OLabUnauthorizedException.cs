using System;

namespace OLabWebAPI.Common.Exceptions
{
    public class OLabUnauthorizedException : Exception
    {
        public OLabUnauthorizedException(string type, uint id) : base($"Unauthorized reading {type} object with id {id}")
        {
        }

        public OLabUnauthorizedException() : base($"Unauthorized")
        {
        }
    }
}