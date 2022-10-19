using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System;
using FinalProjectAPI_Pokemon.Models;
using FinalProjectAPI_Pokemon.Dtos;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FinalProjectAPI_Pokemon.Services;

namespace FinalProjectAPI_Pokemon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController : ControllerBase
    {
        #region "Return all Pokémons"
        // Return all pokemons in Database
        [HttpGet]
        [Route("listAll")]
        public async Task<ActionResult<Pokemon>> GetAll()
        {
            using var reader = new StreamReader("./data.json");
            var json = await reader.ReadToEndAsync();
            var pokemons = JsonSerializer.Deserialize<List<Pokemon>>(json);

            return Ok(pokemons);
        }
        #endregion

        #region "Return all Pokémons with pagination"
        // Return all pokemons in Database
        [HttpGet]
        [Route("list")]
        public async Task<ActionResult<IEnumerable<Pokemon>>> GetAllPagination([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                using var reader = new StreamReader("./data.json");
                var json = await reader.ReadToEndAsync();
                var pokemons = JsonSerializer.Deserialize<List<Pokemon>>(json);

                var totalPages = Math.Ceiling((decimal)pokemons.Count / pageSize);

                if (page < 1 || pageSize < 1)
                {
                    return BadRequest("Dados inválidos.");
                }

                if (page * pageSize > totalPages * pageSize)
                {
                    return NotFound("Página não contém elementos!");
                }

                var pokemonsPerPage = pokemons
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Ok(new
                {
                    StatusCode = 200,
                    Message = $"Mostrando página {page}",
                    Meta = new
                    {
                        CurrentPage = page,
                        PageSize = pageSize,
                        TotalPages = totalPages
                    },
                    Data = pokemonsPerPage
                });
            }
            catch
            {
                return BadRequest("Dados inválidos.");
            }
        }
        #endregion

        #region "Pokemon by ID"
        // Return a pokemon by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Pokemon>> GetById(int id)
        {
            using var reader = new StreamReader("./data.json");
            var json = await reader.ReadToEndAsync();
            var pokemons = JsonSerializer.Deserialize<List<Pokemon>>(json);

            var pokemon = pokemons.Where(p => p.Id == id).FirstOrDefault();

            if (pokemon == null)
            {
                return NotFound(new
                {
                    Message = $"Pokémon nº {id} não encontrado!"
                });
            }

            return Ok(pokemon);
        }
        #endregion

        #region "Search Engine"
        //// Return a pokemon by word or letter
        [HttpGet]
        [Route("search")]
        public async Task<ActionResult<IEnumerable<Pokemon>>> SearchPokemon([FromQuery] string term)
        {
            using var reader = new StreamReader("./data.json");
            var json = await reader.ReadToEndAsync();
            var pokemons = JsonSerializer.Deserialize<List<Pokemon>>(json);

            if (string.IsNullOrEmpty(term))
            {
                return BadRequest("Especifique uma palavra ou letra válida!");
            }

            var filteredData = pokemons.Where(x => x.Name.ToUpper().StartsWith(term.ToUpper())).ToList();

            if (filteredData.Count == 0)
            {
                return NotFound(new
                {
                    Message = $"Nenhum Pokémon encontrado!"
                });
            }

            return Ok(filteredData);
        }
        #endregion

        #region "Order Pokémons by Name"
        [HttpGet]
        [Route("orderBy")]
        public async Task<ActionResult<IEnumerable<Pokemon>>> OrderByName()
        {
            using var reader = new StreamReader("./data.json");
            var json = await reader.ReadToEndAsync();
            var pokemons = JsonSerializer.Deserialize<List<Pokemon>>(json);

            var pokemonsOrderByAscending = pokemons.OrderBy(x => x.Name);

            return Ok(pokemonsOrderByAscending);
        }
        #endregion

        #region "Insert Pokémon"
        // Insert a New Pokemon
        [HttpPost]
        public async Task<ActionResult<Pokemon>> CreatePokemon([FromBody] CreatePokemon request)
        {
            using var reader = new StreamReader("./data.json");
            var json = await reader.ReadToEndAsync();
            reader.Dispose();
            var pokemons = JsonSerializer.Deserialize<List<Pokemon>>(json);

            if (pokemons.Select(p => p.Id).Contains(request.Id))
            {
                return BadRequest($"Id #{request.Id} já existe!");
            }

            var pokemonFromAPI = await PokemonService.GetPokemonFromOfficialAPI(request.Id);

            var pokemon = new Pokemon
            {
                Id = request.Id,
                Name = pokemonFromAPI[0].Name,
                Url = pokemonFromAPI[0].Url
            };

            pokemons.Add(pokemon);

            var content = JsonSerializer.Serialize(pokemons);
            System.IO.File.WriteAllText("./data.json", content);

            return Created($"{pokemonFromAPI[0].Url}", pokemon);
        }
        #endregion

        #region "Update Pokémon"
        // Update a pokemon
        [HttpPut]
        public async Task<ActionResult<Pokemon>> UpdatePokemon([FromBody] UpdatePokemon request)
        {
            using var reader = new StreamReader("./data.json");
            var json = await reader.ReadToEndAsync();
            reader.Dispose();
            var pokemons = JsonSerializer.Deserialize<List<Pokemon>>(json);

            var pokemon = pokemons.Find(p => p.Id == request.Id);
            if (pokemon == null)
            {
                return BadRequest($"Pokémon #{request.Id} não encontrado!");
            }

            pokemon.Id = request.Id;
            pokemon.Name = request.Name;

            var content = JsonSerializer.Serialize(pokemons);
            System.IO.File.WriteAllText("./data.json", content);

            return Ok($"Pokémon #{request.Id} - atualizado com sucesso!");
        }
        #endregion

        #region "Delete Pokémon"
        // Delete a pokemon
        [HttpDelete("{id}")]
        public async Task<ActionResult<Pokemon>> Delete(int id)
        {
            using var reader = new StreamReader("./data.json");
            var json = await reader.ReadToEndAsync();
            reader.Dispose();
            var pokemons = JsonSerializer.Deserialize<List<Pokemon>>(json);

            if (!pokemons.Select(p => p.Id).Contains(id))
            {
                return BadRequest($"Id #{id} não existe!");
            }

            pokemons.Remove(pokemons.SingleOrDefault(p => p.Id == id));

            var content = JsonSerializer.Serialize(pokemons);
            System.IO.File.WriteAllText("./data.json", content);

            return Ok($"Pokémon #{id} - deletado sucesso!");
        }
        #endregion
    }
}
