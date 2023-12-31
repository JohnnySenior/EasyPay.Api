﻿//===========================
// Copyright (c) Tarteeb LLC
// Manage Your Money Easy
//===========================

using EasyPay.Api.Models.Clients;
using EasyPay.Api.Models.Clients.Exceptions;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace EasyPay.Api.Tests.Unit.Services.Foundations.Clients
{
    public partial class ClientServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveIfIdIsInvalidAndLogItAsync()
        {
            // given 
            Guid invalidClientId = Guid.Empty;

            var invalidClientException =
                new InvalidClientException();

            invalidClientException.AddData(
                key: nameof(Client.ClientId),
                values: "Id is required");

            var expectedClientValidationException =
                new ClientValidationException(invalidClientException);

            // when
            ValueTask<Client> removeClientById =
                this.clientService.RemoveClientByIdAsync(invalidClientId);

            ClientValidationException actualClientValidationException =
                await Assert.ThrowsAsync<ClientValidationException>(
                    removeClientById.AsTask);
            // then
            actualClientValidationException.Should()
                .BeEquivalentTo(expectedClientValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedClientValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectClientByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.storageBrokerMock.Verify(broker =>
            broker.DeleteClientAsync(It.IsAny<Client>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRemoveClientByIdIsNotFoundAndLogItAsync()
        {
            // given
            Guid inputClientId = Guid.NewGuid();
            Client noClient = null;

            var notFoundClientException =
                new NotFoundClientException(inputClientId);

            var expectedClientValidationException =
                new ClientValidationException(notFoundClientException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectClientByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(noClient);

            // when
            ValueTask<Client> removeClientById =
                this.clientService.RemoveClientByIdAsync(inputClientId);

            var actualClientValidationException =
                await Assert.ThrowsAsync<ClientValidationException>(
                    removeClientById.AsTask);

            // then
            actualClientValidationException.Should().BeEquivalentTo(expectedClientValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectClientByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedClientValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteClientAsync(It.IsAny<Client>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
