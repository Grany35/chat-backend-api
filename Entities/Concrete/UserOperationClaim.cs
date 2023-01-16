namespace Entities.Concrete
{
    public class UserOperationClaim
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int OperationClaimId { get; set; }

        public User User { get; set; }
        public OperationClaim OperationClaim { get; set; }

        public UserOperationClaim()
        {
            User = new User();
            OperationClaim = new OperationClaim();
        }
    }
}
