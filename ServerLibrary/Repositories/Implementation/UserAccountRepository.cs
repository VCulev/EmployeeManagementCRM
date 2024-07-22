using MainLibrary.Responses;
using MainLibrary.DTOs;
using MainLibrary.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServerLibrary.Data;
using ServerLibrary.Helpers;
using ServerLibrary.Repositories.Contracts;
using Constants = ServerLibrary.Helpers.Constants;

namespace ServerLibrary.Repositories.Implementation
{
    public class UserAccountRepository : IUserAccount
    {
        private readonly IOptions<JwtSection> _config;
        private readonly AppDbContext _appDbContext;

        public UserAccountRepository(IOptions<JwtSection> config, AppDbContext appDbContext) =>
            (_config, _appDbContext) = (config, appDbContext);

        public async Task<GeneralResponse> CreateAsync(Register? user)
        {
            if (user == null) 
                return new GeneralResponse(false, "Model is Empty");

            var checkUser = await FindUserByEmail(user.Email!);
            if (checkUser != null) 
                return new GeneralResponse(false, "User already registered");

            var applicationUser = await AddToDatabase(new User
            {
                Fullname = user.Fullname,
                Email = user.Email!,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password!)
            });

            var checkAdminRole = await _appDbContext.SystemRoles.FirstOrDefaultAsync(_ => _.Name!.Equals(Constants.Admin));
            if (checkAdminRole == null)
            {
                var createAdminRole = await AddToDatabase(new SystemRole { Name = Constants.Admin });
                await AddToDatabase(new UserRole { RoleId = createAdminRole.Id, UserId = applicationUser.Id });
                return new GeneralResponse(true, "Account created with admin role!");
            }

            var checkUserRole = await _appDbContext.SystemRoles.FirstOrDefaultAsync(_ => _.Name!.Equals(Constants.User));
            if (checkUserRole == null)
            {
                var response = await AddToDatabase(new SystemRole { Name = Constants.User });
                await AddToDatabase(new UserRole { RoleId = response.Id, UserId = applicationUser.Id });
            }
            else
            {
                await AddToDatabase(new UserRole { RoleId = checkUserRole.Id, UserId = applicationUser.Id });
            }

            return new GeneralResponse(true, "Account created successfully");
        }

        public Task<LoginResponse> SignInAsync(Login user)
        {
            throw new NotImplementedException();
        }

        private async Task<User> FindUserByEmail(string email) => await
            _appDbContext.Users.FirstOrDefaultAsync(_ => _.Email!.ToLower().Equals(email.ToLower()));

        private async Task<T> AddToDatabase<T>(T model)
        {
            var result = _appDbContext.Add(model!);
            await _appDbContext.SaveChangesAsync();
            return (T)result.Entity;
        }
    }
}