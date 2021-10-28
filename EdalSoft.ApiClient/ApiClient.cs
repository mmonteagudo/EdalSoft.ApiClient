using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;

namespace EdalSoft.ApiClient
{
    /// <summary>
    /// Parameter to be added in the request header
    /// </summary>
    public class HeaderParameter
    {
        /// <summary>
        /// Instantiates a Parameter to be added in the request header
        /// </summary>
        /// <param name="key">Unique key for the parameter</param>
        /// <param name="value">Value for the parameter</param>
        public HeaderParameter(string key, string value)
        {
            Key = key;
            Value = value;
        }
        public string Key { get; set; }
        public string Value { get; set; }
    }

    /// <summary>
    /// Parameter to be added in the request body
    /// </summary>
    public class BodyParameter
    {

        /// <summary>
        /// Instantiates a Parameter to be added in the request body
        /// </summary>
        /// <param name="key">Unique key for the parameter</param>
        /// <param name="value">Value for the parameter</param>
        public BodyParameter(string key, string value)
        {
            Key = key;
            Value = value;
        }
        public string Key { get; set; }
        public string Value { get; set; }
    }

    /// <summary>
    /// Token object for JWT Tokens
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Token
    {
        /// <summary>
        /// Gets the Access Token from request
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// Gets the Token Type from the request
        /// </summary>
        public string token_type { get; set; }

        /// <summary>
        /// Gets the Expiration in hours from the request
        /// </summary>
        public int expires_in { get; set; }

        /// <summary>
        /// Gets the refresh token from the request
        /// </summary>
        public string refresh_token { get; set; }
    }

    /// <summary>
    /// API Client library to simplifies the consumption of Http requests based on RestSharp
    /// </summary>
    public class ApiClient
    {
        /// <summary>
        /// Instantiates a new client
        /// </summary>
        public ApiClient() { }
        /// <summary>
        /// Instantiates a new ApiClient using the indicated Base url for requests
        /// </summary>
        /// <param name="apiUrl"></param>
        public ApiClient(string apiUrl) => ApiUrl = apiUrl;
        /// <summary>
        /// API Url assigned to the client when the instance was created
        /// </summary>
        public string ApiUrl { get; }

        public async Task<IRestResponse> DeleteAsync<T>(string resource, T obj, string token = null, HeaderParameter[] headers = null, BodyParameter[] parameters = null)
            => await ExecuteAsync(Method.DELETE, ApiUrl, resource, obj, token, headers, parameters);

        public async Task<IRestResponse> DeleteAsync<T>(string resource, T obj, string token = null, params HeaderParameter[] headers)
            => await ExecuteAsync(Method.DELETE, ApiUrl, resource, obj, token, headers);

        public async Task<IRestResponse> DeleteAsync<T>(string apiUrl, string resource, T obj, string token = null, params HeaderParameter[] headers)
            => await ExecuteAsync(Method.DELETE, apiUrl, resource, obj, token, headers);

        public async Task<IRestResponse> PutAsync<T>(string resource, T obj, string token = null, HeaderParameter[] headers = null, BodyParameter[] parameters = null)
            => await ExecuteAsync(Method.PUT, ApiUrl, resource, obj, token, headers, parameters);

        public async Task<IRestResponse> PutAsync<T>(string resource, T obj, string token = null, params HeaderParameter[] headers)
            => await ExecuteAsync(Method.PUT, ApiUrl, resource, obj, token, headers);

        public async Task<IRestResponse> PutAsync<T>(string apiUrl, string resource, T obj, string token = null, params HeaderParameter[] headers)
            => await ExecuteAsync(Method.PUT, apiUrl, resource, obj, token, headers);

        public async Task<IRestResponse> PostAsync<T>(string resource, T obj, string token = null, HeaderParameter[] headers = null, BodyParameter[] parameters = null)
            => await ExecuteAsync(Method.POST, ApiUrl, resource, obj, token, headers, parameters);

        public async Task<IRestResponse> PostAsync<T>(string resource, T obj, string token = null, params HeaderParameter[] headers)
            => await ExecuteAsync(Method.POST, ApiUrl, resource, obj, token, headers);

        public async Task<IRestResponse> PostAsync<T>(string apiUrl, string resource, T obj, string token = null, params HeaderParameter[] headers)
            => await ExecuteAsync(Method.POST, apiUrl, resource, obj, token, headers);

        public async Task<IRestResponse<T>> GetAsync<T>(string resource, string token = null, HeaderParameter[] headers = null, BodyParameter[] parameters = null)
            => await ExecuteAsync<T>(Method.GET, ApiUrl, resource, token, headers, parameters);

        public async Task<IRestResponse<T>> GetAsync<T>(string resource, string token = null, params HeaderParameter[] headers)
            => await ExecuteAsync<T>(Method.GET, ApiUrl, resource, token, headers);

        public async Task<IRestResponse<T>> GetAsync<T>(string apiUrl, string resource, string token = null, params HeaderParameter[] headers)
            => await ExecuteAsync<T>(Method.GET, apiUrl, resource, token, headers);

        public async Task<IRestResponse<Token>> RequestTokenAsync(string resource, BodyParameter[] bodyParameters)
            => await RequestTokenAsync(ApiUrl, resource, bodyParameters);
        public async Task<IRestResponse<Token>> RequestTokenAsync(string apiUrl, string resource, BodyParameter[] bodyParameters)
        {
            var req = GetRequest(Method.POST,
                resource,
                new[] {
                    new HeaderParameter("Accept", "application/json"),
                    new HeaderParameter("Content-Type", "application/x-www-form-urlencoded")
                },
                bodyParameters);
            return await GetClient(apiUrl)
                .ExecuteAsync<Token>(req)
                .ConfigureAwait(false);
        }

        public async Task<IRestResponse> ExecuteAsync<T>(RestSharp.Method method, string resource, T obj, string token = null, HeaderParameter[] headers = null, BodyParameter[] parameters = null)
            => await ExecuteAsync<T>(method, ApiUrl, resource, obj, token, headers, parameters);

        public async Task<IRestResponse> ExecuteAsync<T>(RestSharp.Method method, string apiUrl, string resource, T obj,
            string token = null, HeaderParameter[] headers = null, BodyParameter[] parameters = null)
        {
            var req = GetRequest(method, resource, headers)
                        .AddJsonBody(obj);

            return await GetClient(apiUrl, token)
                            .ExecuteAsync(req)
                            .ConfigureAwait(false);
        }

        public async Task<IRestResponse<T>> ExecuteAsync<T>(RestSharp.Method method, string apiUrl, string resource, string token = null, HeaderParameter[] headers = null, BodyParameter[] parameters = null)
        {
            var req = GetRequest(method, resource, headers, parameters);

            return await GetClient(apiUrl, token)
                            .ExecuteAsync<T>(req)
                            .ConfigureAwait(false);
        }

        public async Task<IRestResponse<T>> ExecuteAsync<T>(RestSharp.Method method, string resource, string token = null, HeaderParameter[] headers = null, BodyParameter[] parameters = null)
            => await ExecuteAsync<T>(method, ApiUrl, resource, token, headers, parameters);

        private RestClient GetClient(string apiUrl, string token = null)
        {
            var client = new RestClient(apiUrl);

            if (token != null)
                client.Authenticator = new JwtAuthenticator(token);

            client.UseSystemTextJson(new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return client;
        }

        private RestRequest GetRequest(RestSharp.Method method, string resource, HeaderParameter[] headers = null, BodyParameter[] parameters = null)
        {
            var req = new RestRequest(resource, method, DataFormat.Json);
            if (headers != null)
                foreach (var header in headers)
                {
                    req.AddHeader(header.Key, header.Value);
                }

            if (parameters != null)
                foreach (var parameter in parameters)
                {
                    req.AddParameter(parameter.Key, parameter.Value);
                }

            return req;
        }
    }
}