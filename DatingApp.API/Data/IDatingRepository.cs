using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    public interface IDatingRepository
    {
         void Add<T>(T Entity) where T: class;
         void Delete<T>(T Entity) where T: class;
         Task<bool> SaveAll();
         Task<PagedList<User>> GetUsers(UserParams userParams);
         Task<User> GetUser(int Id);
         Task<Photo> GetMainPhotoForUser(int userId);
         Task<Photo> GetPhoto(int Id);
         Task<Like> GetLike(int userId, int recipientId);
         Task<Message> GetMessage(int Id);         
         Task<PagedList<Message>> GetMessagesForUser(MesageParams messageParams);
         Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId);

    }
}