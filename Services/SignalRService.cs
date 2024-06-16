using JadeMaui.Helpers;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

namespace JadeMaui.Services;

public class SignalRService : ISignalRService
{
    private readonly HubConnection _connection;
    private readonly IConfigurationManager _configuration = ServiceHelper.GetService<IConfigurationManager>();
    private readonly IUserManager _userManager = ServiceHelper.GetService<IUserManager>();

    public SignalRService()
    {
        _connection = new HubConnectionBuilder()
            .WithUrl(_configuration["SignalR:NoteUrl"]!, options =>
            {
                options.AccessTokenProvider = async () => await _userManager.GetAccessToken();
            })
            .Build();
    }

    public async Task StartIfPossible()
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

    public HubConnectionState GetState() => _connection.State;
}