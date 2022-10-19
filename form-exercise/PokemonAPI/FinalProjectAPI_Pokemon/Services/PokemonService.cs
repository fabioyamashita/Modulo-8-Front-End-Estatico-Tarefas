using FinalProjectAPI_Pokemon.Models;
using System.Text.Json.Nodes;
using System.Text.Json;

namespace FinalProjectAPI_Pokemon.Services
{
    public static class PokemonService
    {
        public static async Task<List<Pokemon>> GetPokemonFromOfficialAPI(int id)
        {
            var httpClient = new HttpClient();

            try
            {
                var url = $"https://pokeapi.co/api/v2/pokemon/?offset={id - 1}&limit=1";
                var response = await httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                var pokemonObject = JsonSerializer.Deserialize<JsonObject>(content)["results"].ToJsonString();

                var pokemon = JsonSerializer.Deserialize<List<Pokemon>>(pokemonObject);

                return pokemon;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                throw;
            }
        }
    }
}
