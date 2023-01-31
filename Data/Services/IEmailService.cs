using PaypalLab.Models;
using PaypalLab.Models;
using SendGrid;
using System.Threading.Tasks;

namespace PaypalLab.Data.Services
{
    public interface IEmailService
    {
        Task<Response> SendSingleEmail(ComposeEmailModel payload);
    }
}
