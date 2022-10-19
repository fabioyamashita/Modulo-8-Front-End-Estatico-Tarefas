using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace FinalProjectConsoleApp
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:44332/api/Pokemon/");

            bool endApp = false;
            while (!endApp)
            {
                Console.Clear();
                Console.WriteLine("Seja bem-vindo(a) aos serviços da Pokédex!");
                Console.WriteLine("Escolha uma das opções:");
                Console.WriteLine("1 - Listar todos os Pokémons");
                Console.WriteLine("2 - Listar todos os Pokémons com paginação");
                Console.WriteLine("3 - Procurar um Pokémon pelo ID");
                Console.WriteLine("4 - Procurar Pokémons pelo mecanismo de busca");
                Console.WriteLine("5 - Ordenar todos os Pokémons pelo nome");
                Console.WriteLine("6 - Inserir um novo Pokémon");
                Console.WriteLine("7 - Atualizar um Pokémon existente");
                Console.WriteLine("8 - Deletar um Pokémon");
                Console.WriteLine("9 - Sair");

                Console.Write("\nSelecione uma das opções: ");
                switch (Console.ReadLine())
                {
                    case "1":
                        await PokemonServices.ShowAllPokemons(httpClient);
                        break;

                    case "2":
                        Console.Write("Digite quantos Pokémons você quer ver por página: ");
                        var currentPage = 1;
                        int.TryParse(Console.ReadLine(), out int pokemonsPerPage);
                        await PokemonServices.ShowAllPokemonsWithPagination(httpClient, pokemonsPerPage, currentPage);

                        bool endPagination = false;

                        while(!endPagination)
                        {
                            Console.WriteLine("\n1 - Página anterior");
                            Console.WriteLine("2 - Próxima página");
                            Console.WriteLine("3 - Sair");

                            switch(Console.ReadLine())
                            {
                                case "1":
                                    currentPage--;
                                    await PokemonServices.ShowAllPokemonsWithPagination(httpClient, pokemonsPerPage, currentPage);
                                    break;
                                case "2":
                                    currentPage++;
                                    await PokemonServices.ShowAllPokemonsWithPagination(httpClient, pokemonsPerPage, currentPage);
                                    break;
                                case "3":
                                    endPagination = true;
                                    break;
                            }
                        }
                        break;

                    case "3":
                        Console.Write("Digite o ID procurado: ");
                        string id = Console.ReadLine();
                        await PokemonServices.ShowPokemonById(httpClient, id);
                        break;

                    case "4":
                        Console.Write("Digite uma letra ou palavra: ");
                        string term = Console.ReadLine();
                        await PokemonServices.ShowPokemonSearchResult(httpClient, term);
                        break;

                    case "5":
                        await PokemonServices.ShowAllPokemonsByAlphabeticalOrder(httpClient);
                        break;

                    case "6":
                        Console.Write("Digite o ID do Pokémon que você deseja inserir: ");
                        string idInserted = Console.ReadLine();
                        await PokemonServices.ShowInsertedPokemonResult(httpClient, idInserted);
                        break;

                    case "7":
                        Console.Write("Digite o ID do Pokémon que você deseja modificar: ");
                        string idUpdated = Console.ReadLine();
                        Console.Write("Digite o nome do Pokémon atualizado: ");
                        string nameUpdated = Console.ReadLine();
                        await PokemonServices.ShowUpdatedPokemonResult(httpClient, idUpdated, nameUpdated);
                        break;

                    case "8":
                        Console.Write("Digite o ID do Pokémon que você deseja deletar: ");
                        string idDeleted = Console.ReadLine();
                        await PokemonServices.ShowDeletedPokemonResult(httpClient, idDeleted);
                        break;

                    case "9":
                        endApp = true;
                        break;
                }
                Console.Write("\nDIGITE QUALQUER TECLA PARA CONTINUAR...");
                Console.ReadKey();
            }
        }
    }
}