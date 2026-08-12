namespace Microondas.Dominio.Servicos
{
    /// <summary>
    /// Converte um total de segundos na representação "m:ss".
    /// Atende o Nível 1, item 2.c (ex.: 90 segundos => "1:30"), de forma generalizada:
    /// 30 => "0:30", 120 => "2:00".
    /// </summary>
    public static class FormatadorTempo
    {
        public static string Formatar(int totalSegundos)
        {
            if (totalSegundos < 0)
                totalSegundos = 0;

            int minutos = totalSegundos / 60;
            int segundos = totalSegundos % 60;
            return minutos + ":" + segundos.ToString("D2");
        }
    }
}
