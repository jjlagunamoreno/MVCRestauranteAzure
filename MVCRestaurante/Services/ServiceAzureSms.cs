using Azure.Communication.Sms;
using System.Threading.Tasks;

namespace MVCRestaurante.Services
{
    public class ServiceAzureSms
    {
        private readonly SmsClient _smsClient;
        private readonly string _fromPhoneNumber;

        public ServiceAzureSms(string connectionString, string fromPhoneNumber)
        {
            _smsClient = new SmsClient(connectionString);
            _fromPhoneNumber = fromPhoneNumber;
        }

        public async Task SendSmsAsync(string toPhoneNumber, string message)
        {
            await _smsClient.SendAsync(
                from: string.IsNullOrEmpty(_fromPhoneNumber) ? toPhoneNumber : _fromPhoneNumber,
                to: toPhoneNumber,
                message: message
            );
        }
    }
}
