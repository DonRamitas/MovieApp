using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MovieDisplayManager : MonoBehaviour
{
    [Header("Referencias")]
    public Image movieImage;
    public TMP_Text movieTitle;
    public TMP_Text movieYear;
    public TMP_Text movieOverview;
    public TMP_Text movieBudget;
    public TMP_Text movieRevenue;
    public GameObject movieDisplayPanelGO;
    public SearchManager searchManager;


    private void SetMovieData(Movie movie){
        movieTitle.text = movie.title;
        searchManager.LoadImage(movie.poster_path,movieImage);
    }

    public void DisplayMovie(Movie movie){
        SetMovieData(movie);
        movieDisplayPanelGO.SetActive(true);
    }

    public void GoBack(){
        movieDisplayPanelGO.SetActive(false);
    }
}
