using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.JSInterop;
using ReproIVF.Shared.Auth;
using ReproIVF.Shared.Security;

namespace ReproIVF.Client.Services;

public class AuthService
{
    private const string StorageKey = "reproivf.auth";

    private readonly ApiClient _api;
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;

    public bool IsInitialized { get; private set; }
    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(Token);
    public bool IsAdmin => string.Equals(Role, AppRoles.Admin, StringComparison.OrdinalIgnoreCase);
    public bool IsClient => string.Equals(Role, AppRoles.Client, StringComparison.OrdinalIgnoreCase);

    public string? Token { get; private set; }
    public string? Username { get; private set; }
    public string? Role { get; private set; }

    public event Action? Changed;

    public AuthService(ApiClient api, HttpClient http, IJSRuntime js)
    {
        _api = api;
        _http = http;
        _js = js;
    }

    public async Task InitializeAsync()
    {
        if (IsInitialized)
        {
            return;
        }

        try
        {
            var raw = await _js.InvokeAsync<string?>("localStorage.getItem", StorageKey);
            if (!string.IsNullOrWhiteSpace(raw))
            {
                var payload = JsonSerializer.Deserialize<LoginResponse>(raw);
                if (payload is not null)
                {
                    ApplySession(payload);
                }
            }
        }
        catch
        {
            // Ignore local storage errors and keep anonymous state.
        }

        IsInitialized = true;
        Changed?.Invoke();
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var response = await _api.LoginAsync(new LoginRequest
        {
            Username = username,
            Password = password
        });

        if (response is null)
        {
            return false;
        }

        ApplySession(response);
        await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, JsonSerializer.Serialize(response));
        Changed?.Invoke();
        return true;
    }

    public async Task LogoutAsync()
    {
        ClearSession();
        await _js.InvokeVoidAsync("localStorage.removeItem", StorageKey);
        Changed?.Invoke();
    }

    public bool CanAccessPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || path == "/")
        {
            return IsAuthenticated;
        }

        if (path.Equals("/login", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (!IsAuthenticated)
        {
            return false;
        }

        if (IsAdmin)
        {
            return true;
        }

        return IsClient && path.Equals("/", StringComparison.OrdinalIgnoreCase);
    }

    private void ApplySession(LoginResponse response)
    {
        Token = response.Token;
        Username = response.Username;
        Role = response.Role;
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", response.Token);
    }

    private void ClearSession()
    {
        Token = null;
        Username = null;
        Role = null;
        _http.DefaultRequestHeaders.Authorization = null;
    }
}
