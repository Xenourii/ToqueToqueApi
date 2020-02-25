using System.Threading.Tasks;

namespace ToqueToqueApi.Hubs
{
    public interface IChatClient
    {
        Task ReceiveMessage(ChatMessage response);
    }
}