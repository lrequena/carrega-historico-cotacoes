using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using HistoricoCotacao.Modelos;
using Newtonsoft.Json;

namespace CarregaHistoricoCotacoes.Nucleo
{
    internal static class Api
    {
        private static readonly long IntervaloMinimo = TimeSpan.FromMilliseconds(800).Ticks;

        private static long _ultimoTickRequisicao;

        internal static ResultCotacoes ObterCotacoes(DateTime dataAtual, int idAtivo)
        {
            while (Math.Abs(DateTime.Now.Ticks - _ultimoTickRequisicao) < IntervaloMinimo)
            {
                Thread.Sleep(10);
            }

            string url = $"https://iqoption.com/api/quote/history/v1/expirations?active_id={idAtivo}&tz_offset=0&";
            url += $"date={dataAtual.Year}-{dataAtual.Month}-{dataAtual.Day}-{dataAtual.Hour}";

            if (dataAtual.Minute != 0)
                url += $"-{dataAtual.Minute}";

            string json = new WebClient().DownloadString(url);
            _ultimoTickRequisicao = DateTime.Now.Ticks;

            ResultCotacoes result = JsonConvert.DeserializeObject<ResultCotacoes>(json);

            result.result.expirations.RemoveAll(e => Regex.Match(e.datetime,
                @"[\d]{4}-[\d]{2}-[\d]{2} [\d]{2}:[\d]{2}:([\d]{2})").Groups[1].Value != "00");

            return result;
        }
    }
}
