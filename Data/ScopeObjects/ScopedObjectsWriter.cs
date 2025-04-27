using OLab.Api.Model;
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
    uint newMapId,
    CancellationToken token)
  {
    _logger.LogInformation( $"  Writing map {newMapId} ScopedObjects to database" );

    foreach ( var questionPhys in QuestionsPhys )
      await WriteQuestionToDatabaseAsync(
        questionPhys,
        token );

    foreach ( var constantPhys in ConstantsPhys )
      await WriteConstantToDatabaseAsync(
        constantPhys,
        token );

    foreach ( var filePhys in FilesPhys )
      await WriteFileToDatebaseAsync(
        filePhys,
        token );

    _counterIds.Clear();
    foreach ( var counterPhys in CountersPhys )
      await WriteCounterToDatabaseAsync(
        counterPhys,
        token );
  }

  private async Task WriteActionToDatabaseAsync(
    SystemCounterActions phys,
    CancellationToken token)
  {
    var oldId = phys.Id;

    phys.Id = 0;
    phys.CounterId = GetCounterIdCrossReference( phys.Id );

    if ( phys.ImageableType == Api.Utils.Constants.ScopeLevelMap )
      phys.ImageableId = GetMapIdCrossReference( phys.ImageableId );
    if ( phys.ImageableType == Api.Utils.Constants.ScopeLevelNode )
      phys.ImageableId = GetMapNodeIdCrossReference( phys.ImageableId );

    await _dbContext.SystemCounterActions.AddAsync( phys );
    await _dbContext.SaveChangesAsync( token );

    _logger.LogInformation( $"  wrote action for counter '{phys.CounterId}', {oldId}  ->  {phys.Id}" );
  }

  private async Task WriteCounterToDatabaseAsync(
    SystemCounters phys,
    CancellationToken token)
  {
    var oldId = phys.Id;
    phys.Id = 0;

    if ( phys.ImageableType == Api.Utils.Constants.ScopeLevelMap )
      phys.ImageableId = GetMapIdCrossReference( phys.ImageableId );
    if ( phys.ImageableType == Api.Utils.Constants.ScopeLevelNode )
      phys.ImageableId = GetMapNodeIdCrossReference( phys.ImageableId );

    await _dbContext.SystemCounters.AddAsync( phys );
    await _dbContext.SaveChangesAsync( token );

    // save the new counter id since creating
    // counter actions will need this mapping.
    AddCounterIdCrossReference( oldId, phys.Id );

    _logger.LogInformation( $"  wrote counter '{phys.Name}', {oldId}  ->  {phys.Id}" );
  }

  private async Task WriteFileToDatebaseAsync(
    SystemFiles phys,
    CancellationToken token)
  {
    var oldId = phys.Id;
    phys.Id = 0;

    if ( phys.ImageableType == Api.Utils.Constants.ScopeLevelMap )
      phys.ImageableId = GetMapIdCrossReference( phys.ImageableId );
    if ( phys.ImageableType == Api.Utils.Constants.ScopeLevelNode )
      phys.ImageableId = GetMapNodeIdCrossReference( phys.ImageableId );

    await _dbContext.SystemFiles.AddAsync( phys );
    await _dbContext.SaveChangesAsync( token );

    AddFileIdCrossReference( oldId, phys.Id );

    _logger.LogInformation( $"  wrote file '{phys.Name}' {phys.Path} {phys.Mime}, {oldId} -> {phys.Id}" );
  }

  private async Task WriteConstantToDatabaseAsync(
    SystemConstants phys,
    CancellationToken token)
  {
    var oldId = phys.Id;
    phys.Id = 0;

    if ( phys.ImageableType == Api.Utils.Constants.ScopeLevelMap )
      phys.ImageableId = GetMapIdCrossReference( phys.ImageableId );
    if ( phys.ImageableType == Api.Utils.Constants.ScopeLevelNode )
      phys.ImageableId = GetMapNodeIdCrossReference( phys.ImageableId );

    await _dbContext.SystemConstants.AddAsync( phys );
    await _dbContext.SaveChangesAsync( token );

    AddConstantIdCrossReference( oldId, phys.Id );

    _logger.LogInformation( $"  wrote constant '{phys.Name}', {oldId} -> {phys.Id}" );
  }

  private async Task WriteQuestionToDatabaseAsync(
    SystemQuestions phys,
    CancellationToken token)
  {
    var oldResponseIds = new List<uint>();

    var oldId = phys.Id;
    phys.Id = 0;

    if ( phys.ImageableType == Api.Utils.Constants.ScopeLevelMap )
      phys.ImageableId = GetMapIdCrossReference( phys.ImageableId );
    if ( phys.ImageableType == Api.Utils.Constants.ScopeLevelNode )
      phys.ImageableId = GetMapNodeIdCrossReference( phys.ImageableId );

    foreach ( var responsePhys in phys.SystemQuestionResponses )
    {
      oldResponseIds.Add( responsePhys.Id );

      responsePhys.Id = 0;
      responsePhys.QuestionId = 0;
    }

    await _dbContext.SystemQuestions.AddAsync( phys );
    await _dbContext.SaveChangesAsync( token );

    _logger.LogInformation( $"  wrote question '{phys.Stem}', {oldId} -> {phys.Id}" );

    AddQuestionIdCrossReference( oldId, phys.Id );

    var index = 0;
    foreach ( var responsePhys in phys.SystemQuestionResponses )
      _logger.LogInformation( $"    wrote response '{responsePhys.Response}', {oldResponseIds[ index++ ]} -> {responsePhys.Id}" );

  }

}
