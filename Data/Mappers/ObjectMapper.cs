using OLab.Api.Common;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using System;
using System.Collections.Generic;

namespace OLab.Api.ObjectMapper
{
  public abstract class ObjectMapper<P, D> : object where P : new() where D : new()
  {
    protected IOLabLogger Logger;
    protected WikiTagProvider _tagProvider = null;
    public WikiTagProvider GetWikiProvider() { return _tagProvider; }

    // used to hold on to id translation between origin system and new one
    protected IDictionary<uint, uint?> _idTranslation = new Dictionary<uint, uint?>();

    public abstract D PhysicalToDto(P phys, Object source = null);
    public abstract P DtoToPhysical(D dto, Object source = null);
    public virtual P ElementsToPhys(IEnumerable<dynamic> elements, Object source = null) { return default; }

    public ObjectMapper(IOLabLogger logger) : this(logger, new WikiTagProvider(logger))
    {
    }

    public ObjectMapper(IOLabLogger logger, WikiTagProvider tagProvider)
    {
      Logger = OLabLogger.CreateNew<ObjectMapper<P, D>>(logger);
      _tagProvider = new WikiTagProvider(Logger);
    }

    public virtual D GetDto(Object source = null)
    {
      if (source == null)
        return new D();

      return (D)source;
    }

    public virtual P GetPhys(Object source = null)
    {
      if (source == null)
        return new P();

      return (P)source;
    }

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