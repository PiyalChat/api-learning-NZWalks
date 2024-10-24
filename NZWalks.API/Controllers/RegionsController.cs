using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.CustomActionsFilters;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RegionsController(NZWalksDBContext _dBContext, IRegionRepository _regionRepository, IMapper _mapper) : ControllerBase
    {
        private readonly NZWalksDBContext dBContext = _dBContext;
        private readonly IRegionRepository regionRepository= _regionRepository;
        private readonly IMapper mapper = _mapper;

        //Get all regions
        [HttpGet]
        public async Task<IActionResult> ReadAllRegions()
        {
            var regionDomain = await regionRepository.ReadAllRegions();

            return Ok(mapper.Map<List<RegionDto>>(regionDomain));            
        }

        //Get region by single id
        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> ReadRegionById([FromRoute] Guid id)
        {
            var regionDomain = await regionRepository.ReadRegionById(id);

            if (regionDomain == null)
            {
                return NotFound();
            }
            
            return Ok(mapper.Map<RegionDto>(regionDomain));
        }

        //Post a region
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> CreateRegion([FromBody] AddRegionRequestDto addRegionRequestDto)
        {

            //Map or convert DTO to domain model
            var regionDomainModel = mapper.Map<Region>(addRegionRequestDto);

            //Use domain to save in db
            regionDomainModel = await regionRepository.CreateRegion(regionDomainModel);

            //map domain back to dto

            var regionDTO = mapper.Map<RegionDto>(regionDomainModel);
            return CreatedAtAction(nameof(ReadRegionById), new { id = regionDTO.Id }, regionDTO);

        }

        //Update a region
        [HttpPut]
        [Route("{id:guid}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateRegion([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
            //Check if region exist
            var regionDomainModel = mapper.Map<Region>(updateRegionRequestDto);


            regionDomainModel =  await regionRepository.UpdateRegion(id, regionDomainModel);
            if(regionDomainModel == null)
            {
                return NotFound();
            }

            
            return Ok(mapper.Map<RegionDto>(regionDomainModel));
        }

        //Delete a region

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteRegion([FromRoute] Guid id)
        {
            var regionDomainModel = await regionRepository.DeleteRegion(id);
            if (regionDomainModel == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<RegionDto>(regionDomainModel));
        }

    }
}
