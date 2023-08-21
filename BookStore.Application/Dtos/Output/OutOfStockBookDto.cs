namespace BookStore.Application.Dtos.Output;

public class OutOfStockBookDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public decimal Price { get; set; }
}