using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLab.Data.Contracts;
public class SessionRequest
{
  public uint MapId { get; set; }
  public DateTime CourseDate { get; set; }
  public uint UserId { get; set; }
  public string Issuer { get; set; }
}
