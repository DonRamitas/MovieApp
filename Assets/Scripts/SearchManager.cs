using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// This class manages all the movie search logic

public class SearchManager : MonoBehaviour
{
    [Header("Main Scripts References")]
    public MovieDisplayManager movieDisplayManager;
    public ImageLoader imageLoader;

    [Header("References")]
    public TMP_InputField searchBarText;
    public Transform contentTransform;
    public TMP_Text pageCounter;
    public Button previousButton;
    public Button nextButton;
    public ScrollRect movieListScroll;
    public GameObject startMessageGO;
    public GameObject notFoundMessageGO;
    public Image fadeInImage;
    public GameObject errorMessageGO;

    [Header("Resources")]
    public GameObject movieTemplate;
    public GameObject starTemplate;

    [Header("Variables")]
    private int currentPage = 1;
    private bool isLoading = false;
    private int currentPageCount = 1;
    private GenreResponse genreList; // I decided to save the genre list from the beginning

    void Awake(){
        GetGenres();
    }

    void Start(){
        // Allows the user to search for a movie by pressing the Enter button too
        searchBarText.onEndEdit.AddListener(OnInputSubmit);
        searchBarText.onSubmit.AddListener(OnInputSubmit);

        // Makes sure to start in alpha 1, and the plays a fade in
        fadeInImage.CrossFadeAlpha(1f,0,false);
        fadeInImage.CrossFadeAlpha(0f,1f,false);
    }

    // This makes sure to enable/disable page navigation buttons based on available pages
    void Update(){
        if(currentPage == 1){
            if(previousButton.interactable==true){
                previousButton.interactable = false;
            }
        }else{
            if(previousButton.interactable == false){
                previousButton.interactable = true;
            }
        }

        if(currentPage == currentPageCount){
            if(nextButton.interactable==true){
                nextButton.interactable = false;
            }
        }else{
            if(nextButton.interactable == false){
                nextButton.interactable = true;
            }
        }

    }

    // Allows the user to search for a movie by pressing the Enter button too
    private void OnInputSubmit(string input)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            LoadPage();
        }
    }

    // Get the genres list from the API
    private async void GetGenres(){
        genreList = await APIHelper.GetGenres();
    }

    // This method loads the movie search page based on the entered query
    public async void LoadPage(bool isFirstTime = true){

        string input = searchBarText.text.Trim();

        // If there's nothing written on the input field, do nothing
        if(input==null && input==""){
            return;
        }else{

            // If the method parameter doesn't indicates otherwise, it means that the page 1 must be loaded
            if(isFirstTime){
                currentPage = 1;
            }

            // Flushes the current movie list and loads another based on user query
            FlushMovieList();
            SearchResponse response = await APIHelper.SearchMovie(input,currentPage);
            // Populates the movie list after retrieving the results
            // The null handling is inside the PopulateMovieList method
            PopulateMovieList(response);
        }
    }

    // Loads next page if the movie list is not loading
    public void NextPage(){
        if(currentPage==currentPageCount || isLoading) return;
        currentPage++;
        LoadPage(false);
    }

    // Loads previous page if the movie list is not loading
    public void PreviousPage(){
        if(currentPage==1 || isLoading) return;
        currentPage--;
        LoadPage(false);
    }

    // Flushes the movie list of the recent movie search
    private void FlushMovieList(){
        movieListScroll.verticalNormalizedPosition = 1;
        isLoading = true;
        pageCounter.text = "--/--";
        foreach (Transform child in contentTransform)
        {
            if (child.gameObject != movieTemplate)
            {
                Destroy(child.gameObject);
            }
        }
    }

    // This method takes the movie search response and populates the scroll view based on the response validity
    private void PopulateMovieList(SearchResponse response){

        // If response is null, show an error message
        if(response == null){
            errorMessageGO.SetActive(true);
        }else{
            errorMessageGO.SetActive(false);
        }

        // Disables the welcome message
        startMessageGO.SetActive(false);

        // If the query worked, but there was no results, show a different message
        if(response.total_results==0){
            notFoundMessageGO.SetActive(true);
        }else{
            notFoundMessageGO.SetActive(false);
        }

        // Set page navigation data
        currentPageCount = response.total_pages;
        pageCounter.text = currentPage+"/"+currentPageCount;

        isLoading = false;

        // For every result got, instantiate a movie entry with its data
        for(int i = 0; i < response.results.Count-1;i++){

            // Get the movie data
            Movie movie = response.results[i];

            // Instantiate a movie entry, and fill it with its data, poster, title, year, genres, rating and vote count
            GameObject movieGO = Instantiate(movieTemplate,contentTransform);

            Transform movieTransform = movieGO.transform;

            Image moviePoster = movieTransform.GetChild(0).GetComponent<Image>();
            TMP_Text movieTitle = movieTransform.GetChild(1).GetComponent<TMP_Text>();
            TMP_Text movieYearAndGenre = movieTransform.GetChild(2).GetComponent<TMP_Text>();
            Transform ratingTransform = movieTransform.GetChild(3);
            TMP_Text voteCount = ratingTransform.GetChild(0).GetComponent<TMP_Text>();

            imageLoader.LoadImage(movie.poster_path,moviePoster); //poster
            movieTitle.text = movie.title; //title

            if(movie.release_date!=null && movie.release_date!=""){
                movieYearAndGenre.text = movie.release_date.Split('-')[0];

                // If the movie has a valid release date and genres, write them separated by a "-"
                if(movie.genre_ids.Count>0){
                    movieYearAndGenre.text += " - ";
                }
            }

            // Show the first two genres of the movie (Because they could be 20 max)
            if(movie.genre_ids.Count>0){
                movieYearAndGenre.text += GenreIdToName(movie.genre_ids[0]); //year and genres
            }

            if(movie.genre_ids.Count>1){
                movieYearAndGenre.text += ", "+GenreIdToName(movie.genre_ids[1]);
            }

            // Show rating from 1 to 5 stars, and its vote count
            double rating = movie.vote_average;
            while(rating > 0){
                GameObject star = Instantiate(starTemplate,ratingTransform);
                star.SetActive(true);
                rating -= 2;
            }
            voteCount.text = "("+movie.vote_count+" votes)";
            voteCount.transform.SetAsLastSibling();

            // Set the movie entry functionality; click it and see its data
            movieGO.GetComponent<Button>().onClick.RemoveAllListeners();
            movieGO.GetComponent<Button>().onClick.AddListener(() => movieDisplayManager.DisplayMovie(movie,moviePoster.GetComponent<RectTransform>()));

            movieGO.SetActive(true);
        }
    }

    // A simple method that converts a genre id into its name
    public string GenreIdToName(int id){
        Genre genre = genreList.genres.FirstOrDefault(g => g.id == id);
        return genre.name;
    }
}
