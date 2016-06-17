using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //Cliente, enviar o arquivo
        MemoryStream ms;
        TcpClient client;
        TcpClient client2 = new TcpClient();
        NetworkStream ns;
        BinaryWriter br;
        //receber mensagem
        Socket skt;
        TcpListener t1;
        NetworkStream ns2;
        Thread th;
        string cabecalho;
        void ReceivedText()//recebe a String da placa ou não reconhecida
        {
            try
            {
                t1 = new TcpListener(4250);
                t1.Start();
                skt = t1.AcceptSocket();
                ns2 = new NetworkStream(skt);
                byte[] buffer = new byte[10000];
                ns2.Read(buffer, 0, 1000);
                //Recebe o cabeçalho
                cabecalho = Encoding.UTF8.GetString(buffer);
                cabecalho = cabecalho.Substring(0, cabecalho.IndexOf("$"));
                //Identifica a ação e verifica o que fazer
                if (cabecalho.Substring(0, 1).Equals("4"))
                {
                    MessageBox.Show("Placa não reconhecida", "Erro", MessageBoxButtons.OK);
                }
                else if (cabecalho.Substring(0, 1).Equals("3"))
                {
                    if (MessageBox.Show("A placa reconhecida é: " + cabecalho.Substring(1, 8), "Confirmação", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        cabecalho = "21";
                    }
                    else
                    {
                        cabecalho = "20";
                    }
                    //retorna o cabeçalho ao servidor

                }
                t1.Stop();
                if (skt.Connected == true)
                {
                    while (true)
                    {
                        ReceivedText();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        //obtem endereço local
        //string GetIpAdress()
        //{
        //    //IPHostEntry host;
        //    string localip = "172.16.103.16";
        //    //host = Dns.GetHostEntry(Dns.GetHostName());
        //    //foreach (IPAddress ip in host.AddressList)
        //    //{
        //    //    if(ip.AddressFamily.ToString()== "InterNetwork")
        //    //    {
        //    //        localip = ip.ToString();
        //    //    }
        ////}
        //    return localip;
        //}
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            string caminho = openFileDialog1.FileName;
            pictureBox1.Image = Image.FromFile(caminho);
            textBox2.Text = caminho;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                ms = new MemoryStream();
                pictureBox1.Image.Save(ms, pictureBox1.Image.RawFormat);
                byte[] buffer = ms.GetBuffer();
                ms.Close();
                client = new TcpClient(textBox1.Text, 4242);
                ns = client.GetStream();
                br = new BinaryWriter(ns);
                br.Write(buffer);
                br.Close();
                ns.Close();
                client.Close();
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
            th = new Thread(new ThreadStart(ReceivedText));
            th.Start();
        }
    }
}
