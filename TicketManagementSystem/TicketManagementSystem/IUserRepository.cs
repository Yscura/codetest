namespace TicketManagementSystem;

public interface IUserRepository
{
    User GetUser(string username);

    User GetAccountManager();
}