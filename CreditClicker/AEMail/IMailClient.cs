using System;

namespace gmail
{
    public interface IMailClient : IDisposable
    {
        int GetMessageCount();

        MailMessage GetMessage(int index, bool headersonly = false);

        MailMessage GetMessage(string uid, bool headersonly = false);

        void Disconnect();
    }
}