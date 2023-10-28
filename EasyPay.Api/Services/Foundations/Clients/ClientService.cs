//===========================
// Copyright (c) Tarteeb LLC
// Manage Your Money Easy
//===========================

using EasyPay.Api.Brokers.DateTimes;
using EasyPay.Api.Brokers.Loggings;
using EasyPay.Api.Brokers.Storages;
using EasyPay.Api.Models.Clients;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPay.Api.Services.Foundations.Clients
{
    public partial class ClientService : IClientService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public ClientService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask<Client> RegisterClientAsync(Client client, string password)
        {
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            client.PasswordHash = passwordHash;
            client.PasswordSalt = passwordSalt;

            return await this.storageBroker.RegisterAsync(client);
        }

        public async ValueTask<string> LoginClientAsync(string login, string password)
        {
            var maybeClient =  await this.storageBroker.LoginAsync(login);

            if(maybeClient == null)
            {
                throw new Exception("Client not found.");
            }
            else if(!VerifyPasswordHash(password, maybeClient.PasswordHash, maybeClient.PasswordSalt))
            {
                throw new Exception("Wrong password");
            }

            return this.storageBroker.CreateToken(maybeClient);
        }

        public ValueTask<Client> AddClientAsync(Client client) =>
        TryCatch(async () =>
        {
            ValidateClientOnAdd(client);

            return await this.storageBroker.InsertClientAsync(client);
        });

        public ValueTask<Client> RetrieveClientByIdAsync(Guid clientId) =>
        TryCatch(async () =>
        {
            ValidateClientId(clientId);

            Client maybeClient = await this.storageBroker.SelectClientByIdAsync(clientId);

            ValidateStorageClient(maybeClient, clientId);

            return maybeClient;
        });

        public IQueryable<Client> RetrieveAllClients() =>
            TryCatch(() => this.storageBroker.SelectAllClients());

        public ValueTask<Client> ModifyClientAsync(Client client) =>
        TryCatch(async () =>
        {
            ValidateClientOnModify(client);

            Client maybeClient =
                await this.storageBroker.SelectClientByIdAsync(client.ClientId);

            ValidateAgainstStorageClientOnModify(client, maybeClient);

            return await this.storageBroker.UpdateClientAsync(client);
        });

        public ValueTask<Client> RemoveClientByIdAsync(Guid clientId) =>
         TryCatch(async () =>
         {
             ValidateClientId(clientId);

             Client maybeclient =
                 await this.storageBroker.SelectClientByIdAsync(clientId);

             ValidateStorageClient(maybeclient, clientId);

             return await this.storageBroker.DeleteClientAsync(maybeclient);
         });

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
