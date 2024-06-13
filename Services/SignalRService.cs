using JadeMaui.Helpers;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

namespace JadeMaui.Services;

public class SignalRService
{
    private readonly HubConnection _connection;
    private readonly ConfigurationManager _configuration = ServiceHelper.GetService<ConfigurationManager>();

    public SignalRService()
    {
        _connection = new HubConnectionBuilder()
            .WithUrl(_configuration["SignalR:NoteUrl"]!, options =>
            {
                options.AccessTokenProvider = async () => await UserManager.GetAccessToken();
            })
            .Build();
    }

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