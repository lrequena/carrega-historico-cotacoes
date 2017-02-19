using System;
using System.IO;
using System.Text;

namespace CarregaHistoricoCotacoes.Auxiliar
{
    internal static class Log
    {
        // ReSharper disable once ConvertToConstant.Local
        private static readonly bool LogAcumulativo = false;

        private static readonly string CaminhoLog = ArquivoAux.GerarCaminhoArquivo("log");
        private static bool _primeiraGravacao = true;

        private static void PreparaArquivo()
        {
            if (LogAcumulativo)
                return;

            if (!_primeiraGravacao) return;

            _primeiraGravacao = false;

            using (StreamWriter sw = new StreamWriter(CaminhoLog, false, Encoding.GetEncoding(1252)))
            {
                sw.Flush();
            }
        }

        public static void Gravar(string mensagem, bool console = true)
        {
            PreparaArquivo();

            string timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff") + " - ";

            using (StreamWriter sw = new StreamWriter(CaminhoLog, true, Encoding.GetEncoding(1252)))
            {
                sw.Write(timestamp + mensagem);
                sw.Flush();
            }

            if (console)
                Console.Write(timestamp + mensagem);
        }

        public static void GravarLinha(bool console = true)
        {
            PreparaArquivo();

            using (StreamWriter sw = new StreamWriter(CaminhoLog, true, Encoding.GetEncoding(1252)))
            {
                sw.WriteLine();
                sw.Flush();
            }

            if (console)
                Console.WriteLine();
        }

        public static void GravarLinha(string mensagem, bool console = true)
        {
            PreparaArquivo();

            string timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff") + " - ";

            using (StreamWriter sw = new StreamWriter(CaminhoLog, true, Encoding.GetEncoding(1252)))
            {
                sw.WriteLine(timestamp + mensagem);
                sw.Flush();
            }

            if (console)
                Console.WriteLine(timestamp + mensagem);
        }
    }
}
