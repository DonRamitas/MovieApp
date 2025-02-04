using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SearchManager : MonoBehaviour
{
    [Header("References")]
    public TMP_InputField searchBarText;
    public Transform contentTransform;
    public TMP_Text pageCounter;
    public Button previousButton;
    public Button nextButton;
    public ScrollRect movieListScroll;
    public MovieDisplayManager movieDisplayManager;

    [Header("Resources")]
    public GameObject movieTemplate;
    public Sprite noImage;
    private GenreResponse genreList;
    public GameObject starTemplate;

    [Header("Variables")]
    private int currentPage = 1;
    private bool isLoading = false;
    private int currentPageCount = 1;

    void Awake(){
        GetGenres();
    }

    private async void GetGenres(){
        genreList = await APIHelper.GetGenres();
        Debug.Log(genreList.ToString());
    }

    public async void LoadPage(bool isFirstTime = true){

        string input = searchBarText.text.Trim();

        if(input == "" || input == null){
            //TODO: mostrar mensaje para que ingrese algo
            return;
        }else{

            if(isFirstTime){
                currentPage = 1;
            }

            FlushMovieList();

            SearchResponse response = await APIHelper.SearchMovie(input,currentPage);

            PopulateMovieList(response);
        }
    }

    public void NextPage(){
        if(currentPage==currentPageCount || isLoading) return;
        currentPage++;
        LoadPage(false);
    }

    public void PreviousPage(){
        if(currentPage==1 || isLoading) return;
        currentPage--;
        LoadPage(false);
    }

    private void FlushMovieList(){
        movieListScroll.verticalNormalizedPosition = 1;
        isLoading = true;
        pageCounter.text = "-/-";
        foreach (Transform child in contentTransform)
        {
            if (child.gameObject != movieTemplate)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void PopulateMovieList(SearchResponse response){

        currentPageCount = response.total_pages;

        pageCounter.text = currentPage+"/"+currentPageCount;

        isLoading = false;

        for(int i = 0; i < response.results.Count-1;i++){

            Movie movie = response.results[i];

            GameObject movieGO = Instantiate(movieTemplate,contentTransform);

            Transform movieTransform = movieGO.transform;

            Image moviePoster = movieTransform.GetChild(0).GetComponent<Image>();
            TMP_Text movieTitle = movieTransform.GetChild(1).GetComponent<TMP_Text>();
            TMP_Text movieYearAndGenre = movieTransform.GetChild(2).GetComponent<TMP_Text>();
            Transform ratingTransform = movieTransform.GetChild(3);
            TMP_Text voteCount = ratingTransform.GetChild(0).GetComponent<TMP_Text>();

            //TODO: manejar cuando falta algun atributo, por ejemplo el release date o vote average

            LoadImage(movie.poster_path,moviePoster); //poster
            movieTitle.text = movie.title; //title

            if(movie.release_date!=null && movie.release_date!=""){
                movieYearAndGenre.text = movie.release_date.Split('-')[0] +" - ";
            }

            if(movie.genre_ids.Count>0){
                movieYearAndGenre.text += GenreIdToName(movie.genre_ids[0]); //year and genres
            }

            if(movie.genre_ids.Count>1){
                movieYearAndGenre.text += ", "+GenreIdToName(movie.genre_ids[1]);
            }

            double rating = movie.vote_average;

            while(rating > 0){
                GameObject star = Instantiate(starTemplate,ratingTransform);
                star.SetActive(true);
                rating -= 2;
            }

            voteCount.text = "("+movie.vote_count+" votes)";
            voteCount.transform.SetAsLastSibling();

            movieGO.GetComponent<Button>().onClick.RemoveAllListeners();
            movieGO.GetComponent<Button>().onClick.AddListener(() => movieDisplayManager.DisplayMovie(movie));

            movieGO.SetActive(true);
        }
    }

    string GenreIdToName(int id){
        Genre genre = genreList.genres.FirstOrDefault(g => g.id == id);
        return genre.name;
    }

    public void LoadImage(string path, Image targetImage){
        StartCoroutine(LoadImageCoroutine(path,targetImage));
    }

    IEnumerator LoadImageCoroutine(string path, Image targetImage)
    {

        GameObject loadingGO = targetImage.transform.GetChild(0).gameObject;
        loadingGO.SetActive(true);

        targetImage.sprite = null;

        const string IMAGE_PATH = "https://image.tmdb.org/t/p/w500/";
        string fullUrl = IMAGE_PATH+path;
        // Realiza la solicitud para obtener la imagen desde la URL
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(fullUrl);
        yield return request.SendWebRequest();  // Espera hasta que se complete la solicitud

        if (request.result == UnityWebRequest.Result.Success && targetImage != null)
        {
            // Si la solicitud fue exitosa, asigna la textura al RawImage
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            targetImage.sprite = sprite;
            targetImage.preserveAspect = true;
        }
        else
        {
            // Si hubo un error al cargar la imagen, muestra el error
            if(targetImage != null){
                targetImage.sprite = noImage;
            }
            //Debug.LogError("Error al cargar la imagen: " + request.error);
        }
        if(loadingGO!=null){
            loadingGO.SetActive(false);
        }
    }
}
