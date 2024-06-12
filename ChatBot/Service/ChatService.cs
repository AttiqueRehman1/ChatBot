using Microsoft.AspNetCore.Components.Forms;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ChatBot.Service
{
    public class ChatService
    {
        private readonly IConfiguration _config;
        private static readonly HttpClient httpClient = new HttpClient();
        string URLIP = "https://ace9-34-125-64-203.ngrok-free.app";

        public ChatService(IConfiguration configuration)
        {
            _config = configuration;
        }
        /*public async Task<ApiResponse> ChatApiCall(IBrowserFile file, string message)
        {
            try
            {
                var url = "https://b892-35-245-72-2.ngrok-free.app/process-request"; // Replace with the correct endpoint
                var content = new MultipartFormDataContent();

                if (file != null)
                {
                    var fileContent = new StreamContent(file.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                    content.Add(fileContent, "file", file.Name);
                }

                if (!string.IsNullOrWhiteSpace(message))
                {
                   // content.Add(new StringContent(message), "question");
                    content.Add(new StringContent(message), "prompt");
                }
                var response = await httpClient.PostAsync(url, content);

                var responseString = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response: {responseString}"); // Debug: Log the full response

                if (response.IsSuccessStatusCode)
                {
                    //var responseContent = JsonSerializer.Deserialize<ApiResponse>(responseString);
                    if (responseString != null && !string.IsNullOrEmpty(responseString))
                    {
                        return new ApiResponse { StatusCode = response.StatusCode, IsSuccessStatusCode = true, Response = responseString };
                    }
                    else
                    {
                        return new ApiResponse { StatusCode = response.StatusCode, IsSuccessStatusCode = true, Response = "Information Not Found !" };
                    }
                }
                else
                {
                    return new ApiResponse { StatusCode = response.StatusCode, IsSuccessStatusCode = false, ErrorMessage = responseString };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse { StatusCode = HttpStatusCode.BadRequest, IsSuccessStatusCode = false, ErrorMessage = ex.Message };
            }
        }
*/
        public async Task<ApiResponse> ChatApiCall(IBrowserFile file, string message)
        {
            try
            {
                var url = $"{URLIP}/process-document";
                var content = new MultipartFormDataContent();

                if (file != null)
                {
                    var fileContent = new StreamContent(file.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                    content.Add(fileContent, "file", file.Name);
                }

                if (!string.IsNullOrWhiteSpace(message))
                {
                    content.Add(new StringContent(message), "prompt");
                }

                var response = await httpClient.PostAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response: {responseString}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = JsonDocument.Parse(responseString);
                    if (jsonResponse.RootElement.TryGetProperty("response", out var responseElement) && responseElement.GetString() != null)
                    {
                        return new ApiResponse { StatusCode = response.StatusCode, IsSuccessStatusCode = true, Response = responseElement.GetString() };
                    }
                    else
                    {
                        string responseBody = jsonResponse.RootElement.ToString();
                        return new ApiResponse { StatusCode = response.StatusCode, IsSuccessStatusCode = true, Response = responseBody };
                    }
                }
                else
                {
                    return new ApiResponse { StatusCode = response.StatusCode, IsSuccessStatusCode = false, ErrorMessage = responseString };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse { StatusCode = HttpStatusCode.BadRequest, IsSuccessStatusCode = false, ErrorMessage = ex.Message };
            }
        }




        public async Task<ApiResponse> ChatApiCallMessage(IBrowserFile file, string Message)
        {
            try
            {

                var url = $"{URLIP}/process-query";
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                var postBody = new ApiResponse
                {
                    prompt = Message
                };
                request.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(postBody), Encoding.UTF8, "application/json");
                var response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadFromJsonAsync<ApiResponse>();
                    if (responseContent != null && !string.IsNullOrEmpty(responseContent.Response))
                    {
                        return new ApiResponse { StatusCode = response.StatusCode, IsSuccessStatusCode = true, Response = responseContent.Response };
                    }
                    else
                    {
                        return new ApiResponse { StatusCode = response.StatusCode, IsSuccessStatusCode = true, Response = "Information Not Found !" };
                    }
                }
                else
                {
                    var content = await response.Content.ReadFromJsonAsync<ApiResponse>();
                    return new ApiResponse { StatusCode = response.StatusCode, IsSuccessStatusCode = response.IsSuccessStatusCode };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse { StatusCode = HttpStatusCode.BadRequest, IsSuccessStatusCode = false, ErrorMessage = ex.Message };
            }
        }
    }
    public class ApiResponse
    {
        public string? prompt { get; set; }
        public string Response { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccessStatusCode { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
