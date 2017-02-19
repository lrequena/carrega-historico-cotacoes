using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using CarregaHistoricoCotacoes.Auxiliar;
using HistoricoCotacao.Modelos;

namespace CarregaHistoricoCotacoes.Nucleo
{
    public static class BancoDados
    {
        /*
            CREATE TABLE COTACAO(
	            IDATIVO INT,
	            DATA DATETIME,
	            VALOR FLOAT)

            CREATE INDEX COTACAO_IN_01 ON COTACAO(IDATIVO)
            CREATE INDEX COTACAO_IN_02 ON COTACAO(DATA)
            CREATE INDEX COTACAO_IN_03 ON COTACAO(IDATIVO, DATA)
        */

        internal static void GravarCotacao(int idAtivo, ResultCotacoes result)
        {
            List<Cotacao> cotacoes = result.result.expirations;
            cotacoes.Reverse();

            foreach (Cotacao cotacao in cotacoes)
            {
                DateTime dataRef = DateTime.ParseExact(cotacao.datetime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                double valorCotacao = Convert.ToDouble(cotacao.value);

                string sql = string.Empty;

                if (Configuracoes.Sobreescrever)
                {
                    sql += "IF EXISTS(SELECT 1 FROM COTACAO WHERE IDATIVO = @IDATIVO AND DATA = @DATA)" + Environment.NewLine;
                    sql += '\t' + "UPDATE COTACAO SET VALOR = @VALOR WHERE IDATIVO = @IDATIVO AND DATA = @DATA";
                }
                else
                {
                    sql += "IF NOT EXISTS(SELECT 1 FROM COTACAO WHERE IDATIVO = @IDATIVO AND DATA = @DATA)" + Environment.NewLine + '\t';
                    sql += '\t' + "INSERT INTO COTACAO(IDATIVO, DATA, VALOR) VALUES(@IDATIVO, @DATA, @VALOR)";
                }

                if (Configuracoes.GravarBancoDados)
                {
                    Log.GravarLinha("Gravando na base");

                    SqlConnection conexao = new SqlConnection(Configuracoes.CadeiaConexao);
                    SqlCommand comando = new SqlCommand(sql, conexao);
                    comando.Parameters.AddWithValue("@IDATIVO", idAtivo);
                    comando.Parameters.AddWithValue("@DATA", dataRef);
                    comando.Parameters.AddWithValue("@VALOR", valorCotacao);
                    conexao.Open();
                    comando.ExecuteNonQuery();
                    conexao.Close();
                }

                GravarScript(sql, idAtivo, dataRef, valorCotacao);
            }
        }

        private static void GravarScript(string sql, int idAtivo, DateTime dataRef, double valorCotacao)
        {
            if (!Configuracoes.GravarScript) return;

            Log.GravarLinha("Gravando script");

            sql = sql.Replace("@IDATIVO", idAtivo.ToString());
            sql = sql.Replace("@DATA", $"'{dataRef.ToString("yyyyMMdd HH:mm:ss")}'");
            sql = sql.Replace("@VALOR", valorCotacao.ToString("F6", Constantes.CulturaUs));

            sql += Environment.NewLine;

            string script = ArquivoAux.GerarCaminhoScriptAtivo(idAtivo, dataRef);

            Log.GravarSql(script, sql);
        }
    }
}
