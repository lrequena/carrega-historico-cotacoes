using System;
using System.Diagnostics.CodeAnalysis;

namespace HistoricoCotacao.Modelos
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class Cotacao
    {
        public string datetime { get; set; }
        public double value { get; set; }
        public double bid { get; set; }
        public double ask { get; set; }

        /*
            Período de OTC, considerando horário UTC:
            Sexta-Feira 22:00 até Domingo 20:59
        */
        internal static bool EhOtc(DateTime dataRef)
        {
            if (dataRef.DayOfWeek == DayOfWeek.Saturday)
                return true;

            if (dataRef.DayOfWeek == DayOfWeek.Friday && dataRef.Hour >= 22)
                return true;

            if (dataRef.DayOfWeek == DayOfWeek.Sunday && dataRef.Hour < 21)
                return true;

            return false;
        }

        internal static string ConverterCotacao(int id)
        {
            switch (id)
            {
                case 1:
                    return "EUR/USD";
                case 2:
                    return "EUR/GBP";
                case 3:
                    return "GBP/JPY";
                case 4:
                    return "EUR/JPY";
                case 5:
                    return "GBP/USD";
                case 6:
                    return "USD/JPY";
                case 7:
                    return "AUD/CAD";
                case 8:
                    return "NZD/USD";
                case 9:
                    return "EUR/RUB";
                case 10:
                    return "USD/RUB";
                case 13:
                    return "COMMERZBANK";
                case 14:
                    return "Daimler AG";
                case 15:
                    return "Deutsche Bank";
                case 16:
                    return "E.ON";
                case 23:
                    return "British Petroleum";
                case 27:
                    return "Gazprom";
                case 28:
                    return "Rosneft";
                case 29:
                    return "Sberbank";
                case 31:
                    return "Amazon";
                case 32:
                    return "Apple";
                case 33:
                    return "Baidu";
                case 34:
                    return "Cisco Systems";
                case 35:
                    return "Facebook";
                case 36:
                    return "Google";
                case 37:
                    return "Intel";
                case 38:
                    return "Microsoft";
                case 40:
                    return "Yahoo!";
                case 41:
                    return "AIG";
                case 42:
                    return "Bank of America";
                case 45:
                    return "CitiGroup";
                case 46:
                    return "Coca Cola";
                case 49:
                    return "General Motors";
                case 50:
                    return "Goldman Sachs";
                case 51:
                    return "JP Morgan Chase";
                case 52:
                    return "McDonalds";
                case 53:
                    return "Morgan Stanley";
                case 54:
                    return "Nike";
                case 56:
                    return "Verizon";
                case 57:
                    return "Wal-Mart";
                case 66:
                    return "DAX 30 (Germany)";
                case 67:
                    return "DOW JONES 30";
                case 68:
                    return "FTSE 100 (UK)";
                case 69:
                    return "NASDAQ 100";
                case 70:
                    return "Nikkei 225 (at USA)";
                case 71:
                    return "S&P 500";
                case 72:
                    return "USD/CHF";
                case 73:
                    return "Bitcoin Index";
                case 74:
                    return "Gold";
                case 75:
                    return "Silver";
                case 76:
                    return "EUR/USD (OTC)";
                case 77:
                    return "EUR/GBP (OTC)";
                case 78:
                    return "USD/CHF (OTC)";
                case 79:
                    return "EUR/JPY (OTC)";
                case 80:
                    return "NZD/USD (OTC)";
                case 81:
                    return "GBP/USD (OTC)";
                case 82:
                    return "EUR/RUB (OTC)";
                case 83:
                    return "USD/RUB (OTC)";
                case 84:
                    return "GBP/JPY (OTC)";
                case 85:
                    return "USD/JPY (OTC)";
                case 86:
                    return "AUD/CAD (OTC)";
                case 87:
                    return "Alibaba";
                case 95:
                    return "Yandex";
                case 97:
                    return "Ping An Insurance Group Co of China Ltd";
                case 99:
                    return "AUD/USD";
                case 100:
                    return "USD/CAD";
                case 101:
                    return "AUD/JPY";
                case 102:
                    return "GBP/CAD";
                case 103:
                    return "GBP/CHF";
                case 104:
                    return "GBP/AUD";
                case 105:
                    return "EUR/CAD";
                case 106:
                    return "CHF/JPY";
                case 107:
                    return "CAD/CHF";
                case 108:
                    return "EUR/AUD";
                case 110:
                    return "BMW";
                case 111:
                    return "Lufthansa airline";
                case 113:
                    return "Twitter Inc";
                case 133:
                    return "Ferrari";
                case 166:
                    return "SMI";
                case 167:
                    return "Tesla";
                case 168:
                    return "USD/NOK";
                case 169:
                    return "SSE Index";
                case 170:
                    return "Hang Seng";
                case 208:
                    return "SP/ASX200";
                case 209:
                    return "TOPIX500";
                case 210:
                    return "DX (Dollar Index)";
                case 212:
                    return "EUR/NZD";
                case 213:
                    return "SIN (FAKE)";
                case 215:
                    return "Brent Oil Jul 16";
                case 216:
                    return "Gold Jun 16";
                case 217:
                    return "Brent Oil Aug 16";
                case 218:
                    return "Nintendo";
                case 219:
                    return "USD/SEK";
                case 220:
                    return "USD/TRY";
                default:
                    return "Cotação Inválida";
            }
        }
    }
}