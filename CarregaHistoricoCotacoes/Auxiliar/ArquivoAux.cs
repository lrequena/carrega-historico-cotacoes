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
    }
}
