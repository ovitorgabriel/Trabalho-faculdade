namespace AprenderMais.Aplicacao.Interfaces;

/// <summary>
/// Abstrai a data/hora para que as regras de prazo possam ser testadas.
/// </summary>
public interface IRelogio
{
    DateTime AgoraUtc { get; }
    DateTime AgoraBrasilia { get; }
    DateTime ParaBrasilia(DateTime utc);
}
