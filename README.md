Movie Search App by Axel Neumann

Download APK (CI/CD): https://github.com/DonRamitas/MovieApp/actions/runs/13152927875/artifacts/2539314880

Setup Instructions:
- Clone this repository to your local machine
- This app was made using Unity 2022.3.24f1, but might work with other versions
- Open project in Unity Hub
- Install package Newtonsoft Json 13.0.102 if it's required

Architecture Overview:
- The app initializes prompting the user a TMDB API key
- If the key is valid, the user can access the movie search
- The movie search screen consists in a manager script which retrieves the API url, the user API key, the entered query and the current page number and calls the TMDB API. The API returns, if the parameters are valid, a list of 20 movies per page.
  A scroll view is filled with the data retrieved. The user can navigate through movie pages using the arrows below, if the user wants to see a specific movie data, they might just press the desired movie.
- The movie display screen consists in another manager scripts, which gets the data of the movie that was pressed and displays it in a responsive, user-friendly way. Information like the title, promotional poster, release year, genres, overview and rating.
  The user can go back just by pressing the device back button or pressing the red back button on the bottom right corner of the screen. For a better control in customization, there are two different movie display screens, one is for portrait orientation and
  the other one is for landscape orientation.
- All app screen interfaces are responsive and have smooth animations between them.
- Every data shown in the app is cached in memory when using the app by a cache system, so the app won't call the API again for an already sent query.

Design Decisions and Trade-Offs:
- It might seem obvious, but I decided to validate the API key entered by the user before accessing the full app, I know it will be tested with valid API keys but just in case
- When I knew that the max results per page the TMDB API could return was 20, I decided to make the app paginated instead of progressively generate the entries, working with just 20 entries or less at a time made development easier
- I thought about using just UI anchors to make the movie display in landscape mode look good, but it never satisfied me. That's why I decided to make a whole new UI screen for landscape mode, it slightly increased the app size and resource consumption
  But I optimized the most I could and I think the benefits are greater than the cons

Known issues/limitations and Future Improvements:
- The max page limit per movie search is 500, it's because of the API response but there's room for improvement
- A better color design would be perfect for the app
- There are better ways to display large texts, in this case I used scroll views, but in the end I don't really like side scrollbars
- The last improvement would be an offline mode, using persistent data path in mobile devices
