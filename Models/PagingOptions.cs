using System.ComponentModel.DataAnnotations;

namespace ToqueToqueApi.Models
{
    public sealed class PagingOptions
    {
        [Range(1, int.MaxValue, ErrorMessage = "Offset must be greater than 0.")]
        public int? Offset { get; set; }

        [Range(1, 50, ErrorMessage = "Limit must be greater than 0 and less than 50.")]
        public int? Limit { get; set; }
    }
}