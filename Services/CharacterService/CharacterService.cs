using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.BaseGrid;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        private static Dictionary<string, string> ColumnMapping = new Dictionary<string, string>
        {
            {
                "name", "Name"
            },
            {
                "hitpoints", "HitPoints"
            }
        };

        public CharacterService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            var character = _mapper.Map<Character>(newCharacter);

            _context.Characters.Add(character);
            await _context.SaveChangesAsync();

            serviceResponse.Data =
                await _context.Characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();

            try
            {
                var character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);
                if (character is null)
                    throw new Exception($"Character with Id '{id}' not found.");

                _context.Characters.Remove(character);

                await _context.SaveChangesAsync();

                serviceResponse.Data =
                    await _context.Characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            var dbCharacters = await _context.Characters.ToListAsync();
            serviceResponse.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();
            var dbCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);
            serviceResponse.Data = _mapper.Map<GetCharacterDto>(dbCharacter);
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();
            try
            {
                var character =
                    await _context.Characters.FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);
                if (character is null)
                    throw new Exception($"Character with Id '{updatedCharacter.Id}' not found.");

                character.Name = updatedCharacter.Name;
                character.HitPoints = updatedCharacter.HitPoints;
                character.Strength = updatedCharacter.Strength;
                character.Defense = updatedCharacter.Defense;
                character.Intelligence = updatedCharacter.Intelligence;
                character.Class = updatedCharacter.Class;

                await _context.SaveChangesAsync();
                serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<Object> GetAllStandard(int page = 1, int pageSize = 10, string sortBy = "Name", string filterBy = "")
        {
            var query = _context.Characters.AsQueryable();

            if(!string.IsNullOrEmpty(filterBy)) {
                query = query.Where(c => c.Name.Contains(filterBy));
            }

            switch(sortBy.ToLower())
            {
                case "name":
                    query = query.OrderBy(c => c.Name);
                    break;
                case "hitpoints":
                    query = query.OrderBy(c => c.HitPoints);
                    break;
                default:
                    query = query.OrderBy(c => c.Id);
                    break;
            }

            var totalCount = query.Count();

            var dbCharacters = await query.Skip((page-1)*pageSize).Take(pageSize).ToListAsync();
            var characters = dbCharacters.Select(c => new CharacterGridDto { 
                Id = c.Id,
                Name = c.Name,
                HitPoints = c.HitPoints,
                Strength = c.Strength,
                Defense = c.Defense,
                Intelligence = c.Intelligence,
                Class = c.Class
            }).ToList();

            return new {
                Characters = characters,
                TotalCount = totalCount
            };
        }
        public async Task<BaseGridResponse<CharacterGridDto>> GetAllGeneric(BaseGridSearch search)
        {
            var query = _context.Characters.AsQueryable();

            if (!string.IsNullOrEmpty(search.FilterBy))
            {
                query = query.Where(c => c.Name.Contains(search.FilterBy));
            }

            if (ColumnMapping.ContainsKey(search.SortBy.ToLower()))
            {
                var propertyName = ColumnMapping[search.SortBy.ToLower()];
                query = search.IsSortAsc ? query.OrderBy(c => EF.Property<Character>(c!, propertyName)) : query.OrderByDescending(c => EF.Property<Character>(c!, propertyName));
            }

            var totalCount = query.Count();
            var dbCharacters = await query.Skip((search.Page - 1) * search.PageSize).Take(search.PageSize).ToListAsync();
            var characters = dbCharacters.Select(c => new CharacterGridDto
            {
                Id = c.Id,
                Name = c.Name,
                HitPoints = c.HitPoints,
                Strength = c.Strength,
                Defense = c.Defense,
                Intelligence = c.Intelligence,
                Class = c.Class
            }).ToList();

            return new BaseGridResponse<CharacterGridDto>()
            {
                Items = characters,
                TotalCount = totalCount
            };
        }

        public async Task<ServiceResponse<BaseGridResponse<Object>>> GetAllExpanded(ExpandedGridSearch search)
        {
            var serviceResponse = new ServiceResponse<BaseGridResponse<Object>>();
            var query = _context.Characters.AsQueryable();

            if (!string.IsNullOrEmpty(search.FilterBy))
            {
                query = query.Where(c => c.Name.Contains(search.FilterBy));
            }

            var totalCount = query.Count();

            if (ColumnMapping.ContainsKey(search.SortBy.ToLower()))
            {
                var propertyName = ColumnMapping[search.SortBy.ToLower()];
                query = search.IsSortAsc ? query.OrderBy(c => EF.Property<Character>(c!, propertyName)) : query.OrderByDescending(c => EF.Property<Character>(c!, propertyName));
            }

            var selectedProperties = typeof(Character).GetProperties().Where(prop => search.SelectedColumns.Contains(prop.Name)).ToList();
            var dbCharacters = await query.Skip((search.Page - 1) * search.PageSize).Take(search.PageSize).Select(p => new
            {
                Id = p.Id,
                Name = selectedProperties.Contains(typeof(Character).GetProperty("Name")) ? p.Name : null,
                HitPoints = selectedProperties.Contains(typeof(Character).GetProperty("HitPoints")) ? p.HitPoints : 0,
            }).ToListAsync();

            serviceResponse.Data = new BaseGridResponse<Object>()
            {
                //Items = dbCharacters,
                TotalCount = totalCount
            };
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllFinal()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            var dbCharacters = await _context.Characters.ToListAsync();
            serviceResponse.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            return serviceResponse;
        }
    }
}