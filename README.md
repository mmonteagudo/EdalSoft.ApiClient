# EdalSoft.ApiClient

Very easy and simple to use .netcore client to consume HTTP Apis in an easy way

Just create the client:
```c#
var apiClient = new EdalSoft.ApiClient.ApiClient(baseUrl);
```

Then starting consuming the API

## OAUTH Token Request
```c#
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
```

## HTTP GET

```c#
var getResponse = await apiClient.GetAsync<UserInfo>("connect/userinfo", accessToken).ConfigureAwait(false);
    if (getResponse.StatusCode == System.Net.HttpStatusCode.OK)
    {
        var userInfo = getResponse.Data;
        Console.WriteLine($"User Data: {userInfo}");
    }
```

## HTTP POST
```c#
 var postResponse = await apiClient.PostAsync<Order>("orders", newOrder, accessToken,
        new HeaderParameter("headerKey", "headerValue"));
    if (postResponse.IsSuccessful)
    {
        Console.WriteLine("Order created");
    }
```
