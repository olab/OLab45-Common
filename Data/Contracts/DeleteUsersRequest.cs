using Newtonsoft.Json;
using OLab.Common.Interfaces;
using OLab.Data.ReaderWriters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace OLab.Api.Model;

public class DeleteUsersRequest
{
  [Required]
  public uint Id { get; set; }
}