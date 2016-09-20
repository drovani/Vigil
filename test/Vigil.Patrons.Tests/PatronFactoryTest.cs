﻿using Moq;
using System;
using Vigil.Domain;
using Vigil.MessageQueue;
using Vigil.MessageQueue.Commands;
using Xunit;

namespace Vigil.Patrons
{
    public class PatronFactoryTest
    {
        [Fact]
        public void User_Can_Create_New_Patron()
        {
            var queue = new Mock<ICommandQueue>(MockBehavior.Strict);
            queue.Setup(q => q.QueueCommand(It.IsAny<ICommand>(), It.IsAny<IKeyIdentity>())).Verifiable();
            PatronFactory factory = new PatronFactory(queue.Object);

            FactoryResult result = factory.CreatePatron(new CreatePatronCommand()
            {
                DisplayName = "Test User",
                IsAnonymous = false,
                PatronType = "Test Account"
            });

            queue.VerifyAll();
            Assert.NotEqual(Guid.Empty, result.AffectedEntity.Id);
            Assert.Empty(result.ValidationResults);
        }

        [Fact]
        public void User_Cannot_Create_Patron_That_Fails_Validation()
        {
            var queue = new Mock<ICommandQueue>(MockBehavior.Strict);
            queue.Setup(q => q.QueueCommand(It.IsAny<ICommand>(), It.IsAny<IKeyIdentity>())).Verifiable();
            PatronFactory factory = new PatronFactory(queue.Object);

            FactoryResult result = factory.CreatePatron(new CreatePatronCommand());

            queue.Verify(q => q.QueueCommand(It.IsAny<ICommand>(), It.IsAny<IKeyIdentity>()), Times.Never);
            Assert.Null(result.AffectedEntity);
            Assert.NotEmpty(result.ValidationResults);
        }
    }
}
