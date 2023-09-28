using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_rpg.Dtos.BaseGrid;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Models;

namespace dotnet_rpg.Services.CharacterService
{
    public interface ICharacterService
    {
        Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters();
        Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id);
        Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter);
        Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter);
        Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id);
        Task<Object> GetAllStandard(int page = 1, int pageSize = 10, string sortBy = "Name", string filterBy = "");
        Task<BaseGridResponse<CharacterGridDto>> GetAllGeneric(BaseGridSearch search);
        Task<ServiceResponse<BaseGridResponse<Object>>> GetAllExpanded(ExpandedGridSearch search);
    }
}