using Microsoft.AspNetCore.SignalR.Client;

namespace JadeMaui.Services;

public interface ISignalRService
{
    Task StartIfPossible();
    Task<HubConnection> GetConnection();
    Task StopConnection();
    HubConnectionState GetState();
}