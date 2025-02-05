using System.Collections.Generic;

// Class that's used to store all the movie search cache data using dictionaries

public static class MovieCache
{
    private static Dictionary<string, Dictionary<int, SearchResponse>> searchCache = new Dictionary<string, Dictionary<int, SearchResponse>>();

    // Store search results in cache, based on the query and current page, using them as keys
    public static void AddSearchResult(string query, int page, SearchResponse movies)
    {
        string key = query.ToLowerInvariant();

        if (!searchCache.ContainsKey(key))
        {
            searchCache[key] = new Dictionary<int, SearchResponse>();
        }

        if (!searchCache[key].ContainsKey(page))
        {
            searchCache[key][page] = movies;
        }
    }

    // Tries to retrieve data from search cache, and returns it if its found
    public static bool TryGetSearchResults(string query, int page, out SearchResponse movies)
    {
        string key = query.ToLowerInvariant();

        if (searchCache.ContainsKey(key) && searchCache[key].TryGetValue(page, out movies))
        {
            return true;
        }

        movies = null;
        return false;
    }

    // Clears search cache, not used but essential for larger projects
    public static void ClearCache()
    {
        searchCache.Clear();
    }
}
