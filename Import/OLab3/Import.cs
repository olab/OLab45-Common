using OLab.Api.Common;
using OLab.Api.Data.Interface;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using OLab.Common.Utils;
using OLab.Data;
using OLab.Data.Interface;
using OLab.Import.Interface;
using OLab.Import.OLab3.Dtos;
using OLab.Import.OLab3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Import.OLab3;

 public partial class Importer : OLabImporter
{

  public async override Task<Maps> Import(
    IOLabAuthorization auth,
    Stream archiveFileStream,
    string archiveFileName,
    CancellationToken token = default)
  {
    Authorization = auth;

    await LoadImportFromArchiveFile(
      archiveFileStream, 
      archiveFileName, 
      token);

    var mapPhys = WriteImportToDatabase(
      archiveFileName);
    return mapPhys;
  }

  /// <summary>
  /// Loads import files into memory
  /// </summary>
  /// <param name="archiveFileStream">Stream to write file to</param>
  /// <param name="archiveFileName">Import archive ZIP file name</param>
  /// <returns>true</returns>
  public async Task LoadImportFromArchiveFile(
    Stream archiveFileStream,
    string archiveFileName,
    CancellationToken token = default)
  {
    try
    {
      Logger.LogInformation($"Module archive file: {GetFileModule().BuildPath(OLabFileStorageModule.ImportRoot, archiveFileName)}");

      var archiveFilePath = await GetFileModule().WriteImportFileAsync(
        archiveFileStream,
        archiveFileName,
        token);

      // load all the import files in extract directory
      foreach (var dto in _dtos.Values)
        await dto.LoadAsync(Path.GetFileNameWithoutExtension(archiveFileName));

      // delete source archive file
      await GetFileModule().DeleteImportFileAsync(
        ".", 
        archiveFileName);
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, $"Read error: {ex.Message}");
      throw;
    }

  }

  /// <summary>
  /// WRite import data in memory to database
  /// </summary>
  /// <param name="archiveFileName">Import archive ZIP file name</param>
  /// <returns>true</returns>
  public Maps WriteImportToDatabase(
    string archiveFileName)
  {

    // if anything bad happens, rollback the entire import
    using (var transaction = GetDbContext().Database.BeginTransaction())
    {
      try
      {
        // save all import data sets to database
        foreach (var dto in _dtos.Values)
          dto.SaveToDatabase(Path.GetFileNameWithoutExtension(archiveFileName));

        transaction.Commit();

        // get the new id of thie imported map
        var xmlMapDto = _dtos[DtoTypes.XmlMapDto] as XmlMapDto;
        var xmlMap = (XmlMap)xmlMapDto.GetDbPhys();
        Logger.LogInformation($"Loaded map '{xmlMap.Data[0].Name}'. id = {xmlMap.Data[0].Id}");

        var mapPhys = xmlMap.Data[0];
        return mapPhys;

      }
      catch (Exception ex)
      {
        Logger.LogError(ex, $"Error saving import. reason: {ex.Message} ");
        transaction.Rollback();
      }

    }

    return null;
  }

}