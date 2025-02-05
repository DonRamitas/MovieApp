using TMPro;
using UnityEngine;
using UnityEngine.UI;

// This class manages the displaying of movie data

public class MovieDisplayManager : MonoBehaviour
{
    [Header("Main Scripts References")]
    public SearchManager searchManager;
    public ImageLoader imageLoader;
    public PosterAnimation posterAnimation;

    // In order to get custom appearances for both portrait and landscape, I decided to make a screen for each mode
    // And switch them based on device screen ratio
    [Header("Portrait References")]
    public Image movieImage;
    public TMP_Text movieTitle;
    public TMP_Text movieYear;
    public TMP_Text movieOverview;
    public GameObject movieDisplayPanelGO;
    public Transform genreContent;
    public Transform ratingTransform;
    public TMP_Text movieVoteCount;

    [Header("Landscape References")]
    public Image movieImageL;
    public TMP_Text movieTitleL;
    public TMP_Text movieYearL;
    public TMP_Text movieOverviewL;
    public GameObject movieDisplayPanelGOL;
    public Transform genreContentL;
    public Transform ratingTransformL;
    public TMP_Text movieVoteCountL;

    [Header("Resources")]
    public GameObject genreTemplate;
    public GameObject starTemplate;

    // This makes sure all the time that the right screen for the current orientation is being showed
    void Update(){
        if(Screen.height >= Screen.width){
            if(!movieDisplayPanelGO.activeSelf && movieDisplayPanelGOL.activeSelf){
                movieDisplayPanelGO.SetActive(true);
                movieDisplayPanelGOL.SetActive(false);
            }
        }else{
            if(movieDisplayPanelGO.activeSelf && !movieDisplayPanelGOL.activeSelf){
                movieDisplayPanelGOL.SetActive(true);
                movieDisplayPanelGO.SetActive(false);
            }
        }
    }

    // This method sets all the movie data, for both types of screen, portrait and landscape
    private void SetMovieData(Movie movie){

        // Title
        movieTitle.text = movie.title;
        movieTitleL.text = movie.title;

        // Poster
        imageLoader.LoadImage(movie.poster_path,movieImage);
        imageLoader.LoadImage(movie.poster_path,movieImageL);
        
        // Release year
        if(movie.release_date!=null && movie.release_date!=""){
            movieYear.text = movie.release_date.Split('-')[0];
            movieYearL.text = movie.release_date.Split('-')[0];
        }

        // Overview
        if(movie.overview!=null && movie.overview!=""){
            movieOverview.text = movie.overview;
            movieOverviewL.text = movie.overview;
        }

        // Genres
        FlushGenreList();
        if(movie.genre_ids != null && movie.genre_ids.Count>0){
            for(int i = 0;i<movie.genre_ids.Count;i++){
                GameObject genre = Instantiate(genreTemplate,genreContent);
                GameObject genreL = Instantiate(genreTemplate,genreContentL);
                genre.GetComponentInChildren<TMP_Text>().text = searchManager.GenreIdToName(movie.genre_ids[i]);
                genreL.GetComponentInChildren<TMP_Text>().text = searchManager.GenreIdToName(movie.genre_ids[i]);
                genre.SetActive(true);
                genreL.SetActive(true);
            }
        }

        // Rating (represented by five stars)
        double rating = movie.vote_average;
        FlushRating();
        while(rating > 0){
            GameObject star = Instantiate(starTemplate,ratingTransform);
            GameObject starL = Instantiate(starTemplate,ratingTransformL);
            star.SetActive(true);
            starL.SetActive(true);
            rating -= 2;
        }
        movieVoteCount.text = "("+movie.vote_count+" votes)";
        movieVoteCountL.text = "("+movie.vote_count+" votes)";

    }

    // After setting all the movie data, display the right screen playing a smooth animation
    public void DisplayMovie(Movie movie,RectTransform moviePoster){
        SetMovieData(movie);
        posterAnimation.PlayAnimation(moviePoster);

        // Decides between portrait and landscape
        if(Screen.height>=Screen.width){
            movieDisplayPanelGO.SetActive(true);
        }else{
            movieDisplayPanelGOL.SetActive(true);
        }
    }

    // Go back to movie search
    public void GoBack(){
        if(Screen.height>=Screen.width){
            movieDisplayPanelGO.SetActive(false);
        }else{
            movieDisplayPanelGOL.SetActive(false);
        }
    }

    // Flush the genre list of the movie data display
    private void FlushGenreList(){
        foreach (Transform child in genreContent)
        {
            if (child.gameObject != genreTemplate)
            {
                Destroy(child.gameObject);
            }
        }

        foreach (Transform child in genreContentL)
        {
            if (child.gameObject != genreTemplate)
            {
                Destroy(child.gameObject);
            }
        }
    }

    //Flush the stars of the rating of the movie data display
    private void FlushRating(){
        foreach (Transform child in ratingTransform)
        {
            if (child.gameObject != starTemplate)
            {
                Destroy(child.gameObject);
            }
        }
        foreach (Transform child in ratingTransformL)
        {
            if (child.gameObject != starTemplate)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
