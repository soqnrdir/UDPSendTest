using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class form1 : Form
    {
        public int seq = 1;

        Regex regex = new Regex(@"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$");

        // (1) UdpClient 객체 성성
        UdpClient udp = new UdpClient();
        UdpClient receiveUdp; // Port Number

        string data = "";

        public DateTime localTime;
        public DateTime localDate;

        string sendIp = "10.128.1.203";
        // string receiveIp = "10.128.100.233";
        string receiveIp = "";
        string PortNo = "17224";

        public form1()
        {
            InitializeComponent();

            // IPEndPoint multicastEP = new IPEndPoint(IPAddress.Parse("10.128.101.145"), 5500);

            //int ifIndex = 4;
            //udp.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, (int)IPAddress.HostToNetworkOrder(ifIndex));
            //System.Diagnostics.Debug.WriteLine("ifIndex: " + ifIndex);

            seqVal.Text = seq.ToString();

            sendAddr.Text = sendIp;
            receiveAddr.Text = receiveIp;
            portNo.Text = PortNo;

        }

        private byte toBCD(int val)
        {
            return (byte)(((val / 10) << 4) | (val % 10));
        }
        private byte getFireByte0()
        {
            byte val = 0;
            if (chkFire0.Checked)
                val |= 0x01;
            if (chkFire1.Checked)
                val |= 0x02;
            if (chkFire2.Checked)
                val |= 0x04;
            if (chkFire3.Checked)
                val |= 0x08;
            if (chkFire4.Checked)
                val |= 0x10;
            if (chkFire5.Checked)
                val |= 0x20;
            if (chkFire6.Checked)
                val |= 0x40;
            if (chkFire7.Checked)
                val |= 0x80;
            return val;
        }
        private byte getFireByte1()
        {
            byte val = 0;
            if (chkFire8.Checked)
                val |= 0x02;
            if (chkFire9.Checked)
                val |= 0x04;
            return val;
        }

        private byte getPAByte0()
        {
            byte val = 0;
            if (chkPA0.Checked)
                val |= 0x01;
            if (chkPA1.Checked)
                val |= 0x02;
            if (chkPA2.Checked)
                val |= 0x04;
            if (chkPA3.Checked)
                val |= 0x08;
            if (chkPA4.Checked)
                val |= 0x10;
            if (chkPA5.Checked)
                val |= 0x20;
            if (chkPA6.Checked)
                val |= 0x40;
            if (chkPA7.Checked)
                val |= 0x80;
            return val;
        }
        private byte getPAByte1()
        {
            byte val = 0;
            if (chkPA8.Checked)
                val |= 0x02;
            if (chkPA9.Checked)
                val |= 0x04;
            return val;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (seqVal  .Text.Length == 0)
            {
                seqVal.Focus();
                MessageBox.Show("Seq값을 입력해주세요.");
                return;
            }
            if (receiveAddr.Text.Length == 0)
            {
                receiveAddr.Focus();
                MessageBox.Show("수신지를 입력해주세요.");
                return;
            }
            if (sendAddr.Text.Length == 0)
            {
                sendAddr.Focus();
                MessageBox.Show("송신지를 입력해주세요.");
                return;
            }
            if (portNo.Text.Length == 0)
            {
                portNo.Focus();
                MessageBox.Show("Port를 입력해주세요.");
                return;
            }
            
            byte[] dgram = new byte[42];

            dgram[0] = (byte)((seq >> 24) & 0xFF);
            dgram[1] = (byte)((seq >> 16) & 0xFF);
            dgram[2] = (byte)((seq >> 8) & 0xFF);
            dgram[3] = (byte)((seq >> 0) & 0xFF);

            bool isIPAddr = false;

            if (regex.IsMatch(receiveAddr.Text) && regex.IsMatch(sendAddr.Text))
            {
                isIPAddr = true;
            } else
            {
                MessageBox.Show("IP형식에 맞게 입력해 주세요. (x.x.x.x)");
                return;
            }
            Console.WriteLine(isIPAddr);
            if(isIPAddr)
            {
                string[] sendIpS = sendAddr.Text.Split('.');
                string[] receiveIpS = receiveAddr.Text.Split('.');


                dgram[4] = (byte)Int32.Parse(sendIpS[0]);
                dgram[5] = (byte)Int32.Parse(sendIpS[1]);
                dgram[6] = (byte)Int32.Parse(sendIpS[2]);
                dgram[7] = (byte)Int32.Parse(sendIpS[3]);

                dgram[8] = (byte)Int32.Parse(receiveIpS[0]);
                dgram[9] = (byte)Int32.Parse(receiveIpS[1]);
                dgram[10] = (byte)Int32.Parse(receiveIpS[2]);
                dgram[11] = (byte)Int32.Parse(receiveIpS[3]);
            }

            dgram[12] = 0x20;

            localTime = DateTime.Now;
            localDate = DateTime.Now;

            dgram[13] = toBCD(localDate.Year % 100);
            dgram[14] = toBCD(localDate.Month);
            dgram[15] = toBCD(localDate.Day);

            dgram[16] = toBCD(localDate.Hour);
            dgram[17] = toBCD(localDate.Minute);
            dgram[18] = toBCD(localDate.Second);

            dgram[19] = 1;
            dgram[20] = 0x31;
            dgram[21] = 100;
            dgram[22] = 150;
            dgram[23] = 200;
            dgram[24] = 01;
            dgram[23] = 0x4D;
            dgram[24] = 0x5D;
            dgram[25] = 0x31;
            dgram[26] = 100;
            dgram[27] = 150;
            dgram[28] = 200;
            dgram[29] = 01;
            dgram[30] = getFireByte0();
            dgram[31] = getFireByte1();
            dgram[32] = getPAByte0();
            dgram[33] = getPAByte1();
            dgram[34] = 0x4D;
            dgram[35] = 0x00;
            dgram[36] = 0x00;
            dgram[37] = 0x00;
            dgram[38] = 0x00;
            dgram[39] = 0x00;
            dgram[40] = 0x00;
            dgram[41] = 0x00;


            IPEndPoint multicastEP = new IPEndPoint(IPAddress.Parse("10.128.100.234"), Int32.Parse(portNo.Text));

            udp.Send(dgram, dgram.Length, multicastEP);

            Console.WriteLine("SEND: " + BitConverter.ToString(dgram).Replace("-", ""));

            //Console.WriteLine(dgramStr);

            // byte[] bytes = udp.Receive(ref multicastEP);
            // Thread.Sleep(1000);
            // Console.WriteLine("[Receive] {0} 로부터 {1} 바이트 수신", dgram.ToString(), bytes.Length);

            seq = Int32.Parse(seqVal.Text) + 1;
            seqVal.Text = seq.ToString();
           // receiveMessage.Text = "[Receive] {0} 로부터 {1} 바이트 수신" + dgram.ToString();

           // udp.Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {

            if (seqVal.Text.Length == 0)
            {
                seqVal.Focus();
                MessageBox.Show("Seq값을 입력해주세요.");
                return;
            }
            if (receiveAddr.Text.Length == 0)
            {
                receiveAddr.Focus();
                MessageBox.Show("수신지를 입력해주세요.");
                return;
            }
            if (sendAddr.Text.Length == 0)
            {
                sendAddr.Focus();
                MessageBox.Show("송신지를 입력해주세요.");
                return;
            }
            if (portNo.Text.Length == 0)
            {
                portNo.Focus();
                MessageBox.Show("Port를 입력해주세요.");
                return;
            }

            try
            {   
                receiveUdp = new UdpClient(5500);

                receiveUdp.BeginReceive(new AsyncCallback(recv), null);
                btnReceive.Enabled = false;
                receiveMessage.Text += "Listening... \r\n";
                
            }
            catch (Exception ex)
            {
                receiveMessage.Text += ex.Message.ToString();


            }
        }

        void recv(IAsyncResult res)

        {
            IPEndPoint RemoteIP = new IPEndPoint(IPAddress.Any, 17224);

            byte[] received = receiveUdp.EndReceive(res, ref RemoteIP);

            data = Encoding.UTF8.GetString(received);

            // to avoid cross-threading we use Method Invoker

            this.Invoke(new MethodInvoker(delegate

            {   
                receiveMessage.Text += data + "\r\n";
                // focus가 맨 아래로
                receiveMessage.SelectionStart = receiveMessage.Text.Length;
                receiveMessage.ScrollToCaret();
            }));

            receiveUdp.BeginReceive(new AsyncCallback(recv), null);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
    }
}