using BonhommePendu.Events;
using BonhommePendu.Models;
using BonhommePendu.Services;
using Microsoft.AspNetCore.SignalR;

namespace BonhommePendu.Hubs
{
    public class PenduHub : Hub
    {
        private PenduService _penduService;

        public PenduHub(PenduService penduService)
        {
            _penduService = penduService;
        }

        public async override Task OnConnectedAsync()
        {
            GameData? gameData = _penduService.JoinGame();
            if (gameData != null)
                await Clients.Caller.SendAsync("GameData", gameData);
        }

        public async Task StartGame()
        {
            GameData? gameData = await _penduService.StartGame();
            if (gameData != null)
                await Clients.All.SendAsync("GameData", gameData);
        }

        public async Task GuessLetter(char letter)
        {
            GameEvent? gameEvent = _penduService.GuessLetter(letter);
            if(gameEvent != null)
                await Clients.All.SendAsync("Event", gameEvent as GameEvent);
        }
    }
}
