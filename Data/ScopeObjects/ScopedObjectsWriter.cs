using OLab.Api.Models;
using OLab.Api.Utils;
using OLab.Data.Models;
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
    uint newMapId,
    CancellationToken token)
  {
    Logger.LogInformation($"  Writing map {newMapId} ScopedObjects to database");

    foreach (var questionPhys in QuestionsPhys)
      await WriteQuestionToDatabaseAsync(
        questionPhys,
        token);

    foreach (var constantPhys in ConstantsPhys)
      await WriteConstantToDatabaseAsync(
        constantPhys,
        token);

    foreach (var filePhys in FilesPhys)
      await WriteFileToDatebaseAsync(
        filePhys,
        token);

    _counterIds.Clear();
    foreach (var counterPhys in CountersPhys)
      await WriteCounterToDatabaseAsync(
        counterPhys,
        token);

    foreach (var actionPhys in CounterActionsPhys)
      await WriteActionToDatabaseAsync(
        actionPhys,
        token);
  }

  private async Task WriteActionToDatabaseAsync(
    SystemCounterActions phys, 
    CancellationToken token)
  {
    var oldId = phys.Id;

    phys.Id = 0;
    phys.CounterId = GetCounterIdCrossReference(phys.Id);

    if ( phys.ImageableType == ConstantStrings.ScopeLevelMap )
      phys.ImageableId = GetMapIdCrossReference( phys.ImageableId );
    if ( phys.ImageableType == ConstantStrings.ScopeLevelNode )
      phys.ImageableId = GetMapNodeIdCrossReference( phys.ImageableId );

    await _dbContext.SystemCounterActions.AddAsync(phys);
    await _dbContext.SaveChangesAsync(token);

    Logger.LogInformation($"  imported action for counter '{phys.CounterId}', {oldId}  ->  {phys.Id}. scope type: {phys.ImageableType} id: {phys.ImageableId}");
  }

  private async Task WriteCounterToDatabaseAsync(
    SystemCounters phys, 
    CancellationToken token)
  {
    var oldId = phys.Id;
    phys.Id = 0;

    if ( phys.ImageableType == ConstantStrings.ScopeLevelMap )
      phys.ImageableId = GetMapIdCrossReference( phys.ImageableId );
    if ( phys.ImageableType == ConstantStrings.ScopeLevelNode )
      phys.ImageableId = GetMapNodeIdCrossReference( phys.ImageableId );

    await _dbContext.SystemCounters.AddAsync(phys);
    await _dbContext.SaveChangesAsync(token);

    // save the new counter id since creating
    // counter actions will need this mapping.
    AddCounterIdCrossReference(oldId, phys.Id);

    Logger.LogInformation($"  imported counter '{phys.Name}', {oldId}  ->  {phys.Id}");
  }

  private async Task WriteFileToDatebaseAsync(
    SystemFiles phys,
    CancellationToken token)
  {
    var oldId = phys.Id;
    phys.Id = 0;

    if ( phys.ImageableType == ConstantStrings.ScopeLevelMap )
      phys.ImageableId = GetMapIdCrossReference( phys.ImageableId );
    if ( phys.ImageableType == ConstantStrings.ScopeLevelNode )
      phys.ImageableId = GetMapNodeIdCrossReference( phys.ImageableId );


    await _dbContext.SystemFiles.AddAsync(phys);
    await _dbContext.SaveChangesAsync(token);

    Logger.LogInformation($"  imported file '{phys.Name}', {oldId} -> {phys.Id}");
  }

  private async Task WriteConstantToDatabaseAsync(
    SystemConstants phys,
    CancellationToken token)
  {
    var oldId = phys.Id;
    phys.Id = 0;

    if ( phys.ImageableType == ConstantStrings.ScopeLevelMap )
      phys.ImageableId = GetMapIdCrossReference( phys.ImageableId );
    if ( phys.ImageableType == ConstantStrings.ScopeLevelNode )
      phys.ImageableId = GetMapNodeIdCrossReference( phys.ImageableId );

    await _dbContext.SystemConstants.AddAsync(phys);
    await _dbContext.SaveChangesAsync(token);

    Logger.LogInformation($"  imported constant '{phys.Name}', {oldId} -> {phys.Id}");
  }

  private async Task WriteQuestionToDatabaseAsync(
    SystemQuestions phys,
    CancellationToken token)
  {
    var oldResponseIds = new List<uint>();

    var oldId = phys.Id;
    phys.Id = 0;

    if ( phys.ImageableType == ConstantStrings.ScopeLevelMap )
      phys.ImageableId = GetMapIdCrossReference( phys.ImageableId );
    if ( phys.ImageableType == ConstantStrings.ScopeLevelNode )
      phys.ImageableId = GetMapNodeIdCrossReference( phys.ImageableId );

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
}
