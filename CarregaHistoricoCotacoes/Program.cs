using System;
using System.Collections.Generic;
using System.Reflection;
using CarregaHistoricoCotacoes.Auxiliar;
using CarregaHistoricoCotacoes.Nucleo;
using HistoricoCotacao.Modelos;

namespace CarregaHistoricoCotacoes
{
    public static class Program
    {
        internal static readonly string TituloAplicacao = ((AssemblyTitleAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false)[0]).Title;

        public static void Main()
        {
            Log.GravarLinha("Inicialização da aplicação");

            Configuracoes.CarregaConfiguracoes();

            CargaHistorico();

            EncerrarAplicacao();
        }

        private static void CargaHistorico()
        {
            Log.GravarLinha("Iniciando carga do histórico de cotações");

            DateTime dataAtual = Configuracoes.DataMaxima;
            DateTime dataCorte = Configuracoes.DataMinima;

            while (dataAtual >= dataCorte)
            {
                if (!Configuracoes.IncluirOtc)
                {
                    while (Cotacao.EhOtc(dataAtual))
                    {
                        Log.GravarLinha("Ignorando período OTC: " + dataAtual.ToString("dd-MM-yyyy HH:mm") + " (UTC)");
                        dataAtual = dataAtual.AddMinutes(-1);
                    }
                }

                string processando = $"Consultando: {dataAtual.ToString("dd-MM-yyyy")} UTC";
                processando += $" - Hora: {dataAtual.ToString("HH")}";
                processando += " - Minuto: ";

                if (dataAtual.Minute == 0)
                {
                    processando += "00 ao 16";
                }
                else
                {
                    processando += dataAtual.Minute.ToString().PadLeft(2, '0');
                }

                Log.GravarLinha(processando);

                foreach (int idAtivo in Configuracoes.Ativos)
                {
                    Log.GravarLinha($"Ativo: {Cotacao.ConverterCotacao(idAtivo)}");

                    ResultCotacoes result = Api.ObterCotacoes(dataAtual, idAtivo);

                    if (Configuracoes.ExibirLogCotacao)
                    {
                        List<Cotacao> cotacoes = result.result.expirations;
                        cotacoes.Reverse();

                        foreach (Cotacao cotacao in cotacoes)
                        {
                            Log.GravarLinha($"{cotacao.datetime} - {cotacao.value}");
                        }
                    }

                    BancoDados.GravarCotacao(idAtivo, result);
                }

                dataAtual = dataAtual.Minute == 17 ? dataAtual.AddMinutes(-17) : dataAtual.AddMinutes(-1);
            }
        }

        internal static void EncerrarAplicacao()
        {
            Console.WriteLine();
            Console.WriteLine("Pressione qualquer tecla para sair...");
            Console.ReadKey(true);

            Log.GravarLinha("Encerrando aplicação. ExitCode: " + Environment.ExitCode);

            Environment.Exit(Environment.ExitCode);
        }
    }
}
