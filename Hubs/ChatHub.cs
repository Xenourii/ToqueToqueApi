using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using ToqueToqueApi.Databases.Models;
using ToqueToqueApi.Services;

namespace ToqueToqueApi.Hubs
{
    public class ChatHub : Hub<IChatClient>
    {
        private readonly IConversationService _conversationService;
        private static readonly ConcurrentDictionary<string, string> SubscribedClients = new ConcurrentDictionary<string, string>();

        public ChatHub(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;
            if (SubscribedClients.ContainsKey(connectionId))
                await UnsubscribeClient(connectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task Subscribe(string conversationId)
        {
            var connectionId = Context.ConnectionId;
            var isSuccess = SubscribedClients.TryAdd(connectionId, conversationId);
            if (!isSuccess)
                throw new HubException("Subscription failed! Unable to register the client.");

            await Groups.AddToGroupAsync(connectionId, conversationId);
        }

        private async Task UnsubscribeClient(string connectionId)
        {
            SubscribedClients.TryRemove(connectionId, out var conversationId);
            if (conversationId is null)
                return;

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
        }

        public async Task SendMessage(ChatMessage message)
        {
            var connectionId = Context.ConnectionId;
            var isSuccess = SubscribedClients.TryGetValue(connectionId, out var conversationId);
            if (!isSuccess)
                throw new HubException("The client hasn't subscribed.");

            isSuccess = int.TryParse(conversationId, out var convertedConversationId);
            if (!isSuccess)
                throw new HubException("Unexpected conversationId.");

            _conversationService.Create(new MessageDb
            {
                Text = message.Text,
                SendingTime = message.SendingTime,
                UserId = message.UserId,
                ConversationId = convertedConversationId
            });

            await Clients.Groups(conversationId).ReceiveMessage(message);
        }
    }
}