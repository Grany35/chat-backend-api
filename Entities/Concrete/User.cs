﻿namespace Entities.Concrete
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] PasswordSalt { get; set; }
        public byte[] PasswordHash { get; set; }
        public string ConfirmValue { get; set; }
        public bool IsConfirm { get; set; }
        public string? ForgotPasswordValue { get; set;}
        public DateTime? ForgotPasswordRequestDate { get; set; }
        public bool IsForgotPasswordComplete { get; set; }

        public ICollection<UserOperationClaim> UserOperationClaims { get; set; }

        public User()
        {
            UserOperationClaims = new HashSet<UserOperationClaim>();
        }

    }
}
