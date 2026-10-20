using Microsoft.AspNetCore.SignalR;
using SignalR.Services;

namespace SignalR.Hubs
{
    public class PizzaHub : Hub
    {
        private readonly PizzaManager _pizzaManager;

        public PizzaHub(PizzaManager pizzaManager) {
            _pizzaManager = pizzaManager;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            //-	Lors d’une connexion/déconnexion, il faut mettre le nombre d’utilisateurs à jour
            _pizzaManager.AddUser();
            await Clients.All.SendAsync("UpdateNbUsers", _pizzaManager.NbConnectedUsers);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnConnectedAsync();

            //-	Lors d’une connexion/déconnexion, il faut mettre le nombre d’utilisateurs à jour
            await Clients.All.SendAsync("UpdateNbUsers", _pizzaManager.NbConnectedUsers);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SelectChoice(PizzaChoice choice)
        {

            //o	Il faut joindre le groupe de notre pizza (Il y a une méthode GetGroupName)
            await Groups.AddToGroupAsync(Context.ConnectionId, _pizzaManager.GetGroupName(choice));

            //o	Il faut mettre le client à jour avec le prix de la pizza choisi
            await Clients.Caller.SendAsync("UpdatePizzaPrice", _pizzaManager.GetPizzaPrice(choice));

            //o	Il faut mettre le client à jour avec le nombre de pizzas et la quantité d’argent de ce groupe
            await Clients.Caller.SendAsync("UpdateNbPizzasAndMoney", _pizzaManager.GetNbPizzas(choice), _pizzaManager.GetMoney(choice));

        }

        public async Task UnselectChoice(PizzaChoice choice)
        {
            //Changer de pizza : quitter le groupe de l’ancienne pizza
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, _pizzaManager.GetGroupName(choice));
        }

        public async Task AddMoney(PizzaChoice choice)
        {
            //o	L’argent que vous ajoutez s’additionne à l’argent pour ce groupe
            _pizzaManager.IncreaseMoney(choice);

            //o	On ajoute 2$ à chaque clique et on doit mettre les clients à jour
            await Clients.Group(_pizzaManager.GetGroupName(choice)).SendAsync("UpdateMoney", _pizzaManager.GetMoney(choice));
        }

        public async Task BuyPizza(PizzaChoice choice)
        {
            //o	Il faut réduire l’argent du groupe de la valeur du prix de la pizza et
            //augmenté le nombre de pizza de 1 (Déjà fait pas la méthode BuyPizza)  
            _pizzaManager.BuyPizza(choice);

            //et ensuite mettre les clients à jour
            await Clients.Group(_pizzaManager.GetGroupName(choice)).SendAsync("UpdateNbPizzasAndMoney", _pizzaManager.GetNbPizzas(choice), _pizzaManager.GetMoney(choice));
        }
    }
}
