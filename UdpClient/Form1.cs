using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UdpClient
{
    public partial class Form1 : Form
    {
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        EndPoint Server
        {
            get => new IPEndPoint(IPAddress.Parse(textBoxIpAddress.Text), 51234);
        }

        public Form1()
        {
            InitializeComponent();
        }

        private async void buttonSendRequest_Click(object sender, EventArgs e)
        {
            byte[] data = Encoding.UTF8.GetBytes(textBoxRequest.Text);
            await socket.SendToAsync(data, SocketFlags.None, Server);

            byte[] receiveBuffer = new byte[1024];
            await socket.ReceiveFromAsync(receiveBuffer, SocketFlags.None, Server);
            string result = Encoding.UTF8.GetString(receiveBuffer);

            textBoxResult.Text += $"{result}";
            textBoxResult.AppendText(Environment.NewLine);
        }
    }
}
