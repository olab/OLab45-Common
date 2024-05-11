using OLab.Api.Model;
using Microsoft.Build.Framework;
using System.Linq;
using OLab.Api.Utils;
using OLab.Common.Interfaces;

namespace OLab.Data.ReaderWriters;

public class GroupRoleReaderWriter : ReaderWriter
{
  public static GroupRoleReaderWriter Instance(IOLabLogger logger, OLabDBContext context)
  {
    return new GroupRoleReaderWriter(logger, context);
  }

  public GroupRoleReaderWriter(IOLabLogger logger, OLabDBContext context) : base(logger, context)
  {
  }

  public bool Lookup(
    string groupName,
    Groups groupPhys,
    string roleName,
    Roles rolePhys)
  {
    groupPhys = _context.Groups.FirstOrDefault(x => x.Name.ToLower() == groupName.ToLower());
    rolePhys = _context.Roles.FirstOrDefault(x => x.Name.ToLower() == roleName.ToLower());

    if ((groupPhys == null) || (rolePhys == null))
      return false;

    return true;
  }

}
