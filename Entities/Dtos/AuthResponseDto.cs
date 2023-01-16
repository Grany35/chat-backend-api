using System;
namespace Entities.Dtos
{
	public class AuthResponseDto
	{
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AccessToken { get; set; }
        public DateTime Expiration { get; set; }
        public List<UserOperationClaimDto> UserOperationClaims { get; set; } = new List<UserOperationClaimDto>();
    }

    public class UserOperationClaimDto
    {
        public string Name { get; set; }
    }
}

