using System;

namespace gmail
{
    public class MessageEventArgs : EventArgs
    {
        public int MessageCount { get; set; }

        internal ImapClient Client { get; set; }
    }
}