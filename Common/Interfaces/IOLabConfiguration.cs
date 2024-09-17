using Microsoft.Extensions.Configuration;
using OLab.Api.Utils;

namespace OLab.Common.Interfaces;
public interface IOLabConfiguration
{
  IConfiguration GetRawConfiguration();
  AppSettings GetAppSettings();
}