using PhilanthroPoints.Models;


namespace PhilanthroPoints.Services;


public class CatalogService
{
    private readonly List<Item> _seed =
    [
        new Item(1, "LEGO Creator Box", "https://picsum.photos/seed/lego/400/260", Category.Gifts, 40, 12),
        new Item(2, "Plush Bear", "https://picsum.photos/seed/bear/400/260", Category.Gifts, 20, 8),
        new Item(3, "Graphic Novel", "https://picsum.photos/seed/book/400/260", Category.Books, 15, 20),
        new Item(4, "Birthday Card – Stars", "https://picsum.photos/seed/card/400/260", Category.Cards, 5, 50),
        new Item(5, "Cupcake 6‑Pack", "https://picsum.photos/seed/cupcake/400/260", Category.Treats, 10, 16)
    ];


    public IEnumerable<Item> GetByCategory(Category c) => _seed.Where(i => i.Category == c);
    public IEnumerable<Item> All() => _seed;
    public Item? Find(int id) => _seed.FirstOrDefault(i => i.Id == id);
}