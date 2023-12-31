﻿//===========================
// Copyright (c) Tarteeb LLC
// Manage Your Money Easy
//===========================

using EasyPay.Api.Models.Accounts;
using EasyPay.Api.Models.Accounts.Exceptions;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xeptions;

namespace EasyPay.Api.Services.Foundations.Accounts
{
    public partial class AccountService
    {
        private delegate ValueTask<Account> ReturningAccountFunction();
        private delegate IQueryable<Account> ReturningAccountsFunction();

        private async ValueTask<Account> TryCatch(ReturningAccountFunction returningAccountFunction)
        {
            try
            {
                return await returningAccountFunction();
            }
            catch (NullAccountException accountNotNull)
            {
                throw CreateAndLogValidationException(accountNotNull);
            }
            catch (InvalidAccountException invalidAccountException)
            {
                throw CreateAndLogValidationException(invalidAccountException);
            }
            catch (NotFoundAccountException notFoundAccountException)
            {
                throw CreateAndLogValidationException(notFoundAccountException);
            }
            catch (NotFoundAccountByAccountNumberException notFoundAccountByAccountNumberException)
            {
                throw CreateAndLogValidationException(notFoundAccountByAccountNumberException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsAccountException =
                    new AlreadyExistsAccountException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsAccountException);
            }
            catch (SqlException sqlException)
            {
                var failedStorageAccountException =
                    new FailedStorageAccountException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedStorageAccountException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedAccountException =
                    new LockedAccountException(dbUpdateConcurrencyException);

                throw CreateAndLogDependencyValidationException(lockedAccountException);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedStorageAccountException =
                    new FailedStorageAccountException(dbUpdateException);

                throw CreateAndLogDependencyException(failedStorageAccountException);
            }
            catch (Exception exception)
            {
                var failedAccountServiceException =
                    new FailedAccountServiceException(exception);

                throw CreateAndLogAccountServiceException(failedAccountServiceException);
            }
        }

        private IQueryable<Account> TryCatch(ReturningAccountsFunction returningAccountsFunction)
        {
            try
            {
                return returningAccountsFunction();
            }
            catch (SqlException sqlException)
            {
                var failedStorageAccountException = new FailedStorageAccountException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedStorageAccountException);
            }
            catch (Exception exception)
            {
                var failedAccountServiceException = new FailedAccountServiceException(exception);

                throw CreateAndLogAccountServiceException(failedAccountServiceException);
            }
        }

        private AccountValidationException CreateAndLogValidationException(Xeption exception)
        {
            var accountValidationException = new AccountValidationException(exception);
            this.loggingBroker.LogError(accountValidationException);

            return accountValidationException;
        }

        private AccountDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
        {
            var accountDependencyValidationException = new AccountDependencyValidationException(exception);
            this.loggingBroker.LogError(accountDependencyValidationException);

            return accountDependencyValidationException;
        }

        private AccountDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var accountDependencyException = new AccountDependencyException(exception);
            this.loggingBroker.LogError(accountDependencyException);

            return accountDependencyException;
        }

        private AccountDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var accountDependencyException = new AccountDependencyException(exception);
            this.loggingBroker.LogCritical(accountDependencyException);

            return accountDependencyException;
        }

        private AccountServiceException CreateAndLogAccountServiceException(Xeption exception)
        {
            var accountServiceException = new AccountServiceException(exception);
            this.loggingBroker.LogError(accountServiceException);

            return accountServiceException;
        }
    }
}
