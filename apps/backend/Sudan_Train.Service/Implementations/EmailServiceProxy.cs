using Sudan_Train.Service.Abstracts;
using Sudan_Train.Service.Models;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Sudan_Train.Service.Implementations
{
    /// <summary>
    /// Proxy implementation of IEmailService that forwards email requests to the MessagingApi microservice
    /// </summary>
    public class EmailServiceProxy : IEmailService
    {
        private readonly HttpClient _httpClient;
        private readonly string _messagingApiBaseUrl;

        public EmailServiceProxy(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _messagingApiBaseUrl = configuration["MessagingApi:BaseUrl"]
                ?? throw new InvalidOperationException("MessagingApi:BaseUrl configuration is missing");
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            await SendEmailAsync(email, subject, message, EmailSendingStrategy.Fallback);
        }

        public async Task SendEmailAsync(string email, string subject, string message, EmailSendingStrategy strategy)
        {
            var request = new
            {
                To = email,
                Subject = subject,
                Body = message,
                IsHtml = true,
                Strategy = strategy.ToIntValue() // Send as enum integer value (0=Direct, 1=Queued, 2=Fallback)
            };

            var url = $"{_messagingApiBaseUrl}/api/messaging/email";

            var response = await _httpClient.PostAsJsonAsync(url, request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to send email via MessagingApi. Status: {response.StatusCode}, Error: {errorContent}");
            }
        }
    }
}
