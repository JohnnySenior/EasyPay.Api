﻿//===========================
// Copyright (c) Tarteeb LLC
// Manage Your Money Easy
//===========================

using EasyPay.Api.Brokers.DateTimes;
using EasyPay.Api.Brokers.Loggings;
using EasyPay.Api.Brokers.Storages;
using EasyPay.Api.Models.Transfers;
using System;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace EasyPay.Api.Services.Foundations.Transfers
{
    public partial class TransferService : ITransferService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public TransferService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Transfer> MakeAndInsertTransferAsync(
            string sourceAccountNumber, string receiverAccountNumber, decimal amount) =>
        TryCatch(async () =>
        {
            Transfer transfer = new Transfer
            {
                TransferId = Guid.NewGuid(),
                Amount = amount,
                ReceiverAccountNumber = receiverAccountNumber,
                SourceAccountNumber = sourceAccountNumber,
            };

            ValidateTransferOnAdd(transfer);

            return await this.storageBroker.InsertTransferAsync(transfer);
        });

        public ValueTask<Transfer> RetrieveTransferByIdAsync(Guid transferId) =>
        TryCatch(async () =>
        {
            ValidateTransferId(transferId);

            Transfer maybeTransfer = await this.storageBroker.SelectTransferByIdAsync(transferId);

            return maybeTransfer;
        });

        public IQueryable<Transfer> RetrieveAllTransfers() =>
            TryCatch(() => this.storageBroker.SelectAllTransfers());

        public ValueTask<Transfer> ModifyTransferAsync(Transfer Transfer) =>
        TryCatch(async () =>
        {
            Transfer maybeTransfer =
                await this.storageBroker.SelectTransferByIdAsync(Transfer.TransferId);

            return await this.storageBroker.UpdateTransferAsync(Transfer);
        });

        public ValueTask<Transfer> RemoveTransferByIdAsync(Guid TransferId) =>
         TryCatch(async () =>
         {
             ValidateTransferId(TransferId);

             Transfer maybeTransfer =
                 await this.storageBroker.SelectTransferByIdAsync(TransferId);

             return await this.storageBroker.DeleteTransferAsync(maybeTransfer);
         });
    }
}
