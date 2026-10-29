using AprenderMais.Aplicacao.Interfaces;

namespace AprenderMais.Infraestrutura.Relogio;

public class RelogioSistema : IRelogio
{
    private static readonly TimeZoneInfo FusoBrasilia = ObterFusoBrasilia();

    public DateTime AgoraUtc => DateTime.UtcNow;

    public DateTime AgoraBrasilia => ParaBrasilia(DateTime.UtcNow);

    public DateTime ParaBrasilia(DateTime utc) =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), FusoBrasilia);

    private static TimeZoneInfo ObterFusoBrasilia()
    {
        // "E. South America Standard Time" no Windows; "America/Sao_Paulo" em Linux/macOS.
        foreach (var id in new[] { "E. South America Standard Time", "America/Sao_Paulo" })
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(id);
            }
            catch (TimeZoneNotFoundException)
            {
            }
            catch (InvalidTimeZoneException)
            {
            }
        }

        return TimeZoneInfo.CreateCustomTimeZone("BRT", TimeSpan.FromHours(-3), "Brasília", "Brasília");
    }
}
