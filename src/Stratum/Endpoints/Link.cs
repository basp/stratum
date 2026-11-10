namespace Stratum.Endpoints;

internal sealed record Link(
    string Href, 
    string Rel,
    string? Type = null,
    string? Title = null);