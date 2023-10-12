﻿//===========================
// Copyright (c) Tarteeb LLC
// Manage Your Money Easy
//===========================

using EasyPay.Api.Models.Clients;
using EasyPay.Api.Models.Clients.Exceptions;
using System;

namespace EasyPay.Api.Services.Foundations.Clients
{
    public partial class ClientService
    {
        private void ValidateClientOnAdd(Client client)
        {
            ValidateClientNotNull(client);

            Validate(
                (Rule: IsInvalid(client.ClientId), Parameter: nameof(Client.ClientId)),
                (Rule: IsInvalid(client.FirstName), Parameter: nameof(Client.FirstName)),
                (Rule: IsInvalid(client.LastName), Parameter: nameof(Client.LastName)),
                (Rule: IsInvalid(client.BirthDate), Parameter: nameof(Client.BirthDate)),
                (Rule: IsInvalid(client.Email), Parameter: nameof(Client.Email)),
                (Rule: IsInvalid(client.PhoneNumber), Parameter: nameof(Client.PhoneNumber)),
                (Rule: IsInvalid(client.Address), Parameter: nameof(Client.Address)));

            //Validate(
            //    (Rule: IsLessThen14(client.BirthDate), Parameter: nameof(Client.BirthDate)),
            //    (Rule: IsNotRecent(client.BirthDate), Parameter: nameof(Client.BirthDate)));
        }

        private static dynamic IsInvalid(Guid clientId) => new
        {
            Condition = clientId == default,
            Message = "Id is required"
        };

        private static dynamic IsInvalid(string text) => new
        {
            Condition = String.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is required"
        };

        private dynamic IsLessThen14(DateTimeOffset date) => new
        {
            Condition = IsAgeLessThen14(date),
            Message = "Age is less than 14"
        };

        private bool IsAgeLessThen14(DateTimeOffset date)
        {
            DateTimeOffset now = this.dateTimeBroker
                .GetCurrentDateTimeOffset();
            int age = (now - date).Days / 365;

            return age < 14;
        }

        private dynamic IsNotRecent(DateTimeOffset date) => new
        {
            Condition = IsDateNotRecent(date),
            Message = "Date is not recent"
        };

        private bool IsDateNotRecent(DateTimeOffset date)
        {
            DateTimeOffset currentDateTime = this
                .dateTimeBroker.GetCurrentDateTimeOffset();
            TimeSpan timeDifference = currentDateTime.Subtract(date);

            return timeDifference.TotalSeconds is > 70 or < 0;
        }

        private static void ValidateClientNotNull(Client client)
        {
            if (client == null)
            {
                throw new NullClientException();
            }
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidClientException = new InvalidClientException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidClientException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidClientException.ThrowIfContainsErrors();
        }
    }
}
