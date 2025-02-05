using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using System;
using UnityEngine.SceneManagement;

// This is the class that manager all API logic

public class APIHelper
{
    [Header("Constants")]
    private static readonly HttpClient client = new HttpClient(); // I use HttpClient for simple json retrieving
    private const string API_URL = "https://api.themoviedb.org/3/";

    // Method that returns the list of movies based on the search query and pagination
    public static async Task<SearchResponse> SearchMovie(string query, int page = 1)
    {
        // In every method, I get the API key from PlayerPrefs
        string API_KEY = PlayerPrefs.GetString("apiKey");

        // Detects if there exists a cached result for the specified query/page
        if (MovieCache.TryGetSearchResults(query, page, out SearchResponse cachedResults))
        {
            return cachedResults;
        }

        // I make sure that the query format is compatible with url encoding
        string formattedQuery = System.Web.HttpUtility.UrlEncode(query);

        string url = API_URL+"search/movie?api_key="+API_KEY+"&query="+formattedQuery+"&page="+page;

        // API call, try to get data and deserialize if found
        try
        {
            string json = await client.GetStringAsync(url);
            SearchResponse response = JsonConvert.DeserializeObject<SearchResponse>(json);

            // Store in cache the result, using its query and page
            MovieCache.AddSearchResult(query,page,response);

            // Return the list of movies
            return response;
        }
        catch (HttpRequestException)
        {
            //
            return null;
        }
    }

    // Method that I use to get the genres used by TMDB
    public static async Task<GenreResponse> GetGenres()
    {
        // Everything is the same as the Search Movie method, but in this case, we're looking for the genre list
        string API_KEY = PlayerPrefs.GetString("apiKey");

        string url = API_URL+"genre/movie/list?api_key="+API_KEY+"&language=en-US"; // Endpoint de géneros

        try
        {
            string json = await client.GetStringAsync(url);
            return JsonConvert.DeserializeObject<GenreResponse>(json);
        }
        catch (HttpRequestException)
        {
            // A null response from this method should never happen, because this method is called right after verifying the entered API key in the first scene
            // That's why I decided to get the user back to the first scene, to validate their API key again
            SceneManager.LoadScene("Init");
            return null;
        }
    }

    // Method that returns true or false depending if the API key entered by the user is valid, it is used to decide user access granting
    public static async Task<bool> TestAPIKey(string apiKey)
    {
        string url = API_URL+"configuration?api_key="+apiKey;

        try
        {
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception)
        {
            return false;
        }
    }
}
