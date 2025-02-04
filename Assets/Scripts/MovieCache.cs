using System.Collections.Generic;
using UnityEngine;

public static class MovieCache
{
    private static Dictionary<int, Movie> movieDictionary = new Dictionary<int, Movie>();
    private static Dictionary<string, SearchResponse> searchCache = new Dictionary<string, SearchResponse>();

    // Guardar película en caché
    public static void AddMovie(Movie movie)
    {
        if (!movieDictionary.ContainsKey(movie.id))
        {
            movieDictionary[movie.id] = movie;
        }
    }

    // Intentar obtener película por ID
    public static bool TryGetMovie(int movieId, out Movie movie)
    {
        return movieDictionary.TryGetValue(movieId, out movie);
    }

    // Guardar búsqueda en caché
    public static void AddSearchResult(string query, SearchResponse movies)
    {
        if (!searchCache.ContainsKey(query))
        {
            searchCache[query] = movies;
        }
    }

    // Intentar obtener búsqueda desde caché
    public static bool TryGetSearchResults(string query, out SearchResponse movies)
    {
        return searchCache.TryGetValue(query, out movies);
    }

    //TODO: usar esta función
    public static void ClearCache()
    {
        searchCache.Clear();
        movieDictionary.Clear();
    }
}
