using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CarregaHistoricoCotacoes.Auxiliar;
using Nini.Config;

namespace CarregaHistoricoCotacoes.Nucleo
{
    internal static class Configuracoes
    {
        private const string SetorBancoDados = "BANCO_DADOS";

        private const string ChaveServidor = "servidor";
        private const string ChaveAutenticacaoWindows = "autenticacao_windows";
        private const string ChaveUsuario = "usuario";
        private const string ChaveSenha = "senha";
        private const string ChaveNomeBase = "database";

        private const string SetorCotacoes = "COTACOES";

        private const string ChaveExibirLog = "exibirlog";
        private const string ChaveSobreescrever = "sobreescrever";
        private const string ChaveDataHoraMinima = "data_hora_min";
        private const string ChaveDataHoraMaxima = "data_hora_max";
        private const string ChaveAtivos = "ativos";

        internal static string CadeiaConexao
        {
            get
            {
                StringBuilder connString = new StringBuilder();
                connString.Append("Application Name=" + Program.TituloAplicacao);
                connString.Append($"Data Source={Servidor};");
                connString.Append($"Initial Catalog={NomeBase};");

                if (AutenticacaoWindows)
                {
                    connString.Append("Integrated Security=True;");
                }
                else
                {
                    connString.Append($"User ID={Usuario};");
                    connString.Append($"Password={Senha};");
                }

                return connString.ToString();
            }
        }

        public static string Servidor { get; set; }

        public static bool AutenticacaoWindows { get; set; }

        public static string Usuario { get; set; }

        public static string Senha { get; set; }

        public static string NomeBase { get; set; }

        public static bool ExibirLogCotacao { get; set; }

        public static bool Sobreescrever { get; set; }

        public static DateTime DataMinima { get; set; }

        public static DateTime DataMaxima { get; set; }

        public static List<int> Ativos { get; set; }

        internal static void CarregaConfiguracoes()
        {
            Log.GravarLinha("Carregando configurações");

            try
            {
                IniConfigSource ini = new IniConfigSource(ArquivoAux.GerarCaminhoArquivo("ini"));
                ini.Alias.AddAlias("1", true);
                ini.Alias.AddAlias("0", false);

                Servidor = ini.Configs[SetorBancoDados].GetString(ChaveServidor);
                Usuario = ini.Configs[SetorBancoDados].GetString(ChaveUsuario);
                Senha = ini.Configs[SetorBancoDados].GetString(ChaveSenha);
                NomeBase = ini.Configs[SetorBancoDados].GetString(ChaveNomeBase);

                try
                {
                    AutenticacaoWindows = ini.Configs[SetorBancoDados].GetBoolean(ChaveAutenticacaoWindows);
                }
                catch (ArgumentException)
                {
                    throw new Exception($"Erro de conversão do valor da chave de configuração '{ChaveAutenticacaoWindows}' de string para Boolean.");
                }

                try
                {
                    ExibirLogCotacao = ini.Configs[SetorCotacoes].GetBoolean(ChaveExibirLog);
                }
                catch (ArgumentException)
                {
                    throw new Exception($"Erro de conversão do valor da chave de configuração '{ChaveExibirLog}' de string para Boolean.");
                }

                try
                {
                    Sobreescrever = ini.Configs[SetorCotacoes].GetBoolean(ChaveSobreescrever);
                }
                catch (ArgumentException)
                {
                    throw new Exception($"Erro de conversão do valor da chave de configuração '{ChaveSobreescrever}' de string para Boolean.");
                }

                try
                {
                    string dataMin = ini.Configs[SetorCotacoes].GetString(ChaveDataHoraMinima);

                    if (string.IsNullOrWhiteSpace(dataMin))
                        throw new Exception();

                    DataMinima = Convert.ToDateTime(dataMin, Constantes.CulturaBr);
                    DataMinima = new DateTime(DataMinima.Year,
                                              DataMinima.Month,
                                              DataMinima.Day,
                                              DataMinima.Hour,
                                              DataMinima.Minute,
                                              0);
                }
                catch (Exception)
                {
                    throw new Exception($"Erro de conversão do valor da chave de configuração '{ChaveDataHoraMinima}' de string para DateTime.");
                }

                try
                {
                    string dataMax = ini.Configs[SetorCotacoes].GetString(ChaveDataHoraMaxima);

                    if (string.IsNullOrWhiteSpace(dataMax))
                        throw new Exception();

                    DataMaxima = Convert.ToDateTime(dataMax, Constantes.CulturaBr);
                    DataMaxima = new DateTime(Math.Min(DateTime.Now.Ticks, DataMaxima.Ticks));
                    DataMaxima = new DateTime(DataMaxima.Year,
                                              DataMaxima.Month,
                                              DataMaxima.Day,
                                              DataMaxima.Hour,
                                              DataMaxima.Minute,
                                              0);
                }
                catch (Exception)
                {
                    throw new Exception($"Erro de conversão do valor da chave de configuração '{ChaveDataHoraMaxima}' de string para DateTime.");
                }

                try
                {
                    string ativos = ini.Configs[SetorCotacoes].GetString(ChaveAtivos);

                    if (string.IsNullOrWhiteSpace(ativos))
                        throw new Exception();

                    string[] valores = ativos.Split(',');

                    Ativos = valores.Where(v => !string.IsNullOrWhiteSpace(v)).Select(val => Convert.ToInt32(val.Trim())).ToList();
                }
                catch (Exception)
                {
                    throw new Exception($"Erro de conversão do valor da chave de configuração '{ChaveAtivos}' de string para List<int>.");
                }
            }
            catch (Exception ex)
            {
                Log.GravarLinha(ex.Message);
                Log.GravarLinha(ex.StackTrace, false);
                Environment.ExitCode = Erros.ErroCarregarConfiguracao;
                Program.EncerrarAplicacao();
            }

            Type tipo = typeof(Configuracoes);
            foreach (PropertyInfo campo in tipo.GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
                object valor = campo.GetValue(null);

                if (valor is bool)
                {
                    Log.GravarLinha($"{campo.Name} = {((bool)valor ? "Sim" : "Não")}");
                }
                else if (valor is List<int>)
                {
                    Log.GravarLinha($"{campo.Name} = {string.Join(",", ((List<int>)valor).ToArray())}");
                }
                else
                {
                    Log.GravarLinha($"{campo.Name} = {valor}");
                }
            }

            Log.GravarLinha("Configurações carregadas com sucesso");
        }
    }
}
