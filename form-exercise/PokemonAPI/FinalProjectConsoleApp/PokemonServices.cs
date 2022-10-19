using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;

namespace FinalProjectConsoleApp
{
    public static class PokemonServices
    {
        public static async Task ShowAllPokemons(HttpClient httpClient)
        {
            var response = httpClient.GetAsync("listAll").Result;

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<JsonObject>>(content);

            Console.WriteLine("\nAqui está a lista de todos os pokémons!");
            foreach (var pokemon in result)
            {
                Console.WriteLine("#" + pokemon["id"] + " - " + pokemon["name"].ToString().ToUpper());
            }
        }

        public static async Task ShowAllPokemonsWithPagination(HttpClient httpClient, int pageSize, int currentPage = 1)
        {
            var response = httpClient.GetAsync($"list?page={currentPage}&pageSize={pageSize}").Result;

            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var pokemons = JsonSerializer.Deserialize<JsonObject>(content);
                Console.WriteLine("\nAqui está a lista de todos os pokémons!");
                Console.WriteLine("Página " + pokemons["meta"]["currentPage"] + " de " + pokemons["meta"]["totalPages"]);

                var pokemonsPerPageJson = pokemons["data"].ToJsonString();
                var pokemonsPerPage = JsonSerializer.Deserialize<List<JsonObject>>(pokemonsPerPageJson);

                foreach (var pokemon in pokemonsPerPage)
                {
                    Console.WriteLine("#" + pokemon["id"] + " - " + pokemon["name"].ToString().ToUpper());
                }
            }
            else
            {
                Console.WriteLine("\n" + content);
            }
        }

        public static async Task ShowPokemonById(HttpClient httpClient, string id)
        {
            var response = httpClient.GetAsync(id).Result;

            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var pokemons = JsonSerializer.Deserialize<JsonObject>(content);
                Console.WriteLine("\nPokémon encontrado!");
                Console.WriteLine("#" + pokemons["id"] + " - " + pokemons["name"].ToString().ToUpper());
            }
            else
            {
                Console.WriteLine("\n" + JsonSerializer.Deserialize<JsonObject>(content)["message"]);
            }
        }

        public static async Task ShowPokemonSearchResult(HttpClient httpClient, string term)
        {
            var response = httpClient.GetAsync($"search?term={term}").Result;

            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var pokemons = JsonSerializer.Deserialize<List<JsonObject>>(content);
                Console.WriteLine($"\nAqui está o seu resultado da busca por '{term}'");
                foreach (var pokemon in pokemons)
                {
                    Console.WriteLine("#" + pokemon["id"] + " - " + pokemon["name"].ToString().ToUpper());
                }
            }
            else
            {
                Console.WriteLine("\n" + JsonSerializer.Deserialize<JsonObject>(content)["message"]);
            }
        }

        public static async Task ShowAllPokemonsByAlphabeticalOrder(HttpClient httpClient)
        {
            var response = httpClient.GetAsync("orderBy").Result;

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<JsonObject>>(content);

            Console.WriteLine("\nAqui está a lista de todos os pokémons em ordem alfabética!");
            foreach (var pokemon in result)
            {
                Console.WriteLine("#" + pokemon["id"] + " - " + pokemon["name"].ToString().ToUpper());
            }
        }

        public static async Task ShowInsertedPokemonResult(HttpClient httpClient, string id)
        {
            try
            {
                var body = new
                {
                    Id = int.Parse(id),
                };

                var json = JsonSerializer.Serialize(body);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{httpClient.BaseAddress}", data);

                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var pokemons = JsonSerializer.Deserialize<JsonObject>(content);
                    Console.Write("\nPokémon " + "#" + pokemons["id"] + " - " + pokemons["name"].ToString().ToUpper());
                    Console.Write(" adicionado com sucesso!");
                }
                else
                {
                    Console.WriteLine("\n" + content);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
            }
        }

        public static async Task ShowUpdatedPokemonResult(HttpClient httpClient, string id, string name)
        {
            try
            {
                var body = new
                {
                    Id = int.Parse(id),
                    Name = name
                };

                var json = JsonSerializer.Serialize(body);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PutAsync($"{httpClient.BaseAddress}", data);

                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("\n" + content);
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
            }
        }

        public static async Task ShowDeletedPokemonResult(HttpClient httpClient, string id)
        {
            try
            {
                var response = httpClient.DeleteAsync(id).Result;

                var content = await response.Content.ReadAsStringAsync();

                Console.WriteLine("\n" + content);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
