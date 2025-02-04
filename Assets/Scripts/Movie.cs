using System;
using System.Collections.Generic;

[Serializable]
public class Movie{
    public List<int> genre_ids = new List<int>();
    public int id;
    public string overview; //sinopsis
    public double popularity;
    public string poster_path; //image url
    public string release_date; //release date
    public string title; //titulo
    public double vote_average; //rating
    public int vote_count; //cantidad de gente que votó
}

[Serializable]
public class SearchResponse{
    public int page;
    public List<Movie> results;
    public int total_pages;
    public int total_results;
}

[Serializable]
public class Genre
{
    public int id { get; set; }
    public string name { get; set; }
}

[Serializable]
public class GenreResponse
{
    public List<Genre> genres;
}