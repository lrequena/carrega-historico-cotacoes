using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace HistoricoCotacao.Modelos
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class Cotacoes
    {
        public List<Cotacao> expirations { get; set; }
    }
}