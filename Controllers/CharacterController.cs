using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using dotnet_rpg.Models;
using dotnet_rpg.Services.CharacterService;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Dtos.BaseGrid;

namespace dotnet_rpg.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _characterService;

        public CharacterController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> Get()
        {
            return Ok(await _characterService.GetAllCharacters());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> GetSingle(int id)
        {
            return Ok(await _characterService.GetCharacterById(id));
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> AddCharacter(AddCharacterDto newCharacter)
        {
            return Ok(await _characterService.AddCharacter(newCharacter));
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
            var response = await _characterService.UpdateCharacter(updatedCharacter);
            if(response.Data is null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> DeleteCharacter(int id)
        {
            var response = await _characterService.DeleteCharacter(id);
            if(response.Data is null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet("GetAllStandard")]
        public async Task<ActionResult<Object>> GetAllStandard(int page, int pageSize, string sortBy, string? filterBy)
        {
            if(string.IsNullOrEmpty(filterBy))
            {
            return Ok(await _characterService.GetAllStandard(page, pageSize, sortBy));
            }

            return Ok(await _characterService.GetAllStandard(page, pageSize, sortBy, filterBy));
        }

        [HttpPost("GetAllGeneric")]
        public async Task<ActionResult<BaseGridResponse<CharacterGridDto>>> GetAllGeneric([FromBody] BaseGridSearch search)
        {
            return Ok(await _characterService.GetAllGeneric(search));
        }

        [HttpPost("GetAllExpanded")]
        public async Task<ActionResult<ServiceResponse<BaseGridResponse<Object>>>> GetAllExpanded(ExpandedGridSearch search)
        {
            return Ok(await _characterService.GetAllExpanded(search));
        }
    }
}