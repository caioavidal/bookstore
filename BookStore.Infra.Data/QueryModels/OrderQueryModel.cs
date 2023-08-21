using BookStore.Domain.Entities;
using BookStore.Domain.Inventory.Entities;
using BookStore.Domain.Order.Entities;

namespace BookStore.Infra.Data.QueryModels;

public class OrderQueryModel
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public UserModel User { get; set; }
    public List<OrderItemModel> OrderItems { get; set; } = new();
    public List<OrderStatusModel> OrderStatuses { get; set; } = new();

    public static implicit operator Order(OrderQueryModel orderQueryModel)
    {
        var orderStatuses = orderQueryModel.OrderStatuses.Select(x => (OrderStatus)x).ToList();
        var orderItems = orderQueryModel.OrderItems.Select(x => (OrderItem)x).ToList();

        return new Order(orderQueryModel.User, orderStatuses, orderItems)
        {
            Id = orderQueryModel.Id,
            CreatedAt = orderQueryModel.CreatedAt
        };
    }


    public class UserModel
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; }

        public static implicit operator User(UserModel userModel)
        {
            return new User
            {
                Email = userModel.Email,
                BirthDate = userModel.BirthDate,
                Id = userModel.UserId,
                Name = userModel.Name
            };
        }
    }

    public class OrderItemModel
    {
        public Guid OrderItemId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public BookModel Book { get; set; }
        public Order Order { get; set; }

        public static implicit operator OrderItem(OrderItemModel orderItemModel)
        {
            return new OrderItem(orderItemModel.Order, orderItemModel.Book, orderItemModel.Price,
                orderItemModel.Quantity)
            {
                Id = orderItemModel.OrderItemId
            };
        }
    }

    public class BookModel
    {
        public Guid BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Isbn { get; set; }
        public int PublicationYear { get; set; }
        public decimal Price { get; set; }

        public static implicit operator Book(BookModel bookModel)
        {
            return new Book
            {
                Author = bookModel.Author,
                Price = bookModel.Price,
                Id = bookModel.BookId,
                PublicationYear = bookModel.PublicationYear,
                Title = bookModel.Title,
                ISBN = bookModel.Isbn
            };
        }
    }

    public class OrderStatusModel
    {
        public Guid OrderStatusesId { get; set; }
        public int OrderState { get; set; }
        public DateTime CreatedAt { get; set; }
        public Order Order { get; set; }

        public static implicit operator OrderStatus(OrderStatusModel orderStatusModel)
        {
            return new OrderStatus
            {
                Id = orderStatusModel.OrderStatusesId,
                Order = orderStatusModel.Order,
                CreatedAt = orderStatusModel.CreatedAt,
                State = (OrderState)orderStatusModel.OrderState
            };
        }
    }
}