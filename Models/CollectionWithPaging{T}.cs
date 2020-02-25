using Newtonsoft.Json;

namespace ToqueToqueApi.Models
{
    public class PagedCollection<T> : Collection<T>
    {
        public static PagedCollection<T> Create(T[] items, int size, PagingOptions pagingOptions)
            => Create<PagedCollection<T>>(items, size, pagingOptions);

        public static TResponse Create<TResponse>(T[] items, int size, PagingOptions pagingOptions)
            where TResponse : PagedCollection<T>, new() =>
            new TResponse
            {
                Data = items,
                Count = size,
                Offset = pagingOptions.Offset,
                Limit = pagingOptions.Limit
            };

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Offset { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Limit { get; set; }

        public int Count { get; set; }
    }
}