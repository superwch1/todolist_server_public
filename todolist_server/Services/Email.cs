using MailKit.Net.Smtp;
using MimeKit;

namespace todolist_server.Services
{
    public class Email
    {
        private readonly string _email;
        private readonly string _password;
        private readonly string _port;
        private readonly string _host;
        public Email(IConfiguration configuration)
        {
            _email = configuration["GoDaddySMTP:email"]!;
            _password = configuration["GoDaddySMTP:password"]!;
            _port = configuration["GoDaddySMTP:port"]!;
            _host = configuration["GoDaddySMTP:host"]!;
        }


        public async Task Welcome(string username, string receiverEmail)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("noreply", _email));
            message.To.Add(new MailboxAddress($"\"{username}\"", receiverEmail));
            message.Subject = "Welcome to ToDoList communicty";

            message.Body = new TextPart("html")
            {
                Text =
                $@"
                <!DOCTYPE html> 
                <html>
                    <body style=""font-size: 16px;"">
                        <p style=""color: #000000"">Welcome to join the todolist community</p>
                        <p style=""color: #000000"">If you need any help, feel free to contact us.</p>
                    </body>
                </html>
                "
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_host, Convert.ToInt32(_port), true);
                await client.AuthenticateAsync(_email, _password);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }


        public async Task PasswordReset(string username, string receiverEmail, string passcode)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("noreply", _email));
            message.To.Add(new MailboxAddress($"\"{username}\"", receiverEmail));
            message.Subject = "Reset Password";

            message.Body = new TextPart("html")
            {
                Text =
                $@"
                <!DOCTYPE html> 
                <html>
                    <body style=""font-size: 16px;"">
                        <p style=""color: #000000"">Passcode:</p>
                        <h3 style=""text-decoration: none; padding: 10px; "">{passcode}</h3>
                        <p style=""color: #000000"">If you didn't request a password reset, please ignore this email and let us know.</p>
                    </body>
                </html>
                "
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_host, Convert.ToInt32(_port), true);
                await client.AuthenticateAsync(_email, _password);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
