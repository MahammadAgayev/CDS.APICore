using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using CDS.APICore.Bussiness.Abstraction;
using CDS.APICore.Entities;
using CDS.APICore.Helpers;
using CDS.APICore.Models.Account;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CDS.APICore.Services
{
    public interface IAccountService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest request);
        void Register(RegisterRequest request);
    }

    public class AccountService : IAccountService
    {

        private readonly IAccountManager _accountManager;
        private readonly IHashService _hashService;

        private readonly AppSettings _settings;

        public AccountService(IAccountManager accountManager, IHashService hashService, IOptions<AppSettings> settings)
        {
            _accountManager = accountManager;
            _hashService = hashService;

            _settings = settings.Value;
        }


        public AuthenticateResponse Authenticate(AuthenticateRequest request)
        {
            var account = _accountManager.Get(request.Username);

            if(account is null || !_hashService.VerifyHash(request.Password, account.PasswordHash))
            {
                throw new AppException("Username or password is incorrect");
            }

            var token = this.generateJwtToken(account);

            var response = new AuthenticateResponse
            {
                JwtToken = token
            };

            return response;
        }

        public void Register(RegisterRequest request)
        {
            if(_accountManager.Get(request.Username) != null)
            {
                throw new AppException("User already exists");
            }

            var account = new Account
            {
                Username = request.Username,
                PasswordHash = _hashService.Hash(request.Password).Digest,
                Created = DateTime.Now,
                Updated = DateTime.Now
            };

            _accountManager.Create(account);
        }


        private string generateJwtToken(Account account)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_settings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                new[] {
                    new Claim("id", account.Id.ToString()),
                    new Claim("username", account.Username),
                }),
                Expires = DateTime.UtcNow.AddDays(15), //Token expires after 15 days
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
