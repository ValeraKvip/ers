using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class LobbyController 
{
    protected static readonly string ALLOCATION_JOIN_CODE_KEY = "ajck";
    private static LobbyController _instance;  
    private string _lobbyID => _connectedLobby?.Id;
    private Lobby _connectedLobby;

    public static LobbyController Singelton => _instance ??= new LobbyController();
    public Lobby connectedLobby => _connectedLobby;
    public string JoinCode => _connectedLobby?.LobbyCode;

    private LobbyController()
    {
    }

    public async Task Initialize()
    {
        await UnityServices.InitializeAsync();
        await Authenticate();
    }


    //Try join already exists lobbies or create new one public lobby;
    public async Task<Lobby> CreateOrJoinLobby()
    {
        try
        {
            _connectedLobby = await QuickJoinLobby() ?? await CreateLobby();
            if (_connectedLobby != null)
            {
                Debug.Log($"connected or created lobby {_connectedLobby.Id}");
                return _connectedLobby;
            }

            Debug.Log("Failed to connect or create lobby ");

        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        return null;
    }

    private async Task<string> GetRegion()
    {
        try
        {
            var regions = await Relay.Instance.ListRegionsAsync();
            if (regions.Count > 0)
            {
                Debug.Log($"RGION {regions[0].Id}");
                return regions[0].Id;
            }
        }
        catch(Exception e)
        {
            Debug.LogException(e);
        }
        return null;
    }

    public async Task<Lobby> CreateLobby(bool publicLobby = true)
    {
        try
        {
         //   var region = await GetRegion();
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);
            var allocationJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            var dataObj = new DataObject(DataObject.VisibilityOptions.Public, allocationJoinCode);
            var options = new CreateLobbyOptions
            {
                Data = new Dictionary<string, DataObject> { { ALLOCATION_JOIN_CODE_KEY, dataObj } },
                IsPrivate = !publicLobby

            };

            _connectedLobby = await Lobbies.Instance.CreateLobbyAsync("lobby", 2, options);                     
            
            SetHostRelayData(allocation);
            NetworkManager.Singleton.StartHost();
            return _connectedLobby;

        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return null;
        }
    }


    private async Task<Lobby> QuickJoinLobby()
    {
        try
        {        
            _connectedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            if(_connectedLobby == null)
            {
                return null;
            }
         
            var allocation = await RelayService.Instance.JoinAllocationAsync(_connectedLobby.Data[ALLOCATION_JOIN_CODE_KEY].Value);
            SetClientRelayData(allocation);
            NetworkManager.Singleton.StartClient();

            return _connectedLobby;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        return null;
    }

    public async Task<Lobby> JoinLobby(string code)
    {
        try
        {
            Debug.Log($"JOINLOBBY CODE: {code}");
            _connectedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code);

            if (_connectedLobby == null)
            {
                Debug.Log($"JOINLOBBY unable joined the lobby");
                return null;
            }
            Debug.Log($"JOINLOBBY joined to the lobby: {_connectedLobby.Data[ALLOCATION_JOIN_CODE_KEY].Value}");

            var allocation = await RelayService.Instance.JoinAllocationAsync(_connectedLobby.Data[ALLOCATION_JOIN_CODE_KEY].Value);
            Debug.Log($"JOINLOBBY joined to the relay {allocation != null}");
            SetClientRelayData(allocation);
            Debug.Log($"JOINLOBBY setup transport");
            NetworkManager.Singleton.StartClient();
            Debug.Log($"JOINLOBBY client started");

            return _connectedLobby;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            Debug.Log($"JOINLOBBY exception: {e.Message}");
        }

        return null;
    }

    public async Task DisconnectLobby()
    {
        if (_connectedLobby == null)
        {
            return;
        }
       
        string playerId = AuthenticationService.Instance.PlayerId;
        await LobbyService.Instance.RemovePlayerAsync(_connectedLobby.Id, playerId);
        _connectedLobby = null;
    }
    public async Task DeleteLobbyAsync()
    {
        try
        {
            if (!string.IsNullOrEmpty(_lobbyID))
            {
                await LobbyService.Instance.DeleteLobbyAsync(_lobbyID);
            }
                   
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

    }

    private async Task Authenticate()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            var playerID = AuthenticationService.Instance.PlayerId;          
        }
    }

    private void SetHostRelayData(Allocation allocation)
    {
        UnityTransport transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
         RelayServerData relayServerData = new RelayServerData(allocation, "wss");
         transport.SetRelayServerData(relayServerData);

        // transport.SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes,
        //     allocation.Key, allocation.ConnectionData);

        SharedData.Singelton.relayHostData = new()
        {
            IPv4Address = allocation.RelayServer.IpV4,
            Port = (ushort)allocation.RelayServer.Port,
            AllocationID = allocation.AllocationId,
            AllocationIDBytes = allocation.AllocationIdBytes,
            ConnectionData = allocation.ConnectionData,
            Key = allocation.Key,
        };
    }

    private void SetClientRelayData(JoinAllocation allocation)
    {
        UnityTransport transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
         RelayServerData relayServerData = new RelayServerData(allocation, "wss");
         transport.SetRelayServerData(relayServerData);

        // transport.SetClientRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes,
        //   allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);

        SharedData.Singelton.relayJoinData = new()
        {
            IPv4Address = allocation.RelayServer.IpV4,
            Port = (ushort)allocation.RelayServer.Port,
            AllocationID = allocation.AllocationId,
            AllocationIDBytes = allocation.AllocationIdBytes,
            ConnectionData = allocation.ConnectionData,
            HostConnectionData = allocation.HostConnectionData,
            Key = allocation.Key,
        };
    }

    public void HeartbeatLobby()
    {
        if(_connectedLobby != null && (NetworkManager.Singleton?.IsServer ?? false))
        {
            LobbyService.Instance.SendHeartbeatPingAsync(_connectedLobby.Id);
        }
    }

}
