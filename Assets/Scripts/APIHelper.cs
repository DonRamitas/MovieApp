using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

public class APIHelper
{
    [Header("Constants")]
    private static readonly HttpClient client = new HttpClient();
    private const string API_URL = "https://api.themoviedb.org/3/";
    private const string API_KEY = "666d00beb8a3dc5d87ddf5259ab016b0";

    public static async Task<SearchResponse> SearchMovie(string query, int page = 1)
    {

        if (MovieCache.TryGetSearchResults(query, out SearchResponse cachedResults))
        {
            return cachedResults;
        }

        string formattedQuery = System.Web.HttpUtility.UrlEncode(query);

        string url = API_URL+"search/movie?api_key="+API_KEY+"&query="+formattedQuery+"&page="+page;

        try
        {
            string json = await client.GetStringAsync(url);
            SearchResponse response = JsonConvert.DeserializeObject<SearchResponse>(json);
            MovieCache.AddSearchResult(query,response);
            return response;
        }
        catch (HttpRequestException e)
        {
            //TODO: manejar esto
            Debug.LogError($"Error: {e.Message}");
            return null;
        }
    }

    // Método para obtener los géneros de la API de TMDB
    public static async Task<GenreResponse> GetGenres()
    {
        string url = API_URL+"genre/movie/list?api_key="+API_KEY+"&language=en-US"; // Endpoint de géneros

        try
        {
            string json = await client.GetStringAsync(url);
            return JsonConvert.DeserializeObject<GenreResponse>(json);
        }
        catch (HttpRequestException e)
        {
            //TODO: manejar esto
            Debug.LogError($"Error: {e.Message}");
            return null;
        }
    }
}
