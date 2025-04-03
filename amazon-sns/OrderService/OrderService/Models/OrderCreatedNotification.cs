namespace OrderService.Models;
public class OrderCreatedNotification(int orderId, int customerId, List<ProductDetail> productDetails)
{
    public int OrderId { get; init; } = orderId;
    public int CustomerId { get; init; } = customerId;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public List<ProductDetail> ProductDetails { get; init; } = productDetails;
}