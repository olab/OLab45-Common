using Microsoft.Extensions.Logging;
using OLab.Api.Utils;
using System;
using System.Collections.Generic;

namespace OLab.Common.Interfaces;

public interface IOLabLogger
{
  bool HaveFatalError { get; }
  void Clear();
  ILogger GetLogger();
  ILoggerFactory GetLoggerFactory();
  IList<string> GetMessages(OLabLogMessage.MessageLevel level = OLabLogMessage.MessageLevel.Debug);
  bool HasErrorMessage();
  void LogDebug(string message);
  void LogDebug(string type, int index, string message);
  void LogError(Exception ex, string message = "");
  void LogError(string message);
  void LogError(string type, int index, Exception ex, string message);
  void LogError(string type, int index, string message);
  void LogFatal(string message);
  void LogFatal(string type, int index, string message);
  void LogInformation(string message);
  void LogInformation(string type, int index, string message);
  void LogWarning(string message);
  void LogWarning(string type, int index, string message);
}