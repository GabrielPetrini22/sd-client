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
        TcpClient cliente3;
        NetworkStream ns;
        BinaryWriter br;
        //receber mensagem
        Socket skt;
        TcpListener t1;
        NetworkStream ns2;
        NetworkStream ns3;
        Thread th;
        string cabecalho;
        string resposta;
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
                //MessageBox.Show(cabecalho);
                
                //Identifica a ação e verifica o que fazer
                if (cabecalho.Substring(0, 1).Equals("4"))
                {
                    Console.WriteLine("Ação: 4-O servidor não reconheceu a placa!");
                    MessageBox.Show("Placa não reconhecida", "Erro", MessageBoxButtons.OK);
                }
                else if (cabecalho.Substring(0, 1).Equals("3"))
                {
                    Console.WriteLine("Ação: 3-Placa reconhecida!");
                    if (MessageBox.Show("A placa reconhecida é: " + cabecalho.Substring(1, cabecalho.Length -1), "Confirmação", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        resposta = "21";
                    }
                    else
                    {
                        resposta = "20";
                    }
                    Console.WriteLine("Ação: 2-Cliente confirmou a placa ou não!");
                    //retorna o cabeçalho ao servidor
                    byte[] buffer2 = Encoding.ASCII.GetBytes(resposta + "$");
                    cliente3 = new TcpClient("172.16.102.113", 4350);
                    ns3 = cliente3.GetStream();
                    ns3.Write(buffer2, 0, buffer2.Length);
                    ns3.Flush();

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
                //string resposta1;
                Console.WriteLine("Ação: 1-Envia a imagem!");
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
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
            th = new Thread(new ThreadStart(ReceivedText));
            th.Start();
        }
    }
}
