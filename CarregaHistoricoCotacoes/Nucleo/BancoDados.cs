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
            Log.GravarLinha("Gravando na base");

            List<Cotacao> cotacoes = result.result.expirations;
            cotacoes.Reverse();

            foreach (Cotacao cotacao in cotacoes)
            {
                SqlConnection conexao = new SqlConnection(Configuracoes.CadeiaConexao);
                SqlCommand comando = new SqlCommand($"INSERT INTO COTACAO(IDATIVO, DATA, VALOR) VALUES(@IDATIVO, @DATA, @VALOR)", conexao);
                comando.Parameters.AddWithValue("@IDATIVO", idAtivo);
                comando.Parameters.AddWithValue("@DATA", DateTime.ParseExact(cotacao.datetime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
                comando.Parameters.AddWithValue("@VALOR", Convert.ToDouble(cotacao.value));

                conexao.Open();

                comando.ExecuteNonQuery();

                conexao.Close();
            }
        }
    }
}
