using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utils
{
  public static class ConnectionId
  {
    public static string Shorten(string connectionId) 
    {
      if (string.IsNullOrEmpty(connectionId))
        return "<none>";
      return connectionId.Substring(connectionId.Length - 3); 
    }
  }
}
