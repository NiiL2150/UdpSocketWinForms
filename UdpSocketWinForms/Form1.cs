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

namespace UdpSocketWinForms
{
    public partial class Form1 : Form
    {
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        EndPoint point;
        List<Product> products = new List<Product>()
        {
            new Product("AMD 5600X", 400),
            new Product("RTX 3090", 10000),
            new Product("24K GOLD", 9999)
        };

        void SyncListBox()
        {
            listBoxItems.Items.Clear();
            foreach (Product product in products)
            {
                listBoxItems.Items.Add(product.Name);
            }
        }

        public Form1()
        {
            InitializeComponent();
            SyncListBox();
            point = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 51234);
            socket.Bind(point);
            socket.SendTimeout = 6000;
            socket.ReceiveTimeout = 6000;
            Task.Run(Listen);
        }

        private void listBoxItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            labelPrice.Text = $"Price: {products.ElementAt(listBoxItems.SelectedIndex).Price}";
        }

        async void Listen()
        {
            while (true)
            {
                byte[] receiveBuffer = new byte[1024];
                IPEndPoint client = new IPEndPoint(IPAddress.Any, 61234);
                var receive = await socket.ReceiveFromAsync(receiveBuffer, SocketFlags.None, client);
                client = (IPEndPoint)receive.RemoteEndPoint;
                string result = Encoding.UTF8.GetString(receiveBuffer);
                result = result.Trim(' ', '\0', '\n', '\r');
                Product? product = products.FirstOrDefault(p => p.Name.ToLower() == result.ToLower());
                textBoxLog.Text += $"{client.Address}: {result}{Environment.NewLine}";

                if (product != null)
                {
                    string info = $"{product.Name} - {product.Price}$";
                    byte[] sendBuffer = Encoding.UTF8.GetBytes(info);
                    await socket.SendToAsync(sendBuffer, SocketFlags.None, client);
                    textBoxLog.Text += $"Sent {info} to {client.Address}{Environment.NewLine}";
                }
            }
        }
    }
}
