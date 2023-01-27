namespace Entities.Dtos.Message
{
    public class CreateMessageDto
    {
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public string Content { get; set; }
    }
}