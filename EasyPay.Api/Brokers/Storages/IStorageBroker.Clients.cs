//===========================
// Copyright (c) Tarteeb LLC
// Manage Your Money Easy
//===========================

using EasyPay.Api.Models.Clients;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPay.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Client> LoginAsync(string login);
        ValueTask<Client> RegisterAsync(Client client);
        string CreateToken(Client client);
        ValueTask<Client> InsertClientAsync(Client client);
        IQueryable<Client> SelectAllClients();
        ValueTask<Client> SelectClientByIdAsync(Guid clientId);
        ValueTask<Client> UpdateClientAsync(Client client);
        ValueTask<Client> DeleteClientAsync(Client client);
    }
}
