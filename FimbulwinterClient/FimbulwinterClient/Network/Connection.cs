using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace FimbulwinterClient.Network
{
    public class Connection
    {
        private TcpClient m_client;
        public TcpClient Client
        {
            get { return m_client; }
        }

        private NetworkStream m_ns;
        public NetworkStream Stream
        {
            get { return m_ns; }
        }

        private BinaryWriter m_bw;
        public BinaryWriter BinaryWriter
        {
            get { return m_bw; }
        }

        private PacketSerializer m_pserial;
        public PacketSerializer PacketSerializer
        {
          get { return m_pserial; }
          set { m_pserial = value; }
        }

        private byte[] receiveBuffer;

        public Connection()
        {
            m_client = new TcpClient();
            m_pserial = new PacketSerializer();
            receiveBuffer = new byte[16 * 1024];
        }

        public void Connect(string target, int port)
        {
            if (m_client.Connected)
                Disconnect();

            m_client.Connect(target, port);

            m_ns = m_client.GetStream();
            m_bw = new BinaryWriter(m_ns);
        }

        public void Disconnect()
        {
            if (m_client.Connected)
            {
                try
                {
                    m_client.Close();
                    m_client.Client.Dispose();
                }
                catch
                {

                }

                m_client = new TcpClient();
                m_pserial.Reset();
            }
        }

        public void Start()
        {
            SocketError err;

            m_client.Client.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, out err, ReadComplete, null);
        }

        private void ReadComplete(IAsyncResult ar)
        {
            SocketError err;

            int size = m_client.Client.EndReceive(ar, out err);

            if (size == 0)
            {
                if (Disconnected != null)
                    Disconnected();
            }
            else
            {
                m_pserial.EnqueueBytes(receiveBuffer, size);
            }

            Start();
        }

        public void SendPacket(OutPacket op)
        {
            op.Write(m_bw);
        }

        public event Action Disconnected;
    }
}
