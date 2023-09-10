using Dawn;
using Microsoft.Extensions.Logging;
using OLab.Api.Utils;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace OLab.Api.Common
{
  public class OLabModuleProvider<T> : IOLabModuleProvider<T> where T : class
  {
    protected Dictionary<string, T> Modules = new Dictionary<string, T>();
    protected readonly OLabLogger Logger;

    public OLabModuleProvider(OLabLogger logger)
    {
      Guard.Argument(logger).NotNull(nameof(logger));

      Logger = new OLabLogger(logger.GetLoggerFactory(), logger.GetLoggerFactory().CreateLogger<OLabModuleProvider<T>>());
      Logger.LogInformation($"{GetType().Name} ctor");

    }

    /// <summary>
    /// Reloads the plug-in assemblies
    /// </summary>
    protected void Load(string sourceFiles)
    {
      Guard.Argument(sourceFiles).NotEmpty(nameof(sourceFiles));

      Logger.LogInformation($"{GetType().Name} loading {sourceFiles}");

      if (null == Modules)
        Modules = new Dictionary<string, T>();
      else
        Modules.Clear();

      var plugInAssemblies = LoadPlugInAssemblies(sourceFiles);
      Modules = GetPlugIns(plugInAssemblies);
    }

    /// <summary>
    /// Gets a list of assemblies in the Plugin directory
    /// </summary>
    /// <returns>List of assemblies</returns>
    private List<Assembly> LoadPlugInAssemblies(string sourceFiles)
    {
      var currentAssemblyFile = Assembly.GetExecutingAssembly().Location;
      var rootPath = Path.GetDirectoryName(currentAssemblyFile);

      // remove the URI specifier (both for Windows and Linux)
      rootPath = rootPath.Replace("file:\\", "");
      rootPath = rootPath.Replace("file:", "");

      if (!Directory.Exists(rootPath))
        throw new DirectoryNotFoundException($"Unable to load plugin path. '{rootPath}'");

      var files = Directory.GetFiles(rootPath, sourceFiles);

      var plugInAssemblyList = new List<Assembly>();

      foreach (var file in files)
      {
        Logger.LogInformation($"Loading file '{file}'");
        plugInAssemblyList.Add(Assembly.LoadFile(file));
      }

      return plugInAssemblyList;
    }

    /// <summary>
    /// Get installed Wiki tag plugs-in from a list of assemblies
    /// </summary>
    /// <param name="assemblies">List of assemblies</param>
    /// <returns>Dictionary of plug-ins</returns>
    private Dictionary<string, T> GetPlugIns(List<Assembly> assemblies)
    {
      var availableTypes = new List<Type>();

      foreach (var currentAssembly in assemblies)
        availableTypes.AddRange(currentAssembly.GetTypes());

      // get a list of objects that implement the IWikiTagModule interface AND 
      // have the WikiTagModuleAttribute
      var typeList = availableTypes.FindAll(delegate (Type t)
      {
        var interfaceTypes = new List<Type>(t.GetInterfaces());
        var arr = t.GetCustomAttributes(typeof(OLabModuleAttribute), true);
        return !(arr == null || arr.Length == 0) && interfaceTypes.Contains(typeof(IWikiTagModule));
      });

      var dict = new Dictionary<string, T>();
      foreach (var item in typeList)
      {
        Logger.LogInformation($"Loading type '{item.Name}'");
        var t = item.GetCustomAttribute<OLabModuleAttribute>();
        dict.Add(t.Name, (T)Activator.CreateInstance(item, Logger));
      }
      return dict;
    }

    /// <summary>
    /// Get module
    /// </summary>
    /// <param name="moduleName">Module name</param>
    /// <returns>Module</returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public T GetModule(string moduleName)
    {
      if (!Modules.ContainsKey(moduleName))
        throw new KeyNotFoundException($"{moduleName} not implemented");

      return Modules[moduleName];
    }
  }
}
