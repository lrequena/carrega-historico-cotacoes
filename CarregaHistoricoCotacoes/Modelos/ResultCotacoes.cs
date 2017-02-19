using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace HistoricoCotacao.Modelos
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class ResultCotacoes
    {
        public bool isSuccessful { get; set; }
        public List<string> message { get; set; }
        public Cotacoes result { get; set; }
    }
}
