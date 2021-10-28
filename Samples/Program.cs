// See https://aka.ms/new-console-template for more information

using EdalSoft.ApiClient;
using Samples;

var baseUrl = "https://localhost:5001"; // Base url for the api

var apiClient = new EdalSoft.ApiClient.ApiClient(baseUrl);

var userName = string.Empty;
var password = string.Empty;
var clientId = string.Empty;
var clientSecret = string.Empty;

var accessToken = string.Empty;
var refreshToken = string.Empty;
var tokenType = string.Empty;
var expiresIn = 0;

// Asking for a token using password grant_type
var req = await apiClient.RequestTokenAsync("connect/token",
                       new[]
                       {
                            new EdalSoft.ApiClient.BodyParameter("grant_type", "password"),
                            new EdalSoft.ApiClient.BodyParameter("username", userName),
                            new EdalSoft.ApiClient.BodyParameter("password", password),
                            new EdalSoft.ApiClient.BodyParameter("scope", "offline_access"),
                            new EdalSoft.ApiClient.BodyParameter("client_id", clientId),
                            new EdalSoft.ApiClient.BodyParameter("client_secret",clientSecret)
                       })
                   .ConfigureAwait(false);

if (req.IsSuccessful)
{
    accessToken = req.Data.access_token;
    refreshToken = req.Data.refresh_token;
    tokenType = req.Data.token_type;
    expiresIn = req.Data.expires_in;

    // Example getting data using token
    var getResponse = await apiClient.GetAsync<UserInfo>("connect/userinfo", accessToken).ConfigureAwait(false);
    if (getResponse.StatusCode == System.Net.HttpStatusCode.OK)
    {
        var userInfo = getResponse.Data;
        Console.WriteLine($"User Data: {userInfo}");
    }

    // Example posting data using token
    var newOrder = new Order
    {
        Id = Guid.NewGuid(),
        Created = DateTime.Now,
        CustomerName = "Test Customer"
    };

    var postResponse = await apiClient.PostAsync<Order>("orders", newOrder, accessToken,
        new HeaderParameter("headerKey", "headerValue"));
    if (postResponse.IsSuccessful)
    {
        Console.WriteLine("Order created");
    }
}