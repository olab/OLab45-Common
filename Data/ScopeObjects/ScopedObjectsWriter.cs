using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OLab.Api.Models;
using OLab.Api.Utils;
using OLab.Common.Exceptions;
using OLab.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Data;

public partial class ScopedObjects
{
  /// <summary>
  /// Writes scoped objects to database under new map
  /// </summary>
  /// <param name="newMapId">New map id</param>
  /// <param name="token"></param>
  public async Task WriteAllToDatabaseAsync(
    IDbContextTransaction transaction,
    uint newMapId,
    CancellationToken token)
  {
    Logger.LogInformation($"  Writing map {newMapId} ScopedObjects to database");

    foreach (var questionPhys in QuestionsPhys)
      await WriteQuestionToDatabaseAsync(
        transaction,
        questionPhys,
        token);

    foreach (var constantPhys in ConstantsPhys)
      await WriteConstantToDatabaseAsync(
        transaction,
        constantPhys,
        token);

    foreach (var filePhys in FilesPhys)
      await WriteFileToDatebaseAsync(
        transaction,
        filePhys,
        token);

    _counterIds.Clear();
    foreach (var counterPhys in CountersPhys)
      await WriteCounterToDatabaseAsync(
        transaction,
        counterPhys,
        token);

    foreach (var actionPhys in CounterActionsPhys)
      await WriteActionToDatabaseAsync(
        transaction,
        actionPhys,
        token);
  }

  private async Task WriteActionToDatabaseAsync(
    IDbContextTransaction transaction,
    SystemCounterActions phys,
    CancellationToken token)
  {
    var oldId = phys.Id;

    try
    {

      phys.Id = 0;
      phys.CounterId = GetCounterIdCrossReference(phys.Id);

      if (phys.ImageableType == ConstantStrings.ScopeLevelMap)
        phys.ImageableId = GetMapIdCrossReference(phys.ImageableId);
      if (phys.ImageableType == ConstantStrings.ScopeLevelNode)
        phys.ImageableId = GetMapNodeIdCrossReference(phys.ImageableId);

      await _dbContext.SystemCounterActions.AddAsync(phys);
      await _dbContext.SaveChangesAsync(token);

      Logger.LogInformation($"  imported action for counter '{phys.CounterId}', {oldId}  ->  {phys.Id}. scope type: {phys.ImageableType} id: {phys.ImageableId}");
    }
    catch (Exception ex)
    {
      _dbContext.Remove(phys);
      Logger.LogError(ex, $"import counter action '{oldId}'");
    }

  }

  private async Task WriteCounterToDatabaseAsync(
    IDbContextTransaction transaction,
    SystemCounters phys,
    CancellationToken token)
  {
    var oldId = phys.Id;

    try
    {
      phys.Id = 0;

      if (phys.ImageableType == ConstantStrings.ScopeLevelMap)
        phys.ImageableId = GetMapIdCrossReference(phys.ImageableId);
      if (phys.ImageableType == ConstantStrings.ScopeLevelNode)
        phys.ImageableId = GetMapNodeIdCrossReference(phys.ImageableId);

      await _dbContext.SystemCounters.AddAsync(phys);
      await _dbContext.SaveChangesAsync(token);

      // save the new counter id since creating
      // counter actions will need this mapping.
      AddCounterIdCrossReference(oldId, phys.Id);

      Logger.LogInformation($"  imported counter '{phys.Name}', {oldId}  ->  {phys.Id}");
    }
    catch (Exception ex)
    {
      _dbContext.Remove(phys);
      Logger.LogError(ex, $"importing counter id {oldId}, name: '{phys.Name}'");
    }


  }

  private async Task WriteFileToDatebaseAsync(
    IDbContextTransaction transaction,
    SystemFiles phys,
    CancellationToken token)
  {
    var oldId = phys.Id;

    try
    {
      phys.Id = 0;

      if (phys.ImageableType == ConstantStrings.ScopeLevelMap)
        phys.ImageableId = GetMapIdCrossReference(phys.ImageableId);
      if (phys.ImageableType == ConstantStrings.ScopeLevelNode)
        phys.ImageableId = GetMapNodeIdCrossReference(phys.ImageableId);


      await _dbContext.SystemFiles.AddAsync(phys);
      await _dbContext.SaveChangesAsync(token);

      Logger.LogInformation($"  imported file '{phys.Name}', {oldId} -> {phys.Id}");
    }
    catch (Exception ex)
    {
      _dbContext.Remove(phys);
      Logger.LogError(ex, $"importing file id {oldId}, name: '{phys.Name}'");
    }

  }

  private async Task WriteConstantToDatabaseAsync(
    IDbContextTransaction transaction,
    SystemConstants phys,
    CancellationToken token)
  {
    var oldId = phys.Id;

    try
    {
      phys.Id = 0;

      if (phys.ImageableType == ConstantStrings.ScopeLevelMap)
        phys.ImageableId = GetMapIdCrossReference(phys.ImageableId);
      if (phys.ImageableType == ConstantStrings.ScopeLevelNode)
        phys.ImageableId = GetMapNodeIdCrossReference(phys.ImageableId);

      await _dbContext.SystemConstants.AddAsync(phys);
      await _dbContext.SaveChangesAsync(token);

      Logger.LogInformation($"  imported constant '{phys.Name}', {oldId} -> {phys.Id}");
    }
    catch (Exception ex)
    {
      _dbContext.Remove(phys);
      Logger.LogError(ex, $"importing constant id {oldId}, name: '{phys.Name}'");
    }


  }

  private async Task WriteQuestionToDatabaseAsync(
    IDbContextTransaction transaction,
    SystemQuestions phys,
    CancellationToken token)
  {
    var oldId = phys.Id;

    try
    {
      var oldResponseIds = new List<uint>();

      phys.Id = 0;

      if (phys.ImageableType == ConstantStrings.ScopeLevelMap)
        phys.ImageableId = GetMapIdCrossReference(phys.ImageableId);
      if (phys.ImageableType == ConstantStrings.ScopeLevelNode)
        phys.ImageableId = GetMapNodeIdCrossReference(phys.ImageableId);

      foreach (var responsePhys in phys.SystemQuestionResponses)
      {
        oldResponseIds.Add(responsePhys.Id);

        responsePhys.Id = 0;
        responsePhys.QuestionId = 0;
      }

      await _dbContext.SystemQuestions.AddAsync(phys);
      await _dbContext.SaveChangesAsync(token);

      Logger.LogInformation($"  imported question '{phys.Stem}', {oldId} -> {phys.Id}");

      AddQuestionIdCrossReference(oldId, phys.Id);

      int index = 0;
      foreach (var responsePhys in phys.SystemQuestionResponses)
        Logger.LogInformation($"    imported response '{responsePhys.Response}', {oldResponseIds[index++]} -> {responsePhys.Id}");

    }
    catch (Exception ex)
    {
      _dbContext.Remove(phys);
      Logger.LogError(ex, $"importing question id {oldId}, name: '{phys.Name}'");
    }

  }
}
