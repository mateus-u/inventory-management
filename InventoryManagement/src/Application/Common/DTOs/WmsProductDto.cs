namespace Application.Common.DTOs;

public record WmsProductDto(
    string Name,
    string Description,
    decimal Price,
    int Quantity
);

public record WmsProductResponseDto(
    string WmsProductId
);

public record WmsDispatchResponseDto(
    string Message
);
