using Entities.Concrete;


namespace Entities.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string About { get; set; }
        public string ProfileImageUrl { get; set; }

        public static UserDto ToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                About = user.About ?? null,
                ProfileImageUrl = !String.IsNullOrEmpty(user.ProfileImageUrl)
                    ? "files/uploads/" + user.ProfileImageUrl
                    : null,
            };
        }
    }
}