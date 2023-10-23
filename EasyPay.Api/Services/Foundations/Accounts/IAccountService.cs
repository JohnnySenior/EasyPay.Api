﻿//===========================
// Copyright (c) Tarteeb LLC
// Manage Your Money Easy
//===========================

using EasyPay.Api.Models.Accounts;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPay.Api.Services.Foundations.Accounts
{
    public interface IAccountService
    {
        /// <exception cref="Models.Accounts.Exceptions.AccountValidationException"></exception>
        /// <exception cref="Models.Accounts.Exceptions.AccountDependencyValidationException"></exception>
        /// <exception cref="Models.Accounts.Exceptions.AccountDependencyException"></exception>
        /// <exception cref="Models.Accounts.Exceptions.AccountServiceException"></exception>
        ValueTask<Account> AddAccountAsync(Account account);
        /// <exception cref="Models.Accounts.Exceptions.AccountDependencyException"></exception>
        /// <exception cref="Models.Accounts.Exceptions.AccountServiceException"></exception>   
        IQueryable<Account> RetrieveAllAccounts();
        /// <exception cref="Models.Accounts.Exceptions.AccountDependencyException"></exception>
        /// <exception cref="Models.Accounts.Exceptions.AccountServiceException"></exception>   
        ValueTask<Account> RetrieveAccountByIdAsync(Guid accountId);
        /// <exception cref="Models.Accounts.Exceptions.AccountDependencyValidationException"></exception>
        /// <exception cref="Models.Accounts.Exceptions.AccountDependencyException"></exception>
        /// <exception cref="Models.Accounts.Exceptions.AccountServiceException"></exception>
        ValueTask<Account> RemoveAccountByIdAsync(Guid accountId);
        /// <exception cref="Models.Accounts.Exceptions.AccountValidationException"></exception>
        /// <exception cref="Models.Accounts.Exceptions.AccountDependencyValidationException"></exception>
        /// <exception cref="Models.Accounts.Exceptions.AccountDependencyException"></exception>
        /// <exception cref="Models.Accounts.Exceptions.AccountServiceException"></exception>
        ValueTask<Account> ModifyAccountAsync(Account account);
    }
}
