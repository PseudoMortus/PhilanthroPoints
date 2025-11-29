namespace PhilanthroPoints.Models;

public class Item { public int Id{get;set;} public string Name{get;set;}=string.Empty; public string? Description{get;set;} public string? ImageUrl{get;set;} public int Cost{get;set;} public int Stock{get;set;} public int CategoryId{get;set;} public Category? Category{get;set;} }