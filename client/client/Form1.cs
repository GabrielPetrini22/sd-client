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
        MemoryStream ms;
        TcpClient client;
        NetworkStream ns;
        BinaryWriter br;
        ////server
        //Socket skt;
        //TcpListener t1;
        //NetworkStream ns2;
        //StreamReader sr;
        //Thread th;
        string ip;
        private const int TAMANHO_BUFFER = 10000;
        private int requisicoes;
        //mensagem que o cliente manda para o servidor
        private string mensagemCliente;
        //mensagem que o servidor manda ao cliente
        private string respostaServidor;
        //Socket do servidor
        private TcpListener servidor;
        //Socket do cliente
        private TcpClient cliente;
        void Servidor()
        {
            this.servidor = new TcpListener(IPAddress.Any, 4243);
            this.cliente = default(TcpClient);
            this.servidor.Start();
            this.cliente = servidor.AcceptTcpClient();
            this.requisicoes = 0;
            this.respostaServidor = "";
        }
        public void Run()
        {
            this.requisicoes++;
            NetworkStream netStream = cliente.GetStream();
            byte[] recebido = new byte[TAMANHO_BUFFER];
            //recebe a mensagem do cliente
            netStream.Read(recebido, 0, (int)cliente.ReceiveBufferSize);
            //converte bytes em string
            this.mensagemCliente = Encoding.ASCII.GetString(recebido);
            /* reduz a string deixando de fora os caracteres
             * adicionados durante o processo de conversão bytes->string */
            this.mensagemCliente = this.mensagemCliente.Substring(0, this.mensagemCliente.IndexOf("$"));

            ///* define a resposta do servidor
            // * manda para o cliente a mensagem recebida
            // * convertida em letras maiusculas */
            //this.respostaServidor = "Resposta do Servidor " + Convert.ToString(requisicoes) + ": " +
            //+this.mensagemCliente.ToUpperInvariant();

            //Byte[] enviado = Encoding.ASCII.GetBytes(this.RespostaServidor);
            ////envia a resposta em bytes ao cliente
            //netStream.Write(enviado, 0, enviado.Length);
            //netStream.Flush();
        }

        //void ReceivedText()
        //{
        //    try
        //    {
        //        t1 = new TcpListener(4243);
        //        t1.Start();
        //        skt = t1.AcceptSocket();
        //        ns2 = new NetworkStream(skt);
        //        byte[] buffer = new byte[10];
        //        ns.Read(buffer, 0, 1);
        //        string line = Encoding.UTF8.GetString(buffer);
        //        textBox3.Text = line;
        //        t1.Stop();
        //        if (skt.Connected == true)
        //        {
        //            while (true)
        //            {
        //                ReceivedText();
        //            }
        //        }
        //    }
        //    catch (Exception ex) { MessageBox.Show(ex.Message); }
        //}
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
            //th = new Thread(new ThreadStart(ReceivedText));
            //th.Start();
            //textBox1.Text = GetIpAdress();
        }
    }
}
