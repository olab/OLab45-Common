using OLab.Api.Model;
using OLab.Common.Utils;
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
    phys.CounterId = Convert.ToUInt32( GetCounterCrossReference( phys.Id.ToString() ) );

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
    var oldPhys = SerializerUtilities.DeepCopy( phys );
    phys.Id = 0;

    // remap if name is same as id
    var rename = oldPhys.Name == oldPhys.Id.ToString();

    if ( phys.ImageableType == Api.Utils.Constants.ScopeLevelMap )
      phys.ImageableId = GetMapIdCrossReference( phys.ImageableId );
    if ( phys.ImageableType == Api.Utils.Constants.ScopeLevelNode )
      phys.ImageableId = GetMapNodeIdCrossReference( phys.ImageableId );

    await _dbContext.SystemCounters.AddAsync( phys );
    await _dbContext.SaveChangesAsync( token );

    AddCounterCrossReference( oldPhys, phys );

    if ( rename )
    {
      phys.Name = phys.Id.ToString();
      _dbContext.SystemCounters.Update( phys );
      await _dbContext.SaveChangesAsync( token );
    }

    _logger.LogInformation( $"  wrote file '{phys.Name}', {oldPhys.Id} -> {phys.Id}" );
  }

  private async Task WriteFileToDatebaseAsync(
    SystemFiles phys,
    CancellationToken token)
  {
    var oldPhys = SerializerUtilities.DeepCopy( phys );
    phys.Id = 0;

    // remap if name is same as id
    var rename = oldPhys.Name == oldPhys.Id.ToString();

    if ( phys.ImageableType == Api.Utils.Constants.ScopeLevelMap )
      phys.ImageableId = GetMapIdCrossReference( phys.ImageableId );
    if ( phys.ImageableType == Api.Utils.Constants.ScopeLevelNode )
      phys.ImageableId = GetMapNodeIdCrossReference( phys.ImageableId );

    await _dbContext.SystemFiles.AddAsync( phys );
    await _dbContext.SaveChangesAsync( token );

    AddFileCrossReference( oldPhys, phys );

    if ( rename )
    {
      phys.Name = phys.Id.ToString();
      _dbContext.SystemFiles.Update( phys );
      await _dbContext.SaveChangesAsync( token );
    }

    _logger.LogInformation( $"  wrote file '{phys.Name}', {oldPhys.Id} -> {phys.Id}" );
  }

  private async Task WriteConstantToDatabaseAsync(
    SystemConstants phys,
    CancellationToken token)
  {
    var oldPhys = SerializerUtilities.DeepCopy( phys );
    phys.Id = 0;

    // remap if name is same as id
    var rename = oldPhys.Name == oldPhys.Id.ToString();

    if ( phys.ImageableType == Api.Utils.Constants.ScopeLevelMap )
      phys.ImageableId = GetMapIdCrossReference( phys.ImageableId );
    if ( phys.ImageableType == Api.Utils.Constants.ScopeLevelNode )
      phys.ImageableId = GetMapNodeIdCrossReference( phys.ImageableId );

    await _dbContext.SystemConstants.AddAsync( phys );
    await _dbContext.SaveChangesAsync( token );

    AddConstantCrossReference( oldPhys, phys );

    if ( rename )
    {
      phys.Name = phys.Id.ToString();
      _dbContext.SystemConstants.Update( phys );
      await _dbContext.SaveChangesAsync( token );
    }

    _logger.LogInformation( $"  wrote constant '{phys.Name}', {oldPhys.Id} -> {phys.Id}" );
  }

  private async Task WriteQuestionToDatabaseAsync(
    SystemQuestions phys,
    CancellationToken token)
  {
    var oldResponseIds = new List<uint>();

    var oldPhys = SerializerUtilities.DeepCopy( phys );
    phys.Id = 0;

    // remap if name is same as id
    var rename = oldPhys.Name == oldPhys.Id.ToString();

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

    AddQuestionCrossReference( oldPhys, phys );

    if ( rename )
    {
      phys.Name = phys.Id.ToString();
      _dbContext.SystemQuestions.Update( phys );
      await _dbContext.SaveChangesAsync( token );
    }

    _logger.LogInformation( $"  wrote question '{phys.Stem}', {oldPhys.Id} -> {phys.Id}" );

    var index = 0;
    foreach ( var responsePhys in phys.SystemQuestionResponses )
      _logger.LogInformation( $"    wrote response '{responsePhys.Response}', {oldResponseIds[ index++ ]} -> {responsePhys.Id}" );

  }

}
