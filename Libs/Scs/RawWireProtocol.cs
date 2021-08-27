using System.Collections.Generic;
using System.IO;
using Hik.Communication.Scs.Communication.Messages;
using Hik.Communication.Scs.Communication.Protocols;

namespace Hik
{
    /// <summary>
    /// RawWireProtocol
    /// </summary>
    public class RawWireProtocol : IScsWireProtocol
    {
        /// <summary>
        /// RawWireProtocol
        /// </summary>
        public RawWireProtocol()
        {
        }

        /// <summary>
        ///     Serializes a message to a byte array to send to remote application.
        ///     This method is synchronized. So, only one thread can call it concurrently.
        /// </summary>
        /// <param name="message">Message to be serialized</param>
        public byte[] GetBytes(IScsMessage message)
        {
            if (message is ScsRawDataMessage)
            {
                var mes = message as ScsRawDataMessage;
                return mes.MessageData;
            }
            return new byte[0];
        }

        /// <summary>
        ///     Builds messages from a byte array that is received from remote application.
        ///     The Byte array may contain just a part of a message, the protocol must
        ///     cumulate bytes to build messages.
        ///     This method is synchronized. So, only one thread can call it concurrently.
        /// </summary>
        /// <param name="receivedBytes">Received bytes from remote application</param>
        /// <returns>
        ///     List of messages.
        ///     Protocol can generate more than one message from a byte array.
        ///     Also, if received bytes are not sufficient to build a message, the protocol
        ///     may return an empty list (and save bytes to combine with next method call).
        /// </returns>
        public IEnumerable<IScsMessage> CreateMessages(byte[] receivedBytes)
        {
            ////Write all received bytes to the _receiveMemoryStream
            //_receiveMemoryStream.Write(receivedBytes, 0, receivedBytes.Length);
            //Create a list to collect messages
            var messages = new List<IScsMessage>();
            var mes = new ScsRawDataMessage(receivedBytes);
            //Console.WriteLine(mes.Text);
            messages.Add(mes);
            //Return message list
            return messages;
        }

        /// <summary>
        ///     This method is called when connection with remote application is reset (connection is renewing or first
        ///     connecting).
        ///     So, wire protocol must reset itself.
        /// </summary>
        public void Reset()
        {
        }
    }
}
