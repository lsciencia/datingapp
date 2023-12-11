using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext dataContext,
            IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }
        public void AddMessage(Message message)
        {
            _dataContext.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _dataContext.Messages.Remove(message);
        }

        public async Task<Message> GetMessageAsync(int id)
        {
            return await _dataContext.Messages.FindAsync(id);
        }

        public async Task<PagedList<MessageDTO>> GetMessagesForUserAsync(MessageParams messageParams)
        {
            var query = _dataContext.Messages
                .OrderByDescending(x => x.MessageSent)
                .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.RecipientUsername == messageParams.Username
                        && u.RecipientDeleted == false),

                "Outbox" => query.Where(u => u.SenderUsername == messageParams.Username
                        && u.SenderDeleted == false),

                _ => query.Where(u => u.RecipientUsername == messageParams.Username
                        && u.RecipientDeleted == false
                        && u.DateRead == null)
            };

            var messages = query.ProjectTo<MessageDTO>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDTO>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDTO>> GetMessageThreadAsync(string currentUsername, string recipientUsername)
        {
            var messages = await _dataContext.Messages
                .Include(u => u.Sender).ThenInclude(p => p.Photos)
                .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                .Where(
                    m =>
                        m.RecipientUsername == currentUsername
                        && m.SenderUsername == recipientUsername
                        && m.RecipientDeleted == false
                            ||
                        m.RecipientUsername == recipientUsername
                        && m.SenderUsername == currentUsername
                        && m.SenderDeleted == false)
                .OrderBy(m => m.MessageSent)
                .ToListAsync();

            var unreadMessages = messages.Where(m => m.DateRead == null && m.RecipientUsername == currentUsername).ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }
                await _dataContext.SaveChangesAsync();
            }

            return _mapper.Map<IEnumerable<MessageDTO>>(messages);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }
    }
}
