using Microsoft.AspNetCore.Http;

namespace Entities.Dtos
{
    public class ProfileImageUpdateDto
    {
        public int UserId { get; set; }
        public IFormFile File { get; set; }
    }
}