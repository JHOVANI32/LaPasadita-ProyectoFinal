using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace LaPasadita.Services
{
    /// <summary>
    /// Servicio centralizado para conexión con Supabase
    /// Gestiona todas las operaciones HTTP hacia la API REST de Supabase
    /// </summary>
    public interface ISupabaseService
    {
        Task<T?> GetAsync<T>(string table, string? query = null);
        Task<T?> GetByIdAsync<T>(string table, int id);
        Task<T?> PostAsync<T>(string table, object data);
        Task<T?> PatchAsync<T>(string table, int id, object data);
        Task<bool> DeleteAsync(string table, int id);
    }

    public class SupabaseService : ISupabaseService
    {
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl;
        private readonly string _supabaseKey;
        private readonly ILogger<SupabaseService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public SupabaseService(IConfiguration configuration, ILogger<SupabaseService> logger)
        {
            _logger = logger;
            
            // Obtener configuración de Supabase desde appsettings.json
            _supabaseUrl = configuration["Supabase:Url"] 
                ?? throw new ArgumentNullException("Supabase:Url no está configurado");
            _supabaseKey = configuration["Supabase:AnonKey"] 
                ?? throw new ArgumentNullException("Supabase:AnonKey no está configurado");

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri($"{_supabaseUrl}/rest/v1/");
            
            // Headers requeridos por Supabase
            _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", _supabaseKey);
            _httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            };
        }

        /// <summary>
        /// GET - Obtener registros de una tabla
        /// </summary>
        public async Task<T?> GetAsync<T>(string table, string? query = null)
        {
            try
            {
                var endpoint = string.IsNullOrEmpty(query) ? table : $"{table}?{query}";
                _logger.LogInformation("GET: {Endpoint}", endpoint);

                var response = await _httpClient.GetAsync(endpoint);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error en GET: {StatusCode} - {Content}", 
                        response.StatusCode, content);
                    return default;
                }

                return JsonSerializer.Deserialize<T>(content, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción en GetAsync");
                return default;
            }
        }

        /// <summary>
        /// GET por ID - Obtener un registro específico
        /// </summary>
        public async Task<T?> GetByIdAsync<T>(string table, int id)
        {
            try
            {
                var endpoint = $"{table}?id=eq.{id}";
                _logger.LogInformation("GET by ID: {Endpoint}", endpoint);

                var response = await _httpClient.GetAsync(endpoint);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error en GET by ID: {StatusCode}", response.StatusCode);
                    return default;
                }

                var items = JsonSerializer.Deserialize<List<T>>(content, _jsonOptions);
                return items?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción en GetByIdAsync");
                return default;
            }
        }

        /// <summary>
        /// POST - Crear nuevo registro
        /// </summary>
        public async Task<T?> PostAsync<T>(string table, object data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogInformation("POST: {Table} - {Data}", table, json);

                var response = await _httpClient.PostAsync(table, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error en POST: {StatusCode} - {Content}", 
                        response.StatusCode, responseContent);
                    return default;
                }

                var items = JsonSerializer.Deserialize<List<T>>(responseContent, _jsonOptions);
                return items?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción en PostAsync");
                return default;
            }
        }

        /// <summary>
        /// PATCH - Actualizar registro existente
        /// </summary>
        public async Task<T?> PatchAsync<T>(string table, int id, object data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var endpoint = $"{table}?id=eq.{id}";

                _logger.LogInformation("PATCH: {Endpoint} - {Data}", endpoint, json);

                var request = new HttpRequestMessage(HttpMethod.Patch, endpoint)
                {
                    Content = content
                };

                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error en PATCH: {StatusCode} - {Content}", 
                        response.StatusCode, responseContent);
                    return default;
                }

                var items = JsonSerializer.Deserialize<List<T>>(responseContent, _jsonOptions);
                return items?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción en PatchAsync");
                return default;
            }
        }

        /// <summary>
        /// DELETE - Eliminar registro
        /// </summary>
        public async Task<bool> DeleteAsync(string table, int id)
        {
            try
            {
                var endpoint = $"{table}?id=eq.{id}";
                _logger.LogInformation("DELETE: {Endpoint}", endpoint);

                var response = await _httpClient.DeleteAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error en DELETE: {StatusCode} - {Content}", 
                        response.StatusCode, content);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción en DeleteAsync");
                return false;
            }
        }
    }
}
