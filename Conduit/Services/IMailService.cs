using System.Threading.Tasks;

namespace Conduit.Services
{
    public interface IMailService
    {
        Task Send(string subject, string message, string name, string email);
    }
}
