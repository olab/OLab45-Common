using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OLab.Api.Common;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data.Exceptions;
using OLab.Api.Data.Interface;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using OLab.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints;

public partial class QuestionsEndpoint : OLabEndpoint
{
  public QuestionsEndpoint(
    IOLabLogger logger,
    IOLabConfiguration configuration,
    OLabDBContext context,
    IOLabModuleProvider<IWikiTagModule> wikiTagProvider,
    IOLabModuleProvider<IFileStorageModule> fileStorageProvider)
    : base(
        logger,
        configuration,
        context,
        wikiTagProvider,
        fileStorageProvider)
  {
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  private bool Exists(uint id)
  {
    return GetDbContext().SystemQuestions.Any(e => e.Id == id);
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="take"></param>
  /// <param name="skip"></param>
  /// <returns></returns>
  public async Task<OLabAPIPagedResponse<QuestionsDto>> GetAsync([FromQuery] int? take, [FromQuery] int? skip)
  {

    GetLogger().LogInformation($"ReadAsync take={take} skip={skip}");

    var physList = new List<SystemQuestions>();
    var total = 0;
    var remaining = 0;

    if (!skip.HasValue)
      skip = 0;

    physList = await GetDbContext().SystemQuestions.OrderBy(x => x.Name).ToListAsync();
    total = physList.Count;

    if (take.HasValue && skip.HasValue)
    {
      physList = physList.Skip(skip.Value).Take(take.Value).ToList();
      remaining = total - take.Value - skip.Value;
    }

    GetLogger().LogInformation(string.Format("found {0} questions", physList.Count));

    var dtoList = new Questions(GetLogger(), _wikiTagProvider).PhysicalToDto(physList);

    var maps = GetDbContext().Maps.Select(x => new IdName() { Id = x.Id, Name = x.Name }).ToList();
    var nodes = GetDbContext().MapNodes.Select(x => new IdName() { Id = x.Id, Name = x.Title }).ToList();
    var servers = GetDbContext().Servers.Select(x => new IdName() { Id = x.Id, Name = x.Name }).ToList();

    foreach (var dto in dtoList)
      dto.ParentInfo = FindParentInfo(dto.ImageableType, dto.ImageableId, maps, nodes, servers);

    return new OLabAPIPagedResponse<QuestionsDto> { Data = dtoList, Remaining = remaining, Count = total };

  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  public async Task<QuestionsFullDto> GetAsync(
    IOLabAuthorization auth,
    uint id)
  {

    GetLogger().LogInformation($"ReadAsync id {id}");

    if (!Exists(id))
      throw new OLabObjectNotFoundException("QuestionsPhys", id);

    var phys = await GetDbContext().SystemQuestions.Include("SystemQuestionResponses").FirstAsync(x => x.Id == id);
    var builder = new QuestionsFullMapper(GetLogger());
    var dto = builder.PhysicalToDto(phys);

    // test if user has access to object
    var accessResult = await auth.HasAccessAsync(IOLabAuthorization.AclBitMaskRead, dto);
    if (accessResult is UnauthorizedResult)
      throw new OLabUnauthorizedException("QuestionsPhys", id);

    AttachParentObject(dto);

    return dto;

  }

  /// <summary>
  /// Saves a question edit
  /// </summary>
  /// <param name="id">question id</param>
  /// <returns>IActionResult</returns>
  public async Task PutAsync(
    IOLabAuthorization auth,
    uint id,
    QuestionsFullDto dto)
  {
    GetLogger().LogInformation($"PutAsync id {id}");

    dto.ImageableId = dto.ParentInfo.Id;

    // test if user has access to object
    var accessResult = await auth.HasAccessAsync(IOLabAuthorization.AclBitMaskWrite, dto);
    if (accessResult is UnauthorizedResult)
      throw new OLabUnauthorizedException("QuestionsPhys", id);

    try
    {
      var builder = new QuestionsFullMapper(GetLogger());
      var phys = builder.DtoToPhysical(dto);

      phys.UpdatedAt = DateTime.Now;

      GetDbContext().Entry(phys).State = EntityState.Modified;
      await GetDbContext().SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
      var existingObject = await GetQuestionAsync(id);
      if (existingObject == null)
        throw new OLabObjectNotFoundException("QuestionsPhys", id);
    }

  }

  /// <summary>
  /// Create new question
  /// </summary>
  /// <param name="dto">Question data</param>
  /// <returns>IActionResult</returns>
  public async Task<QuestionsFullDto> PostAsync(
    IOLabAuthorization auth,
    QuestionsFullDto dto)
  {
    GetLogger().LogInformation($"PostAsync name = {dto.Name}");

    dto.ImageableId = dto.ParentInfo.Id != 0 ? dto.ParentInfo.Id : dto.ImageableId;
    dto.Prompt = !string.IsNullOrEmpty(dto.Prompt) ? dto.Prompt : "";

    // test if user has access to object
    var accessResult = await auth.HasAccessAsync(IOLabAuthorization.AclBitMaskWrite, dto);
    if (accessResult is UnauthorizedResult)
      throw new OLabUnauthorizedException("QuestionsPhys", 0);

    var builder = new QuestionsFullMapper(GetLogger());
    var phys = builder.DtoToPhysical(dto);

    phys.CreatedAt = DateTime.Now;

    GetDbContext().SystemQuestions.Add(phys);
    await GetDbContext().SaveChangesAsync();

    var newPhys = await GetQuestionAsync(phys.Id);
    dto = builder.PhysicalToDto(newPhys);

    return dto;

  }

  /// <summary>
  /// Delete a question
  /// </summary>
  /// <param name="id"></param>
  /// <returns>No Content</returns>
  public async Task DeleteAsync(
    IOLabAuthorization auth,
    uint id)
  {
    GetLogger().LogInformation($"DeleteAsync id {id}");

    if (!Exists(id))
      throw new OLabObjectNotFoundException("Question", id);

    try
    {
      var phys = await GetQuestionAsync(id);
      var dto = new Questions(GetLogger(), _wikiTagProvider).PhysicalToDto(phys);

      // test if user has access to object
      var accessResult = await auth.HasAccessAsync(IOLabAuthorization.AclBitMaskWrite, dto);
      if (accessResult is UnauthorizedResult)
        throw new OLabUnauthorizedException("Question", id);

      if (GetDbContext().UserResponses.Any(x => x.QuestionId == id))
        throw new Exception($"Question {id} is in use. Cannot delete.");

      if (phys.SystemQuestionResponses.Count > 0)
        GetDbContext().SystemQuestionResponses.RemoveRange(phys.SystemQuestionResponses.ToArray());

      GetDbContext().SystemQuestions.Remove(phys);

      await GetDbContext().SaveChangesAsync();

    }
    catch (DbUpdateConcurrencyException)
    {
      var existingObject = await GetQuestionAsync(id)
        ?? throw new OLabObjectNotFoundException("Question", id);
    }

  }

}
