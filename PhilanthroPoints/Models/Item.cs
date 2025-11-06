namespace PhilanthroPoints.Models;


public record Item(
    int Id,
    string Name,
    string ImageUrl,
    Category Category,
    int PointsCost,
    int Stock
);