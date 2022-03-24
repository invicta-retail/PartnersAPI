using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using InvictaPartnersAPI.Helpers;
using InvictaPartnersAPI.Models;
using System.Security.Cryptography;
using System.Threading.Tasks;
using InvictaPartnersAPI.Interfaces;
using Microsoft.Extensions.Logging;
using InvictaPartnersAPI.Exceptions;

namespace InvictaPartnersAPI.Services
{
    public interface IUserService
    {
        Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest model);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(string id);
    }

    public class UserService : IUserService
    {

        private readonly AppSettings _appSettings;
        private readonly ILogger<UserService> _logger;
        private readonly ICosmosDbService _cosmosDbService;
        public UserService(IOptions<AppSettings> appSettings,ICosmosDbService cosmosDbService, ILogger<UserService> logger)
        {
            _appSettings = appSettings.Value;
            _cosmosDbService = cosmosDbService;
            _logger = logger;
        }

        public async Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest model)
        {
            try
            {
                _logger.LogDebug($"Connecting to cosmosdb");
                var query = "SELECT * FROM c where c.type = 'user' and c.userName = '" + model.Username + "' and c.password = '" + GetHashString(model.Password) + "'";
                _logger.LogDebug($"Query : {query}");
                var userList = await _cosmosDbService.GetUserAsyncQuery(query);
                _logger.LogDebug($"Found users: {userList.Count()}");
                var token = "";
                foreach (User _user in userList)
                {
                    token = generateJwtToken(_user);
                    _logger.LogDebug("Approved authentication");
                    return new AuthenticateResponse(_user, token);
                }
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception found: {e.Message}");
                throw new BusinessException($"Exception found: {e.Message}");
            }
            

        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            try
            {
                _logger.LogDebug("Extracting user list");
                var query = "SELECT * FROM c where c.type = 'user' ";
                _logger.LogDebug($"Query : {query}");
                var userList = (List<User>)await _cosmosDbService.GetUserAsyncQuery(query);
                _logger.LogDebug($"Found users: {userList.Count()}");
                return userList;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception found: {e.Message}");
                throw new BusinessException($"Exception found: {e.Message}");
            }
            
        }

        public async Task<User> GetByIdAsync(string id)
        {
            try
            {
                _logger.LogDebug($"Extracting user: {id}");
                return await _cosmosDbService.GetUserAsync(id);
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception found: {e.Message}");
                throw new BusinessException($"Exception found: {e.Message}");
            }
            
        }

        private string generateJwtToken(User user)
        {
            try
            {
                _logger.LogDebug($"Generating token for the user: {user.firstName}");
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] { new Claim("id", user.id) }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                _logger.LogDebug("Token created successfully");
                _logger.LogDebug($"Token: {token}");
                return tokenHandler.WriteToken(token);
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception found: {e.Message}");
                throw new BusinessException($"Exception found: {e.Message}");
            }
            
        }

        public static byte[] GetHash(string inputString)
        {

            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

    }
}