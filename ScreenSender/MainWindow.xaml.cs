using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ScreenShotHelper;

namespace ScreenSender
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private const int PACKAGE_SIZE = 512;

        private bool IsWorking;

        public Socket Socket { get; set; }

        public EndPoint EndPoint { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            this.Title = "Sender - Pause";
            this.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.EndPoint = new IPEndPoint(IPAddress.Loopback, 10_000);
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.IsWorking == false)
            {
                this.Title = "Sender - Sending";
                startBtn.Content = "Стоп";
                this.IsWorking = true;
                Sender();
            }
            else
            {
                this.Title = "Sender - Pause";
                startBtn.Content = "Старт";
                this.IsWorking = false;
            }

            //System.Windows.Controls.Image img = new System.Windows.Controls.Image();
            //img.Source = CreateBitmapSourceFromBitmap(GetScreen());
            //stackPanel.Children.Add(img);
        }

        private void Sender()
        {
            Task.Run(new Action(() =>
            {
                while (this.IsWorking == true)
                {
                    byte[] buff = ScreenShotHelper.ScreenShotHelper.GetBytesFromScreen();

                    int totalLength = buff.Length;
                    int total = 0;

                    while (true)
                    {
                        if (total < totalLength - PACKAGE_SIZE)
                        {
                            Socket.SendTo(buff, total, PACKAGE_SIZE, SocketFlags.None, EndPoint);
                            total += PACKAGE_SIZE;
                        }
                        else
                        {
                            Socket.SendTo(buff, total, totalLength - total, SocketFlags.None, EndPoint);
                            total += totalLength - total;
                            Socket.SendTo(buff, 0, 0, SocketFlags.None, EndPoint);
                            break;
                        }
                    }
                }
            }));
        }
    }
}
