using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.External;
using Application.Models;
using Application.Models.External;

namespace Infrastructure.Services.External
{
    public class JokeService : IJokeService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const string JokeApiClientName = "JokeApiClient";

        public JokeService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<JokeDto> GetRandomJokeAsync()
        {
            var httpClient = _httpClientFactory.CreateClient(JokeApiClientName);

            try
            {
                var joke = await httpClient.GetFromJsonAsync<JokeDto>("random_joke");

                if (joke == null)
                {
                    throw new ApplicationException("La API devolvio una respuesta vacia.");
                }

                return joke;
            }
            catch (HttpRequestException ex)
            {
                throw new ApplicationException("Error al conectar con la API de chistes", ex);
            }
        }
    }
}
