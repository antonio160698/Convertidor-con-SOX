using Renci.SshNet;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WinSCP;

namespace Convertir
{
    class Program
    {
        static void Main(string[] args)
        {
            Stereo.Stereo stereo = new Stereo.Stereo();
            /*stereo.ObtieneDocumento(@"C:\Users\antonio\Downloads\Ejemplo.gsm", "Hoy.mp3");
            stereo.ObtieneDocumento(@"C:\Users\antonio\Downloads\Ejemplo.mp3", "Manana.mp3");*/
            stereo.ssh();
        }
    }

}
namespace Stereo
{
    public class Stereo
    {
        public Stereo() { }
        
        public void ObtieneDocumento(string dirDocument, string Destino)
        {
            string[] arregloDireccion = dirDocument.Split("\\");
            arregloDireccion[arregloDireccion.Length - 1] = "";
            string originDocument = string.Join('\\', arregloDireccion);
            byte[] readText = File.ReadAllBytes(dirDocument);
            byte[] datos = new byte[300];
            int contador = 0;
            int contarbyte = 0;
            string archivo = "";
            while(contador < readText.Length)
            {
                if (contador > readText.Length - 148)
                {
                    datos[contarbyte] = readText[contador];
                    contarbyte++;
                }
                if (contador < 10)
                {
                    datos[contarbyte] = readText[contador];
                    contarbyte++;
                }
                if (readText[contador] == 75)
                {
                    if(readText[contador-1] == 162)
                    {
                        if (readText[contador - 2] == 121)
                        {
                            if (readText[contador - 3] == 239)
                            {
                                if (readText[contador - 4] == 221)
                                {
                                    Console.WriteLine(Destino + " - "+contador);
                                }
                            }
                        }
                    }
                }
                //Console.WriteLine(readText[contador]);
                contador++;
            }
            //string hex = Convert.ToBase64String(readText);
            //Console.WriteLine(hex);
            //Console.WriteLine(System.Text.ASCIIEncoding.ASCII.GetString(readText));
            Console.WriteLine();
            Console.WriteLine(System.Text.ASCIIEncoding.ASCII.GetString(datos));
            Console.WriteLine(readText.Length);
                //Console.WriteLine(archivo);
            /*if(File.Exists(originDocument + Destino))
            {
                File.Delete(originDocument + Destino);
            }*/
            //File.WriteAllBytes(originDocument+ Destino, readText);
            //Create(readText, originDocument + "Destino");
        }

        public void Create(byte[] audio, string outputName)
        {
            using (FileStream bytesToAudio = File.Create(outputName + ".mp3"))
            {
                bytesToAudio.Write(audio, 0, audio.Length);
                Stream audioFile = bytesToAudio;
                bytesToAudio.Close();
                //return audioFile;
            }
        }

        public void ssh()
        {
            SessionOptions sessionOptions = new SessionOptions
            {
                Protocol = Protocol.Sftp,
                HostName = "192.168.1.76",
                UserName = "antonio",
                Password = "blacky98",
            };
            using (SftpClient sftp = new SftpClient("192.168.1.76",22, "antonio", "blacky98" ))
            {
                try
                {
                    string pathLocalFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "musica.gsm");
                    string pathWavFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "musica.wav");
                    if (File.Exists(pathWavFile))
                    {
                        File.Delete(pathWavFile);
                    }
                    sftp.Connect();
                    using (Stream fileStream = File.OpenWrite(pathLocalFile))
                    {
                        sftp.DownloadFile(@"/home/antonio/Downloads/mar.gsm", fileStream);
                    }
                    sftp.Dispose();

                    var startInfo = new ProcessStartInfo();
                    startInfo.FileName = "C://Program Files (x86)/sox-14-4-2/sox.exe";
                    startInfo.Arguments = pathLocalFile+ "  -r 8000 -c 1 " + pathWavFile;
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.UseShellExecute = false;
                    startInfo.CreateNoWindow = false;
                    startInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    using (Process soxProc = Process.Start(startInfo))
                    {
                        soxProc.WaitForExit();
                    }
                    if (File.Exists(pathLocalFile))
                    {
                        File.Delete(pathLocalFile);
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

    }
}
