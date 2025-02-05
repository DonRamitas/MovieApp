using System;
using System.Collections.Generic;

// This classes are used for API data retrieving, self explanatory

// Attributes of a movie entity
[Serializable]
public class Movie{
    public List<int> genre_ids = new List<int>();
    public int id;
    public string overview;
    public string poster_path;
    public string release_date;
    public string title;
    public double vote_average;
    public int vote_count;
}

// Attributes of a movie search, includes the list of movies (results), current page, the total pages and results
[Serializable]
public class SearchResponse{
    public int page;
    public List<Movie> results;
    public int total_pages;
    public int total_results;
}

// The TMDB API stored genres with numbers as id, I needed this class to convert those ids into the genres names
[Serializable]
public class Genre
{
    public int id { get; set; }
    public string name { get; set; }
}

// The list of retrieved genres
[Serializable]
public class GenreResponse
{
    public List<Genre> genres;
}