using NUnit.Framework;
using System.Threading.Tasks;
using UnityEngine;

[TestFixture]
public class APIHelperTests
{
    [Test]
    public async Task SearchMovie_ValidQuery_ReturnsResults()
    {
        // Arrang
        PlayerPrefs.SetString("apiKey", "666d00beb8a3dc5d87ddf5259ab016b0"); // Configura la API Key
        string query = "Inception";

        // Act
        var result = await APIHelper.SearchMovie(query);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotEmpty(result.results);
    }

    [Test]
    public async Task SearchMovie_InvalidQuery_ReturnsEmptyResults()
    {
        // Arrange
        PlayerPrefs.SetString("apiKey", "666d00beb8a3dc5d87ddf5259ab016b0");
        string query = "SomeNonExistentMovieTitle";

        // Act
        var result = await APIHelper.SearchMovie(query);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result.results);
    }

    [Test]
    public async Task SearchMovie_InvalidApiKey_ReturnsNull()
    {
        // Arrange
        PlayerPrefs.SetString("apiKey", "666d00beb8a3dc5d87ddf5259ab016b0");
        string query = "Inception";

        // Act
        var result = await APIHelper.SearchMovie(query);

        // Assert
        Assert.IsNull(result);
    }
}
