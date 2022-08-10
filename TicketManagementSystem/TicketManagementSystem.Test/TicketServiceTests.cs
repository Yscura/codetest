using System;
using Moq;
using NUnit.Framework;

namespace TicketManagementSystem.Test
{
    public class Tests
    {
        private TicketService ticketService;
        private Mock<ITicketRepository> ticketRepositoryMock;
        private Mock<IUserRepository> urMock;

        [SetUp]
        public void Setup()
        {
            ticketRepositoryMock = new Mock<ITicketRepository>();

            urMock = new Mock<IUserRepository>();
            ticketService = new TicketService(ticketRepositoryMock.Object);
        }

        [Test]
        public void ShallThrowExceptionIfTitleIsNull()
        {
            Assert.That(() => ticketService.CreateTicket(null, Priority.High, "jim", "high prio ticket", DateTime.Now, false), Throws.InstanceOf<InvalidTicketException>().With.Message.EqualTo("Title or description were null"));
        }

        [Test]
        public void ShallThrowExceptionIfUserIsNull()
        {
            Assert.That(() => ticketService.CreateTicket("t", Priority.High, null, "high prio ticket", DateTime.Now, false), Throws.InstanceOf<UnknownUserException>().With.Message.EqualTo("User  not found"));
        }

        [Test]
        public void ShallThrowExceptionIfUserIsNotFound()
        {
            Assert.That(() => ticketService.CreateTicket("t", Priority.High, "me", "high prio ticket", DateTime.Now, false), Throws.InstanceOf<UnknownUserException>().With.Message.EqualTo("User me not found"));
        }

        [Test]
        public void ShallCreateTicket()
        {
            const string title = "MyTicket";
            const Priority prio = Priority.High;
            const string assignedTo = "jsmith";
            const string description = "This is a high ticket"; 
            DateTime when = DateTime.Now;

            ticketService.CreateTicket(title, prio, assignedTo, description, when, false);

            ticketRepositoryMock.Verify(a => a.CreateTicket(It.Is<Ticket>(t =>
                t.Title == title && 
                t.Priority == Priority.High && 
                t.Description == description &&
                t.AssignedUser.Username == assignedTo && 
                t.Created == when)));
        }

        [Test]
        public void ShallRaisePrio()
        {
            const string title = "MyImportantTicket";
            const Priority prio = Priority.Medium;
            const string assignedTo = "jsmith";
            const string description = "This is an Important ticket"; 
            DateTime when = DateTime.Now;

            ticketService.CreateTicket(title, prio, assignedTo, description, when, false);

            ticketRepositoryMock.Verify(a => a.CreateTicket(It.Is<Ticket>(t =>
                t.Title == title && 
                t.Priority == Priority.High && 
                t.Description == description &&
                t.AssignedUser.Username == assignedTo && 
                t.Created == when)));
        }

        [Test]
        public void ShallGetUser()
        {
            const string title = "MyTicket";
            const Priority prio = Priority.Medium;
            const string assignedTo = "jsmith";
            const string description = "This is a ticket"; 
            DateTime when = DateTime.Now;

            urMock.Setup(x => x.GetUser(It.IsAny<string>())).Returns(new User()).Verifiable();

            ticketService = new TicketService(ticketRepositoryMock.Object, urMock.Object);
            ticketService.CreateTicket(title, prio, assignedTo, description, when, false);
           
            urMock.Verify();
        }

    }
}