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

        private const string ChaveExibirLog = "exibir_log";
        private const string ChaveGravarScript = "gravar_script";
        private const string ChaveGravarBancoDados = "gravar_banco_dados";
        private const string ChaveSobreescrever = "sobreescrever";
        private const string ChaveIncluirOtc = "incluir_otc";
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

        private static string Servidor { get; set; }

        private static bool AutenticacaoWindows { get; set; }

        private static string Usuario { get; set; }

        private static string Senha { get; set; }

        private static string NomeBase { get; set; }

        public static bool ExibirLogCotacao { get; private set; }

        public static bool GravarScript { get; private set; }

        public static bool GravarBancoDados { get; private set; }

        public static bool Sobreescrever { get; private set; }

        /*
            Período de OTC, considerando horário UTC:
            Sexta-Feira 22:00 até Domingo 20:59
        */ 
        public static bool IncluirOtc { get; private set; }

        public static DateTime DataMinima { get; private set; }

        public static DateTime DataMaxima { get; private set; }

        public static List<int> Ativos { get; private set; }

        internal static void CarregaConfiguracoes()
        {
            Log.GravarLinha("Carregando configurações");

            try
            {
                IniConfigSource ini = new IniConfigSource(ArquivoAux.GerarCaminhoArquivo("ini"));
                ini.Alias.AddAlias("1", true);
                ini.Alias.AddAlias("0", false);

                Servidor = ObterConfiguracao<string>(ini, SetorBancoDados, ChaveServidor);
                AutenticacaoWindows = ObterConfiguracao<bool>(ini, SetorBancoDados, ChaveAutenticacaoWindows);
                Usuario = ObterConfiguracao<string>(ini, SetorBancoDados, ChaveUsuario);
                Senha = ObterConfiguracao<string>(ini, SetorBancoDados, ChaveSenha);
                NomeBase = ObterConfiguracao<string>(ini, SetorBancoDados, ChaveNomeBase);
                ExibirLogCotacao = ObterConfiguracao<bool>(ini, SetorCotacoes, ChaveExibirLog);
                GravarScript = ObterConfiguracao<bool>(ini, SetorCotacoes, ChaveGravarScript);
                GravarBancoDados = ObterConfiguracao<bool>(ini, SetorCotacoes, ChaveGravarBancoDados);
                Sobreescrever = ObterConfiguracao<bool>(ini, SetorCotacoes, ChaveSobreescrever);
                IncluirOtc = ObterConfiguracao<bool>(ini, SetorCotacoes, ChaveIncluirOtc);
                Ativos = ObterConfiguracao<List<int>>(ini, SetorCotacoes, ChaveAtivos);

                DataMinima = ObterConfiguracao<DateTime>(ini, SetorCotacoes, ChaveDataHoraMinima);
                DataMinima = new DateTime(DataMinima.Year,
                                             DataMinima.Month,
                                             DataMinima.Day,
                                             DataMinima.Hour,
                                             DataMinima.Minute,
                                             0);

                DataMaxima = ObterConfiguracao<DateTime>(ini, SetorCotacoes, ChaveDataHoraMaxima);
                DataMaxima = new DateTime(Math.Min(DateTime.UtcNow.Ticks, DataMaxima.Ticks));
                DataMaxima = new DateTime(DataMaxima.Year,
                                          DataMaxima.Month,
                                          DataMaxima.Day,
                                          DataMaxima.Hour,
                                          DataMaxima.Minute,
                                          0);
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

        private static T ObterConfiguracao<T>(IConfigSource ini, string setor, string chave)
        {
            // Tratamento para string
            if (typeof(T) == typeof(string))
            {
                try
                {
                    string valor = ini.Configs[setor].GetString(chave);

                    return (T)Convert.ChangeType(valor, typeof(T));
                }
                catch (InvalidCastException)
                {
                    throw new Exception($"Erro de conversão genérica do valor da chave de configuração '{chave}' para String.");
                }
            }

            // Tratamento para bool
            if (typeof(T) == typeof(bool))
            {
                try
                {
                    bool valor;

                    try
                    {
                        valor = ini.Configs[setor].GetBoolean(chave);
                    }
                    catch (ArgumentException)
                    {
                        throw new Exception(
                            $"Erro de conversão do valor da chave de configuração '{chave}' de string para Boolean.");
                    }

                    return (T)Convert.ChangeType(valor, typeof(T));
                }
                catch (InvalidCastException)
                {
                    throw new Exception($"Erro de conversão genérica do valor da chave de configuração '{chave}' para Boolean.");
                }
            }

            // Tratamento para DateTime
            if (typeof(T) == typeof(DateTime))
            {
                try
                {
                    string valor = ini.Configs[setor].GetString(chave);

                    if (string.IsNullOrWhiteSpace(valor))
                        throw new ArgumentException();

                    return (T)Convert.ChangeType(Convert.ToDateTime(valor, Constantes.CulturaBr), typeof(T));
                }
                catch (InvalidCastException)
                {
                    throw new Exception($"Erro de conversão genérica do valor da chave de configuração '{chave}' para DateTime.");
                }
            }

            // Tratamento para List<int>
            if (typeof(T) == typeof(List<int>))
            {
                try
                {
                    string valor = ini.Configs[setor].GetString(chave);

                    if (string.IsNullOrWhiteSpace(valor))
                        throw new ArgumentException();

                    string[] valores = valor.Split(',');

                    List<int> lista = valores.Where(v => !string.IsNullOrWhiteSpace(v)).Select(val => Convert.ToInt32(val.Trim())).ToList();

                    return (T)Convert.ChangeType(lista, typeof(T));
                }
                catch (InvalidCastException)
                {
                    throw new Exception($"Erro de conversão genérica do valor da chave de configuração '{chave}' para List<int>.");
                }
            }

            throw new ArgumentException($"Sem tratamento genérico para o tipo {typeof(T)}.");
        }
    }
}
