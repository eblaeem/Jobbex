using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ViewModel.User
{
    public class Token
    {
        [JsonPropertyName("refreshToken")]
        [Required]
        public string RefreshToken { get; set; }
    }
}
