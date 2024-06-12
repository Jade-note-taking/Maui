using JadeMaui.Helpers;
using Microsoft.AspNetCore.SignalR.Client;

namespace JadeMaui.Services;

public class CosmosService
{
    private readonly HubConnection _connection = ServiceHelper.GetService<HubConnection>();

    private async Task StartIfPossible()
    {
        if (_connection.State == HubConnectionState.Disconnected)
        {
            await _connection.StartAsync();
        }
    }

    public async Task<HubConnection> GetConnection()
    {
        await StartIfPossible();
        return _connection;
    }

    public async Task StopConnection()
    {
        if (_connection.State == HubConnectionState.Disconnected) return;

        await _connection.StopAsync();
    }
}