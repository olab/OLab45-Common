using AutoMapper;
using OLab.Api.Common;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using System;
using System.Collections.Generic;

namespace OLab.Api.ObjectMapper
{
  public class DateTimeTypeConverter : ITypeConverter<decimal, DateTime>
  {
    public DateTime Convert(decimal source, DateTime destination, ResolutionContext context)
    {
      return new System.DateTime(1970, 1, 1).AddSeconds(System.Convert.ToDouble(source));
    }
  }

  public class DecimalDateTimeTypeConverter : ITypeConverter<DateTime, decimal>
  {
    public decimal Convert(DateTime source, decimal destination, ResolutionContext context)
    {
      var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
      var diff = source.ToUniversalTime() - origin;
      return System.Convert.ToDecimal(Math.Floor(diff.TotalSeconds));
    }
  }

  public abstract class OLabMapper<P, D> : object where P : new() where D : new()
  {
    protected readonly Mapper _mapper;

    protected IOLabLogger Logger;
    protected WikiTagProvider _wikiTagModules = null;

    // used to hold on to id translation between origin system and new one
    protected IDictionary<uint, uint?> _idTranslation = new Dictionary<uint, uint?>();

    public virtual P ElementsToPhys(IEnumerable<dynamic> elements, Object source = null) { return default; }
    public WikiTagProvider GetWikiProvider() { return _wikiTagModules; }


    public OLabMapper(
      IOLabLogger logger)
    {
      Logger = OLabLogger.CreateNew<OLabMapper<P, D>>(logger);
      _mapper = new Mapper(GetConfiguration());
    }

    public OLabMapper(
      IOLabLogger logger,
      IOLabModuleProvider<IWikiTagModule> wikiTagProvider)
    {
      Logger = OLabLogger.CreateNew<OLabMapper<P, D>>(logger);

      _wikiTagModules = wikiTagProvider as WikiTagProvider;
      _mapper = new Mapper(GetConfiguration());
    }

    /// <summary>
    /// Default (overridable) AutoMapper cfg
    /// </summary>
    /// <returns>MapperConfiguration</returns>
    protected virtual MapperConfiguration GetConfiguration()
    {
      return new MapperConfiguration(cfg =>
      {
        cfg.CreateMap<P, D>().ReverseMap();
      });
    }

    /// <summary>
    /// Convert a physical object to a specific dto. 
    /// </summary>
    /// <remarks>
    /// Allows for derived class specific overrides that 
    /// don't fit well with default implementation
    /// </remarks>
    /// <param name="phys">Physical object</param>
    /// <param name="source">Base dto object</param>
    /// <returns>Dto object</returns>
    public virtual D PhysicalToDto(P phys, D source)
    {
      return source;
    }

    /// <summary>
    /// Convert a physical object to new dto. 
    /// </summary>
    /// <remarks>
    /// Allows for derived class specific overrides that 
    /// don't fit well with default implementation
    /// </remarks>
    /// <param name="phys">Physical object</param>
    /// <returns>Dto object</returns>
    public virtual D PhysicalToDto(P phys)
    {
      var dto = _mapper.Map<D>(phys);
      dto = PhysicalToDto(phys, dto);
      return dto;
    }

    /// <summary>
    /// Convert a list of physical objects to Dto
    /// </summary>
    /// <param name="physList">Physical objects</param>
    /// <returns></returns>
    public virtual IList<D> PhysicalToDto(IList<P> physList)
    {
      var dtoList = new List<D>();
      foreach (var phys in physList)
      {
        var dto = PhysicalToDto(phys);
        dtoList.Add(dto);
      }

      return dtoList;
    }

    /// <summary>
    /// Convert a dto object to a specific physicla one. 
    /// </summary>
    /// <remarks>
    /// Allows for derived class specific overrides that 
    /// don't fit well with default implementation
    /// </remarks>
    /// <param name="dto">Ddto object</param>
    /// <param name="source">Base physical object</param>
    /// <returns>Physical object</returns>
    public virtual P DtoToPhysical(D dto, P source)
    {
      return source;
    }

    /// <summary>
    /// Convert a dto object to new physical one. 
    /// </summary>
    /// <remarks>
    /// Allows for derived class specific overrides that 
    /// don't fit well with default implementation
    /// </remarks>
    /// <param name="dto">Dto object</param>
    /// <returns>Physical object</returns>
    public virtual P DtoToPhysical(D dto)
    {
      var phys = new P();
      _mapper.Map(dto, phys);

      phys = DtoToPhysical(dto, phys);
      return phys;
    }

    /// <summary>
    /// Convert a list of dto objects to physical
    /// </summary>
    /// <param name="dtoList">Dto objects</param>
    /// <returns></returns>
    public virtual IList<P> DtoToPhysical(IList<D> dtoList)
    {
      var physList = new List<P>();
      foreach (var dto in dtoList)
      {
        var phys = DtoToPhysical(dto);
        physList.Add(phys);
      }

      return physList;
    }

    /// <summary>
    /// Convert an Object to a Dto
    /// </summary>
    /// <param name="source"></param>
    /// <returns>Dto</returns>
    public virtual D GetDto(Object source = null)
    {
      if (source == null)
        return new D();

      return (D)source;
    }

    /// <summary>
    /// Convert an Object to a Physical
    /// </summary>
    /// <param name="source"></param>
    /// <returns>Physical</returns>
    public virtual P GetPhys(Object source = null)
    {
      if (source == null)
        return new P();

      return (P)source;
    }

    protected void CreateIdTranslation(uint originalId)
    {
      if (_idTranslation.ContainsKey(originalId))
        return;
      _idTranslation.Add(originalId, null);
    }

    protected bool SetIdTranslation(uint originalId, uint newId)
    {
      return _idTranslation.TryAdd(originalId, newId);
    }

    protected uint? GetIdTranslation(uint originalId)
    {
      if (!_idTranslation.TryGetValue(originalId, out var newId))
        return newId;

      throw new KeyNotFoundException($"Cound not find Id key {originalId}");
    }

  }
}