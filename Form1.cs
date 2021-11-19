using SimpleTcp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerServer
{
    public partial class Form1 : Form
    {
        public int Kord;
        public int[,] field = new int[4, 4];
        public bool client1Ready;
        public bool client2Ready;
        private byte[] transmission;
        public int c = 1;
        public int tmp = 64;
        public string str;
        public string tmp_str;
        public Form1()
        {
            InitializeComponent();
        }
        SimpleTcpServer server;

        private void Form1_Load(object sender, EventArgs e)
        {
            btnSend.Enabled = false;
            client1Ready = false;
            client2Ready = false;
            server = new SimpleTcpServer(txtIP.Text);
            server.Events.ClientConnected += Events_ClientConnected;
            server.Events.ClientDisconnected += Events_ClientDisconnected;
            server.Events.DataReceived += Events_DataReceived;
        }

        /*
        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"{e.IpPort}:{Encoding.UTF8.GetString(e.Data)}{Environment.NewLine}";
                try 
                { 
                    tmp = Convert.ToInt32(Encoding.UTF8.GetString(e.Data));
                }
                catch
                {
                    tmp_str = Encoding.UTF8.GetString(e.Data);
                    txtInfo.Text += $"Username: {tmp_str}{Environment.NewLine}";
                }
                str = e.IpPort;
                btnSend.PerformClick();
            });

        }
        */
        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"{e.IpPort}:{Encoding.UTF8.GetString(e.Data)}{Environment.NewLine}";
                try
                {
                    transmission = e.Data;
                    Message ms = FromByteArray<Message>(e.Data);
                    if(ms.getType() == 6)
                    {
                        if(client1Ready && client2Ready)
                        {
                            if (ListClientIP.Items.Count == 2)
                            {
                                Message m = new Message(5, "1");
                                BinaryFormatter formatter = new BinaryFormatter();
                                using (var s = new MemoryStream())
                                {
                                    formatter.Serialize(s, m);
                                    transmission = s.ToArray();

                                }
                                str = e.IpPort;
                                btnSend.PerformClick();
                            }
                        } else if(client1Ready){
                            client2Ready = true;
                        } else
                        {
                            client1Ready = true;
                        }
                        return;
                    }
                    //tmp = Convert.ToInt32(Encoding.UTF8.GetString(e.Data));
                }
                catch
                {
                    tmp_str = Encoding.UTF8.GetString(e.Data);
                    txtInfo.Text += $"Username: {tmp_str}{Environment.NewLine}";
                }
                str = e.IpPort;
                btnSend.PerformClick();
            });

        }

        private void Events_ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"{e.IpPort}disconnected.{Environment.NewLine}";
                ListClientIP.Items.Remove(e.IpPort);
            });
        }

        private void Events_ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"{e.IpPort} connected.{Environment.NewLine}";
                ListClientIP.Items.Add(e.IpPort);
            });
        }

        public static T FromByteArray<T>(byte[] data)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream s = new MemoryStream(data))
            {
                object o = formatter.Deserialize(s);
                return (T)o;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            server.Start();
            txtInfo.Text += $"Starting...{Environment.NewLine}";
            btnStart.Enabled = false;
            btnSend.Enabled = true;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            ListClientIP.SetSelected(0, true);
            if (!str.Equals(ListClientIP.SelectedItem))
            {          
                if (server.IsListening)
                {
                    if (ListClientIP.SelectedItem != null)
                    {

                        server.Send(ListClientIP.SelectedItem.ToString(), transmission);
                    } else
                    {
                        int x = 0;
                    }
                }              
            }
            else
            {
                ListClientIP.SetSelected(1, true);
                if (server.IsListening)
                {
                    if (ListClientIP.SelectedItem != null)
                    {
                        server.Send(ListClientIP.SelectedItem.ToString(), transmission);
                    }
                }
                c = 1;
            }
        }
    }
}
