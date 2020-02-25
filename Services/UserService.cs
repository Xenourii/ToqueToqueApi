using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ToqueToqueApi.Databases;
using ToqueToqueApi.Databases.Models;
using ToqueToqueApi.Exceptions;
using ToqueToqueApi.Helpers;

namespace ToqueToqueApi.Services
{
    public class UserService : IUserService
    {
        private readonly ToqueToqueContext _dbContext;

        public UserService(ToqueToqueContext context)
        {
            _dbContext = context;
        }

        public UserDb Authenticate(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;

            var user = _dbContext.Users.SingleOrDefault(x => x.Email == email);

            if (user == null)
                return null;

            if (!PasswordHelper.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        public IEnumerable<UserDb> GetAll() => _dbContext.Users;

        public UserDb Get(int id) => _dbContext.Users.Find(id);

        public UserDb Create(UserDb user, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new PasswordRequiredException("Password is required");

            if (_dbContext.Users.Any(x => x.Email == user.Email))
                throw new EmailAlreadyTakenException($"Email {user.Email} is already taken");

            PasswordHelper.CreatePasswordHash(password, out var passwordHash, out var passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            if (!string.IsNullOrWhiteSpace(user.ProfilePicture))
                SaveBase64Image(user);

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            return user;
        }

        private static void SaveBase64Image(UserDb user)
        {
            var folderName = Path.Combine("Public", "Profiles");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            var fileName = $"{Guid.NewGuid()}.jpg";
            var fullPath = Path.Combine(pathToSave, fileName);
            var dbPath = Path.Combine(folderName, fileName);
            dbPath = dbPath.Replace(Path.DirectorySeparatorChar, '/');

            var base64Image = user.ProfilePicture;
            var bytes = Convert.FromBase64String(base64Image);
            
            using (var imageFile = new FileStream(fullPath, FileMode.Create))
            {
                imageFile.Write(bytes, 0, bytes.Length);
                imageFile.Flush();
            }

            user.ProfilePicture = dbPath;
        }

        public void Update(UserDb userParam, string password = null)
        {
            var user = _dbContext.Users.Find(userParam.Id);

            if (user == null)
                throw new NotFoundException("User not found");

            if (!string.IsNullOrWhiteSpace(userParam.Email) && userParam.Email != user.Email)
            {
                if (_dbContext.Users.Any(x => x.Email == userParam.Email))
                    throw new EmailAlreadyTakenException($"Email {userParam.Email} is already taken");

                user.Email = userParam.Email;
            }

            // update user properties if provided
            if (!string.IsNullOrWhiteSpace(userParam.FirstName))
                user.FirstName = userParam.FirstName;

            if (!string.IsNullOrWhiteSpace(userParam.LastName))
                user.LastName = userParam.LastName;

            // update password if provided
            if (!string.IsNullOrWhiteSpace(password))
            {
                PasswordHelper.CreatePasswordHash(password, out var passwordHash, out var passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = _dbContext.Users.Find(id);

            if (user == null)
                throw new NotFoundException("User not found");

            user.Phone = string.Empty;
            user.ProfilePicture = string.Empty;
            user.Address = string.Empty;
            user.BirthDate = DateTime.MinValue;
            user.Email = string.Empty;
            user.FirstName = string.Empty;
            user.LastName = string.Empty;
            user.IsEnable = false;

            _dbContext.Users.Remove(user);
            _dbContext.SaveChanges();
        }
    }
}