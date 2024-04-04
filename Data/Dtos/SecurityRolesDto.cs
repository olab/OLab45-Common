using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLab.Data.Dtos;

public class SecurityRolesDto
{
    public string Acl { get; set; }
    public string ImageableType { get; set; }
    public uint GroupId { get; set; }
    public uint Id { get; set; }
    public uint ImageableId { get; set; }
    public uint RoleId { get; set; }
}
