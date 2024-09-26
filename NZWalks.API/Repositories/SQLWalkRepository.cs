using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public class SQLWalkRepository(NZWalksDBContext dBContext) : IWalkRepository
    {
        private readonly NZWalksDBContext dBContext = dBContext;

        public async Task<Walk> CreateAsync(Walk walk)
        {
            await dBContext.Walks.AddAsync(walk);
            await dBContext.SaveChangesAsync();
            return walk;
        }

        public async Task<Walk?> DeleteAsync(Guid id)
        {
            var existingWalk = await dBContext.Walks.FirstOrDefaultAsync(x => x.Id == id);
            if (existingWalk == null)
            {
                return null;
            }
            dBContext.Walks.Remove(existingWalk);
            await dBContext.SaveChangesAsync();
            return existingWalk;
        }

        public async Task<List<Walk>> GetAllAsync(string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool? isAsc = null, int pageNumber = 1, int pageSize = 1000)
        {

            var walks = dBContext.Walks.Include(x => x.Difficulty)
                                        .Include(x => x.Region)
                                        .AsQueryable();
            //Filtering
            if (string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
            {
                if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    walks = walks.Where(x => x.Name.Contains(filterQuery));
                }

            }

            //Sorting
            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    if ((bool)isAsc)
                    {
                        walks = walks.OrderBy(x => x.Name);
                    }
                    else
                    {
                        walks = walks.OrderByDescending(x => x.Name);
                    }
                }
                if (sortBy.Equals("Length", StringComparison.OrdinalIgnoreCase))
                {
                    if ((bool)isAsc)
                    {
                        walks = walks.OrderBy(x => x.LengthInKm);
                    }
                    else
                    {
                        walks = walks.OrderByDescending(x => x.LengthInKm);
                    }
                }
            }

            //Pagination 

            var skipResults = (pageNumber - 1) * pageSize;

            return await walks.Skip(skipResults).Take(pageSize).ToListAsync();
        }

        public async Task<Walk?> GetByIdAsync(Guid id)
        {
            return await dBContext.Walks.Include(x => x.Difficulty)
                                        .Include(x => x.Region)
                                        .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Walk?> UpdateAsync(Guid id, Walk walk)
        {
            var existingWalk = await dBContext.Walks.FirstOrDefaultAsync(x => x.Id == id);
            if (existingWalk == null)
            {
                return null;
            }
            else
            {
                existingWalk.Name = walk.Name;
                existingWalk.Description = walk.Description;
                existingWalk.LengthInKm = walk.LengthInKm;
                existingWalk.WalkImageUrl = walk.WalkImageUrl;

                existingWalk.DifficultyId = walk.DifficultyId;
                existingWalk.RegionId = walk.RegionId;

                await dBContext.SaveChangesAsync();
                return existingWalk;
            }
        }


    }
}
