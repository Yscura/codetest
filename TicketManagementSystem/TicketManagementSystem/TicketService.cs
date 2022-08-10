using System;

namespace TicketManagementSystem
{
    public class TicketService
    {
        ITicketRepository ticketRepository;
        IUserRepository userRepository;

        public TicketService(ITicketRepository ticketRepository)
        {
            this.ticketRepository = ticketRepository;
            this.userRepository = new UserRepository();
        }

        public TicketService(ITicketRepository ticketRepository, IUserRepository userRepository)
        {
            this.ticketRepository = ticketRepository;
            this.userRepository = userRepository;
        }

       

        public int CreateTicket(string title, Priority prio, string assignedTo, string desc, DateTime date, bool isPayingCustomer)
        {
            // Check if t or desc are null or if they are invalid and throw exception
            if (String.IsNullOrEmpty(title) || String.IsNullOrEmpty(desc))
            {
                throw new InvalidTicketException("Title or description were null");
            }

            User user = null;
            if (!String.IsNullOrEmpty(assignedTo))
            {
                user = userRepository.GetUser(assignedTo);
            }

            if (user == null)
            {
                throw new UnknownUserException("User " + assignedTo + " not found");
            }


            bool containsKeyword = false;
            if ((title.Contains("Crash") || title.Contains("Important") || title.Contains("Failure")))
            {
                containsKeyword = true;
            }
            // Raise priority if ticket is older than 1h or if the title contains some special words
            if (date < DateTime.UtcNow - (TimeSpan.FromHours(1)) || containsKeyword)
            {
                if (prio == Priority.Low)
                {
                    prio = Priority.Medium;
                }
                else if (prio == Priority.Medium)
                {
                    prio = Priority.High;
                }
            }


            double price = 0;
            User accountManager = null;
            if (isPayingCustomer)
            {
                // Only paid customers have an account manager.

                accountManager = userRepository.GetAccountManager();
                if (prio == Priority.High)
                {
                    price = 100;
                }
                else
                {
                    price = 50;
                }
            }

            // Create the tickket
            var ticket = new Ticket()
            {
                Title = title,
                AssignedUser = user,
                Priority = prio,
                Description = desc,
                Created = date,
                PriceDollars = price,
                AccountManager = accountManager
            };

            var id = ticketRepository.CreateTicket(ticket);

            // Return the id
            return id;
        }

        public void AssignTicket(int id, string username)
        {
            User user = null;
            if (!String.IsNullOrEmpty(username))
            {
                user = userRepository.GetUser(username);
            }

            if (user == null)
            {
                throw new UnknownUserException("User not found");
            }

            var ticket = ticketRepository.GetTicket(id);

            if (ticket == null)
            {
                throw new ApplicationException("No ticket found for id " + id);
            }

            ticket.AssignedUser = user;

            ticketRepository.UpdateTicket(ticket);
        }

    }

    public enum Priority
    {
        High,
        Medium,
        Low
    }
}
