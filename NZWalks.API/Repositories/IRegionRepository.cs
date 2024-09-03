using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;

namespace NZWalks.API.Repositories
{
    public interface IRegionRepository
    {
        Task<List<Region>> ReadAllRegions();
        Task<Region?> ReadRegionById(Guid id);
        Task<Region> CreateRegion(Region region);
        Task<Region?> UpdateRegion(Guid id, Region region);
        Task<Region?> DeleteRegion(Guid id);
    }
}
