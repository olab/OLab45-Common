using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OLabWebAPI.Common;
using OLabWebAPI.Common.Exceptions;
using OLabWebAPI.Data.Interface;
using OLabWebAPI.Dto;
using OLabWebAPI.Model;
using OLabWebAPI.ObjectMapper;
using OLabWebAPI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLabWebAPI.Endpoints
{
    public partial class FilesEndpoint : OlabEndpoint
    {

        public FilesEndpoint(
          OLabLogger logger,
          OLabDBContext context) : base(logger, context)
        {
        }

        private bool Exists(uint id)
        {
            return dbContext.SystemFiles.Any(e => e.Id == id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public async Task<OLabAPIPagedResponse<FilesDto>> GetAsync(int? take, int? skip)
        {
            logger.LogDebug($"FilesController.GetAsync([FromQuery] int? take={take}, [FromQuery] int? skip={skip})");

            var Files = new List<SystemFiles>();
            var total = 0;
            var remaining = 0;

            if (!skip.HasValue)
                skip = 0;

            Files = await dbContext.SystemFiles.OrderBy(x => x.Name).ToListAsync();
            total = Files.Count;

            if (take.HasValue && skip.HasValue)
            {
                Files = Files.Skip(skip.Value).Take(take.Value).ToList();
                remaining = total - take.Value - skip.Value;
            }

            logger.LogDebug(string.Format("found {0} Files", Files.Count));

            IList<FilesDto> dtoList = new ObjectMapper.Files(logger).PhysicalToDto(Files);
            return new OLabAPIPagedResponse<FilesDto> { Data = dtoList, Remaining = remaining, Count = total };

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<FilesFullDto> GetAsync(
          IOLabAuthentication auth,
          uint id)
        {

            logger.LogDebug($"FilesController.GetAsync(uint id={id})");

            if (!Exists(id))
                throw new OLabObjectNotFoundException("Files", id);

            SystemFiles phys = await dbContext.SystemFiles.FirstAsync(x => x.Id == id);
            FilesFullDto dto = new ObjectMapper.FilesFull(logger).PhysicalToDto(phys);

            // test if user has access to object
            IActionResult accessResult = auth.HasAccess("R", dto);
            if (accessResult is UnauthorizedResult)
                throw new OLabUnauthorizedException("Files", id);

            AttachParentObject(dto);
            return dto;

        }

        /// <summary>
        /// Saves a file edit
        /// </summary>
        /// <param name="id">file id</param>
        /// <returns>IActionResult</returns>
        public async Task PutAsync(
          IOLabAuthentication auth,
          uint id, FilesFullDto dto)
        {

            logger.LogDebug($"PutAsync(uint id={id})");

            dto.ImageableId = dto.ParentObj.Id;

            // test if user has access to object
            IActionResult accessResult = auth.HasAccess("W", dto);
            if (accessResult is UnauthorizedResult)
                throw new OLabUnauthorizedException("Files", id);

            try
            {
                var builder = new FilesFull(logger);
                SystemFiles phys = builder.DtoToPhysical(dto);

                phys.UpdatedAt = DateTime.Now;

                dbContext.Entry(phys).State = EntityState.Modified;
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                SystemConstants existingObject = await GetConstantAsync(id);
                if (existingObject == null)
                    throw new OLabObjectNotFoundException("Files", id);
            }

        }

        /// <summary>
        /// Create new file
        /// </summary>
        /// <param name="phys">Physical object to save</param>
        /// <returns>FilesFullDto</returns>
        public async Task<FilesFullDto> PostAsync(
          IOLabAuthentication auth,
          SystemFiles phys)
        {
            logger.LogDebug($"FilesController.PostAsync()");
            var builder = new FilesFull(logger);
            FilesFullDto dto = builder.PhysicalToDto(phys);

            // test if user has access to object
            IActionResult accessResult = auth.HasAccess("W", dto);
            if (accessResult is UnauthorizedResult)
                throw new OLabUnauthorizedException("Files", 0);

            phys.CreatedAt = DateTime.Now;

            dbContext.SystemFiles.Add(phys);
            await dbContext.SaveChangesAsync();

            dto = builder.PhysicalToDto(phys);
            return dto;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(
          IOLabAuthentication auth,
          uint id)
        {

            logger.LogDebug($"ConstantsEndpoint.DeleteAsync(uint id={id})");

            if (!Exists(id))
                throw new OLabObjectNotFoundException("Files", id);

            try
            {
                SystemFiles phys = await GetFileAsync(id);
                FilesFullDto dto = new FilesFull(logger).PhysicalToDto(phys);

                // test if user has access to object
                IActionResult accessResult = auth.HasAccess("W", dto);
                if (accessResult is UnauthorizedResult)
                    throw new OLabUnauthorizedException("Constants", id);

                dbContext.SystemFiles.Remove(phys);
                await dbContext.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                SystemFiles existingObject = await GetFileAsync(id);
                if (existingObject == null)
                    throw new OLabObjectNotFoundException("Files", id);
            }

        }

    }

}

