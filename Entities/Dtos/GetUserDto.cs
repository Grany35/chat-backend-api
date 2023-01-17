using System;
namespace Entities.Dtos
{
	public class GetUserDto
	{
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<UserOperationClaimDto> UserOperationClaims { get; set; }
    }
}

