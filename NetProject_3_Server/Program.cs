using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NetProject_3_Server
{
    class Program
    {
        static int port = 8080;
        static void Main(string[] args)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                socket.Bind(endPoint);
                socket.Listen(2);
                Socket handler = socket.Accept();
                StringBuilder builder = new StringBuilder();
                int bytes = 0; 
                byte[] data = new byte[256];                

                do
                {
                    bytes = handler.Receive(data);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (handler.Available > 0);

                string[] list;

                list = XMLParseAdresses(builder.ToString());

                string buff = "";
                for (int i = 0; i != list.Length; i++)
                    buff += list[i].ToString() + ";";



                //Thread th = new Thread(new ParameterizedThreadStart(JsonParseAdresses));
                //th.Start(builder);
                data = Encoding.Unicode.GetBytes(buff);
                handler.Send(data);
                // закрываем сокет
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static string[] XMLParseAdresses (string input)
        {
            string[] adresses = new string[10];
            List<Adress> output = new List<Adress>();
            string pathToXML = "halykaralyk_poshta_almasu_tap-v2.xml";
            try
            {
                XDocument xdoc = XDocument.Load(pathToXML);

                foreach (XElement item in xdoc.Element("halykaralyk_poshta_almasu_tap").Elements("data"))
                {
                    Adress adr = new Adress();
                    adr.Index = item.Element("index").Value;
                    

                    if (adr.Index.Equals(input))
                    {
                        adr.Street = item.Element("street_ru").Value;
                        output.Add(adr);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            for (int i = 0; i < adresses.Length; i++)
            {
                foreach (var item in output)
                {
                    var b = item.ToString();
                    adresses[i] = b;
                }
            }
            

            return adresses;
        }
    }
}
