//===========================
// Copyright (c) Tarteeb LLC
// Manage Your Money Easy
//===========================

using EasyPay.Api.Models.Clients;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EasyPay.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Client> Clients { get; set; }

        public async ValueTask<Client> RegisterAsync(Client client)
        {
            this.Clients.Add(client);
            
            await this.SaveChangesAsync();

            return client;
        }

        public async ValueTask<Client> LoginAsync (string login)
        {
            var maybeClient = await this.Clients.FirstOrDefaultAsync(client => client.Login == login);

            return maybeClient;
        }

        public ValueTask<string> LoginAsync(string login, string password)
        {
            throw new System.NotImplementedException();
        }

        public async ValueTask<Client> InsertClientAsync(Client client) =>
            await InsertAsync(client);

        public IQueryable<Client> SelectAllClients()
        {
            var clients = SelectAll<Client>().Include(a => a.Accounts);

            return clients;
        }

        public async ValueTask<Client> SelectClientByIdAsync(Guid clientId)
        {
            var clientWithAccounts = Clients
                .Include(c => c.Accounts)
                .FirstOrDefault(c => c.ClientId == clientId);

            return await ValueTask.FromResult(clientWithAccounts);
        }

        public async ValueTask<Client> UpdateClientAsync(Client client) =>
            await UpdateAsync(client);

        public async ValueTask<Client> DeleteClientAsync(Client client) =>
            await DeleteAsync(client);

        public string CreateToken(Client client)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, client.ClientId.ToString()),
                new Claim(ClaimTypes.Name, client.Login)
            };

            var appSettingsToken = this.configuration.GetSection("AppSettings:Token").Value;
            if (appSettingsToken is null)
            {
                throw new Exception("AppSettings Token is null.");
            }

            SymmetricSecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                .GetBytes(appSettingsToken));

            SigningCredentials signingCredentials = 
                new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = signingCredentials
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(securityToken);
        }
    }
}
