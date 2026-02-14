using System.Net.Http.Json;
using ReproIVF.Shared.Auth;
using ReproIVF.Client.Models;
using ReproIVF.Shared.Entities;
using ProgramEntity = ReproIVF.Shared.Entities.Program;
using ClientEntity = ReproIVF.Shared.Entities.Client;

namespace ReproIVF.Client.Services;

public class ApiClient
{
    private readonly HttpClient _http;

    public ApiClient(HttpClient http)
    {
        _http = http;
    }

    public Task<List<ProgramEntity>> GetProgramsAsync() =>
        _http.GetFromJsonAsync<List<ProgramEntity>>("api/lookups/programs")!;

    public Task<List<ClientEntity>> GetClientsAsync() =>
        _http.GetFromJsonAsync<List<ClientEntity>>("api/lookups/clients")!;

    public Task<List<Donor>> GetDonorsAsync() =>
        _http.GetFromJsonAsync<List<Donor>>("api/lookups/donors")!;

    public Task<List<Bull>> GetBullsAsync() =>
        _http.GetFromJsonAsync<List<Bull>>("api/lookups/bulls")!;

    public Task<List<Technician>> GetTechniciansAsync() =>
        _http.GetFromJsonAsync<List<Technician>>("api/lookups/technicians")!;

    public Task<List<SemenType>> GetSemenTypesAsync() =>
        _http.GetFromJsonAsync<List<SemenType>>("api/lookups/semen-types")!;

    public Task<List<PreservationMethod>> GetPreservationMethodsAsync() =>
        _http.GetFromJsonAsync<List<PreservationMethod>>("api/lookups/preservation-methods")!;

    public Task<List<Implant>> GetImplantsAsync() =>
        _http.GetFromJsonAsync<List<Implant>>("api/implants")!;

    public Task<List<Bull>> GetBullsAdminAsync() =>
        _http.GetFromJsonAsync<List<Bull>>("api/bulls")!;

    public Task<List<Donor>> GetDonorsAdminAsync() =>
        _http.GetFromJsonAsync<List<Donor>>("api/donors")!;

    public Task<List<ClientEntity>> GetClientsAdminAsync() =>
        _http.GetFromJsonAsync<List<ClientEntity>>("api/clients")!;

    public Task<List<Technician>> GetTechniciansAdminAsync() =>
        _http.GetFromJsonAsync<List<Technician>>("api/technicians")!;

    public Task<List<ProgramEntity>> GetProgramsAdminAsync() =>
        _http.GetFromJsonAsync<List<ProgramEntity>>("api/programs")!;

    public Task<List<SemenType>> GetSemenTypesAdminAsync() =>
        _http.GetFromJsonAsync<List<SemenType>>("api/sementypes")!;

    public Task<List<PreservationMethod>> GetPreservationMethodsAdminAsync() =>
        _http.GetFromJsonAsync<List<PreservationMethod>>("api/preservationmethods")!;

    public async Task<LoginResponse?> LoginAsync(LoginRequest dto)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", dto);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<LoginResponse>();
    }

    public Task<List<UserView>> GetUsersAsync() =>
        _http.GetFromJsonAsync<List<UserView>>("api/users")!;

    public async Task<UserView?> CreateUserAsync(UserUpsert dto)
    {
        var response = await _http.PostAsJsonAsync("api/users", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserView>();
    }

    public async Task UpdateUserAsync(int id, UserUpsert dto)
    {
        var response = await _http.PutAsJsonAsync($"api/users/{id}", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteUserAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/users/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<Implant?> CreateImplantAsync(ImplantUpsert dto)
    {
        var response = await _http.PostAsJsonAsync("api/implants", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Implant>();
    }

    public async Task UpdateImplantAsync(int id, ImplantUpsert dto)
    {
        var response = await _http.PutAsJsonAsync($"api/implants/{id}", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteImplantAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/implants/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<byte[]> ExportImplantsExcelAsync(List<int> ids)
    {
        var response = await _http.PostAsJsonAsync("api/implants/export", new { ids });
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();
    }

    public async Task<Bull?> CreateBullAsync(BullUpsert dto)
    {
        var response = await _http.PostAsJsonAsync("api/bulls", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Bull>();
    }

    public async Task UpdateBullAsync(int id, BullUpsert dto)
    {
        var response = await _http.PutAsJsonAsync($"api/bulls/{id}", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteBullAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/bulls/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<Donor?> CreateDonorAsync(DonorUpsert dto)
    {
        var response = await _http.PostAsJsonAsync("api/donors", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Donor>();
    }

    public async Task UpdateDonorAsync(int id, DonorUpsert dto)
    {
        var response = await _http.PutAsJsonAsync($"api/donors/{id}", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteDonorAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/donors/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<ClientEntity?> CreateClientAsync(NameUpsert dto)
    {
        var response = await _http.PostAsJsonAsync("api/clients", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ClientEntity>();
    }

    public async Task UpdateClientAsync(int id, NameUpsert dto)
    {
        var response = await _http.PutAsJsonAsync($"api/clients/{id}", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteClientAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/clients/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<Technician?> CreateTechnicianAsync(NameUpsert dto)
    {
        var response = await _http.PostAsJsonAsync("api/technicians", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Technician>();
    }

    public async Task UpdateTechnicianAsync(int id, NameUpsert dto)
    {
        var response = await _http.PutAsJsonAsync($"api/technicians/{id}", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteTechnicianAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/technicians/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<ProgramEntity?> CreateProgramAsync(NameUpsert dto)
    {
        var response = await _http.PostAsJsonAsync("api/programs", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProgramEntity>();
    }

    public async Task UpdateProgramAsync(int id, NameUpsert dto)
    {
        var response = await _http.PutAsJsonAsync($"api/programs/{id}", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteProgramAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/programs/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<SemenType?> CreateSemenTypeAsync(NameUpsert dto)
    {
        var response = await _http.PostAsJsonAsync("api/sementypes", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SemenType>();
    }

    public async Task UpdateSemenTypeAsync(int id, NameUpsert dto)
    {
        var response = await _http.PutAsJsonAsync($"api/sementypes/{id}", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteSemenTypeAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/sementypes/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<PreservationMethod?> CreatePreservationMethodAsync(NameUpsert dto)
    {
        var response = await _http.PostAsJsonAsync("api/preservationmethods", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PreservationMethod>();
    }

    public async Task UpdatePreservationMethodAsync(int id, NameUpsert dto)
    {
        var response = await _http.PutAsJsonAsync($"api/preservationmethods/{id}", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeletePreservationMethodAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/preservationmethods/{id}");
        response.EnsureSuccessStatusCode();
    }
}
