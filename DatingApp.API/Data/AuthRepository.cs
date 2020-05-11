using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;

        }
        public async Task<User> Login(string username, string password)
        {
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.Name == username);

            if(user == null)
                return null;
            
            if(!VerifyPasswordHash(password, user.PassswordHash,user.PasswordSalt))
                return null;       
                
            return user;
                 }

        private bool VerifyPasswordHash(string password, byte[] passswordHash, byte[] passwordSalt)
        {
           using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {               
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for(int i=0; i < computedHash.Length; i++)
                {
                    if(computedHash[i] != passswordHash[i])
                    return false;
                }

                return true;
            }
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;

            CreatePassowrdHash(password, out passwordHash, out passwordSalt);

            user.PassswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private void CreatePassowrdHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string usernmae)
        {
            if(await _context.Users.AnyAsync(x => x.Name == usernmae))
                return true;

            return false;
        }
    }
}