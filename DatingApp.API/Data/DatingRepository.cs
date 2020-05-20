using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;
        public DatingRepository(DataContext context)
        {
            _context = context;

        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync( x=> x.LikerId == userId && x.LikeeId == recipientId);
        }
 
        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.Where(x => x.userId == userId).FirstOrDefaultAsync(p => p.IsMain == true);
        }

        public async Task<Message> GetMessage(int Id)
        {
            return await _context.Messages.FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MesageParams messageParams)
        {
            var messages = _context.Messages.Include( u => u.Sender).ThenInclude(p => p.Photos)
                            .Include( u => u.Recipient).ThenInclude(p => p.Photos)
                            .AsQueryable();
                
            switch(messageParams.MessageContainer)
            {
                case "Inbox":
                messages = messages.Where(x => x.RecipientId == messageParams.userId && x.RecipientDeleted == false);
                break;
                case "Outbox":
                messages = messages.Where(x => x.SenderId == messageParams.userId && x.SenderDeleted == false);
                break;
                default:
                messages = messages.Where(x => x.RecipientId == messageParams.userId && x.IsRead == false
                             && x.RecipientDeleted == false);
                break;
            }

            messages = messages.OrderByDescending(x => x.MessageSent);

            return await PagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);

        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
            var messages = await _context.Messages
                            .Include( u => u.Sender).ThenInclude(p => p.Photos)
                            .Include( u => u.Recipient).ThenInclude(p => p.Photos)
                            .Where(p=> p.RecipientId == userId  && p.RecipientDeleted == false
                            && p.SenderId == recipientId 
                            || p.RecipientId == recipientId && p.SenderId == userId && p.SenderDeleted == false)
                            .OrderByDescending(m => m.MessageSent)
                            .ToListAsync();
            
            return messages;
        }

        public async Task<Photo> GetPhoto(int Id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == Id);
            return photo;
        }

        public async Task<User> GetUser(int Id)
        {
            return await _context.Users.Include(p =>p.Photos).FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users =  _context.Users.Include(p =>p.Photos)
                        .OrderByDescending(u => u.LastActive).AsQueryable();

            users = users.Where(x=> x.Id != userParams.userId && x.Gender == userParams.Gender);

            if(userParams.Likers)
            {
                var userLikers = await GetUserLikes(userParams.userId, userParams.Likers);
                users = users.Where(u => userLikers.Contains(u.Id));
            }

            if(userParams.Likees)
            {
                var userLikees = await GetUserLikes(userParams.userId, userParams.Likers);
                users = users.Where(u => userLikees.Contains(u.Id));

            }

            if(userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var minDob = DateTime.Today.AddYears(- userParams.MaxAge -1);
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge -1);

                users = users.Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);
            }

            if ( !string.IsNullOrEmpty(userParams.OderBy))
            {
                switch (userParams.OderBy)
                {
                    case "created":
                    users = users.OrderByDescending(u => u.Created);
                    break;
                    default:
                    users = users.OrderByDescending(u => u.LastActive);
                    break;
                }
            }

            return await PagedList<User>.CreateAsync(users,userParams.PageNumber,userParams.PageSize);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            var user = await _context.Users.Include(x => x.Likees).Include(x=>x.Likers)
            .FirstOrDefaultAsync(x =>x.Id == id);

            if(likers)
                return user.Likers.Where(x=>x.LikeeId == id).Select(i => i.LikerId);
            else
                return user.Likees.Where(x=>x.LikerId == id).Select(id=>id.LikeeId);
        } 
    }
}