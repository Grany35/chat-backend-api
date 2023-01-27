using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Dtos.Message;
using Microsoft.EntityFrameworkCore;

namespace Business.Repositories.MessageRepository
{
    public class MessageManager : IMessageService
    {
        private readonly IMessageDal _messageDal;
        private readonly IUserDal _userDal;

        public MessageManager(IMessageDal messageDal, IUserDal userDal)
        {
            _messageDal = messageDal;
            _userDal = userDal;
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _messageDal.GetAsync(x => x.Id == id);
        }

        public async Task<List<MessageDto>> GetMessagesForUser(int userId)
        {
            var messages = await _messageDal.GetAllAsync(x =>
                    x.SenderId == userId ||
                    x.RecipientId == userId,
                include: i => i
                    .Include(r => r.Recipient)
                    .Include(s => s.Sender));


            var messageDtos = messages.Select(x => new MessageDto
            {
                Id = x.Id,
                SenderId = x.SenderId,
                RecipientId = x.RecipientId,
                Content = x.Content,
                MessageSent = x.MessageSent,
                RecipientFullName = x.Recipient.FirstName + " " + x.Recipient.LastName,
                SenderFullName = x.Sender.FirstName + " " + x.Sender.LastName,
                DateRead = x.DateRead,
            }).ToList();

            return messageDtos;
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(int currentUserId, int recipientId)
        {
            var messages = await _messageDal.GetAllAsync(x =>
                    x.RecipientId == currentUserId && x.SenderId == recipientId ||
                    x.RecipientId == recipientId && x.SenderId == currentUserId,
                include: i => i
                    .Include(r => r.Recipient)
                    .Include(s => s.Sender),
                orderBy: x => x
                    .OrderByDescending(o => o.MessageSent));

            var unreadMessages = messages.Where(m =>
                    m.DateRead == null &&
                    m.RecipientId == currentUserId)
                .ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                    await _messageDal.UpdateAsync(message);
                }
            }

            return messages.Select(x => new MessageDto
            {
                Id = x.Id,
                SenderId = x.SenderId,
                RecipientId = x.RecipientId,
                Content = x.Content,
                RecipientFullName = x.Recipient.FirstName + " " + x.Recipient.LastName,
                SenderFullName = x.Sender.FirstName + " " + x.Sender.LastName,
                DateRead = x.DateRead,
                MessageSent = x.MessageSent,
            });
        }

        public async Task<MessageDto> CreateMessage(CreateMessageDto dto)
        {
            var senderUser = await _userDal.GetAsync(x => x.Id == dto.SenderId);

            if (senderUser.Id == dto.RecipientId)
                throw new Exception("You cannot send message to yourself");

            var recipientUser = await _userDal.GetAsync(x => x.Id == dto.RecipientId);
            if (recipientUser == null)
                throw new Exception("Recipient not found");

            var message = new Message
            {
                Sender = senderUser,
                Recipient = recipientUser,
                SenderId = dto.SenderId,
                RecipientId = dto.RecipientId,
                Content = dto.Content,
                RecipientFullName = recipientUser.FirstName + " " + recipientUser.LastName,
                SenderFullName = senderUser.FirstName + " " + senderUser.LastName,
            };
            await _messageDal.AddAsync(message);

            MessageDto messageDto = new MessageDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                RecipientId = message.RecipientId,
                Content = message.Content,
                MessageSent = DateTime.Now,
                RecipientFullName = recipientUser.FirstName + " " + recipientUser.LastName,
                SenderFullName = senderUser.FirstName + " " + senderUser.LastName,
            };
            return messageDto;
        }
    }
}