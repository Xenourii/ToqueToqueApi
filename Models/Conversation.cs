using System.Collections.Generic;

namespace ToqueToqueApi.Models
{
    public class Conversation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserNames { get; set; }
        public List<int> UsersId { get; set; }
    }
}
