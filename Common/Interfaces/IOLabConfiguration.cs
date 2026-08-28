using Microsoft.Extensions.Configuration;
using OLab.Common.Contracts;

namespace OLab.Common.Interfaces;
public interface IOLabConfiguration
{
  IConfiguration GetRawConfiguration();
  AppSettings GetAppSettings();
}