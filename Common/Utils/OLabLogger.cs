using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OLab.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OLab.Api.Utils
{
  public class OLabLogMessage
  {
    public enum MessageLevel
    {
      Debug = 0,
      Info,
      Warn,
      Error,
      Fatal
    };

    public string ObjectType;
    public int ObjectIndex;
    public MessageLevel Level;
    public string LevelStr { get { return Level.ToString(); } }
    public string Message;
    public DateTime Time;

    public override string ToString()
    {
      return $"{Level}: {ObjectType}({ObjectIndex}): {Message}";
    }
  }

  public class OLabLogger : IOLabLogger
  {
    private readonly IList<OLabLogMessage> _messages = new List<OLabLogMessage>();
    private readonly ILogger _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly bool _keepMessages;

    public ILogger GetLogger() { return _logger; }
    public ILoggerFactory GetLoggerFactory() { return _loggerFactory; }

    // for cases where we don't have/need an actual ILogger
    public OLabLogger(bool keepMessages = false)
      : this(NullLoggerFactory.Instance, keepMessages)
    {
    }

    public OLabLogger(ILoggerFactory loggerFactory, bool keepMessages = false)
    {
      _loggerFactory = loggerFactory;
      _keepMessages = keepMessages;
      _logger = _loggerFactory.CreateLogger("default");
    }

    private OLabLogger(ILoggerFactory loggerFactory, ILogger logger, bool keepMessages = false)
    {
      _loggerFactory = loggerFactory;
      _logger = logger;
      _keepMessages = keepMessages;
    }

    public static IOLabLogger CreateNew<T>(IOLabLogger source)
    {
      var logger = new OLabLogger(source.GetLoggerFactory(), source.GetLoggerFactory().CreateLogger<T>());
      return logger;
    }

    public static IOLabLogger CreateNew<T>(ILoggerFactory loggerFactory, bool keepMessages = false)
    {
      var logger = new OLabLogger(loggerFactory, loggerFactory.CreateLogger<T>());
      return logger;
    }

    public bool HaveFatalError => _messages.Any(x => x.Level == OLabLogMessage.MessageLevel.Fatal);

    public IList<OLabLogMessage> GetMessages(OLabLogMessage.MessageLevel level = OLabLogMessage.MessageLevel.Debug)
    {
      return _messages.Where(x => x.Level >= level).ToList();
    }

    private void Log(OLabLogMessage.MessageLevel level, string message)
    {
      if (_keepMessages)
      {
        var obj = new OLabLogMessage
        {
          Message = message,
          Time = DateTime.Now,
          Level = level
        };
        _messages.Add(obj);
      }
    }

    private void Log(OLabLogMessage.MessageLevel level, string type, int index, string message)
    {
      var obj = new OLabLogMessage
      {
        ObjectIndex = index,
        ObjectType = type,
        Message = message,
        Time = DateTime.Now,
        Level = level
      };
      _messages.Add(obj);
    }

    public void LogDebug(string message)
    {
      if (_logger != null)
        _logger.LogDebug(message);
      Log(OLabLogMessage.MessageLevel.Debug, message);
    }

    public void LogDebug(string type, int index, string message)
    {
      if (_logger != null)
        _logger.LogDebug(message);
      Log(OLabLogMessage.MessageLevel.Debug, type, index, message);
    }

    public void LogFatal(string message)
    {
      if (_logger != null)
        _logger.LogCritical(message);
      Log(OLabLogMessage.MessageLevel.Fatal, message);
    }

    public void LogFatal(string type, int index, string message)
    {
      if (_logger != null)
        _logger.LogCritical(message);
      Log(OLabLogMessage.MessageLevel.Fatal, type, index, message);
    }

    public void LogError(Exception ex, string message)
    {
      if (_logger != null)
        _logger.LogError(ex, message);
      Log(OLabLogMessage.MessageLevel.Error, message);
    }

    public void LogError(string type, int index, Exception ex, string message)
    {
      if (_logger != null)
        _logger.LogError(ex, message);
      Log(OLabLogMessage.MessageLevel.Error, type, index, message);
    }

    public void LogError(string message)
    {
      if (_logger != null)
        _logger.LogError(message);
      Log(OLabLogMessage.MessageLevel.Error, message);
    }

    public void LogError(string type, int index, string message)
    {
      if (_logger != null)
        _logger.LogError(message);
      Log(OLabLogMessage.MessageLevel.Error, type, index, message);
    }

    public void LogInformation(string message)
    {
      if (_logger != null)
        _logger.LogInformation(message);
      Log(OLabLogMessage.MessageLevel.Info, message);
    }

    public void LogWarning(string message)
    {
      if (_logger != null)
        _logger.LogWarning(message);
      Log(OLabLogMessage.MessageLevel.Warn, message);
    }

    public void LogInformation(string type, int index, string message)
    {
      if (_logger != null)
        _logger.LogInformation(message);
      Log(OLabLogMessage.MessageLevel.Info, type, index, message);
    }

    public void LogWarning(string type, int index, string message)
    {
      if (_logger != null)
        _logger.LogWarning(message);
      Log(OLabLogMessage.MessageLevel.Warn, type, index, message);
    }

  }

}