using System;
using System.IO;

namespace CarregaHistoricoCotacoes.Auxiliar
{
    public static class ArquivoAux
    {
        public static string GerarCaminhoArquivo(string extensao)
        {
            string caminho = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Program.TituloAplicacao);
            caminho = Path.ChangeExtension(caminho, extensao);

            return caminho;
        }

        public static string GerarCaminhoScriptAtivo(int idAtivo, DateTime data)
        {
            string caminho = AppDomain.CurrentDomain.BaseDirectory;
            caminho = Path.Combine(caminho, "scripts", idAtivo.ToString(), data.ToString("yyyyMMdd"));
            caminho = Path.ChangeExtension(caminho, "sql");

            return caminho;
        }
    }
}