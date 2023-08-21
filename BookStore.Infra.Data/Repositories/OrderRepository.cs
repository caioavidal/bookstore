using System.Data;
using BookStore.Domain.Order.Contracts;
using BookStore.Domain.Order.Entities;
using BookStore.Infra.Data.QueryModels;
using BookStore.Infra.Data.Repositories.Contracts;

namespace BookStore.Infra.Data.Repositories;

public class OrderRepository : IOrderRepository
{
    private const string OrderQuery = """
                                      SELECT *, o.Id, o.created_at CreatedAt,
                                      u.id UserId, u.Name, u.birth_date BirthDate, u.email,
                                      oi.id OrderItemId, oi.price, oi.quantity,
                                      b.id BookId, b.title, b.author, b.isbn, b.publication_year PublicationYear, b.price,
                                      os.id OrderStatusesId, os.order_state OrderState, os.created_at CreatedAt
                                      FROM ORDERS o
                                      INNER JOIN users u ON u.id = o.user_id
                                      INNER JOIN order_items oi ON oi.order_id = o.id
                                      INNER JOIN books b ON b.id = oi.book_id
                                      INNER JOIN order_statuses os ON os.order_id = o.id
                                      """;

    private readonly IDapperWrapper _dapperWrapper;

    private readonly IDbConnection _db;

    public OrderRepository(IDbConnection db, IDapperWrapper dapperWrapper)
    {
        _db = db;
        _dapperWrapper = dapperWrapper;
    }

    public async Task<IEnumerable<Order>> GetByUserId(Guid userId)
    {
        const string sql = $"""
                            {OrderQuery}
                            WHERE u.id = @UserId
                            """;

        return await QueryOrders(sql, new { UserId = userId });
    }

    public async Task<IEnumerable<Order>> GetByItemId(Guid itemId)
    {
        const string sql = $"""
                            {OrderQuery}
                            WHERE b.id = @ItemId
                            """;

        return await QueryOrders(sql, new { ItemId = itemId });
    }

    public async Task<Order> GetById(Guid orderId)
    {
        const string sql = $"""
                            {OrderQuery}
                            WHERE o.id = @Id
                            """;

        OrderQueryModel orderEntry = null;

        var result = await _dapperWrapper.QueryAsync<
            OrderQueryModel,
            OrderQueryModel.UserModel,
            OrderQueryModel.OrderItemModel,
            OrderQueryModel.BookModel,
            OrderQueryModel.OrderStatusModel,
            OrderQueryModel>(
            sql,
            (order, user, orderItem, book, orderStatus) =>
            {
                orderEntry ??= order;

                orderEntry.User = user;

                if (orderEntry.OrderItems.All(x => x.OrderItemId != orderItem.OrderItemId))
                {
                    orderEntry.OrderItems.Add(orderItem);
                    orderItem.Book = book;
                    orderItem.Order = orderEntry;
                }

                orderEntry.OrderStatuses.Add(orderStatus);
                orderStatus.Order = orderEntry;

                return orderEntry;
            },
            new { id = orderId },
            splitOn: "UserId,OrderItemId,BookId,OrderStatusesId");

        return result.FirstOrDefault();
    }

    public async Task AddOrder(Order order)
    {
        using var transaction = _db.BeginTransaction();

        var sql = "INSERT INTO Orders (Id, user_id, Created_At) VALUES (@Id, @UserId, @CreatedAt)";
        await _dapperWrapper.ExecuteAsync(sql, new
        {
            order.Id,
            order.CreatedAt,
            UserId = order.User?.Id
        });

        foreach (var orderItem in order.OrderItems) await AddOrderItem(orderItem);

        foreach (var status in order.Statuses) await AddOrderStatus(status);

        transaction.Commit();
    }

    public async Task UpdateOrderStatus(Order order)
    {
        var orderStatusRecords = (await GetOrderStatuses(order.Id)).ToList();

        foreach (var orderStatus in order.Statuses)
        {
            if (orderStatusRecords.Any(x => x.Id == orderStatus.Id)) continue;

            await AddOrderStatus(orderStatus);
        }
    }

    private async Task<IEnumerable<Order>> QueryOrders(string sql, object param)
    {
        var ordersMap = new Dictionary<Guid, OrderQueryModel>();

        var result = await _dapperWrapper.QueryAsync<
            OrderQueryModel,
            OrderQueryModel.UserModel,
            OrderQueryModel.OrderItemModel,
            OrderQueryModel.BookModel,
            OrderQueryModel.OrderStatusModel,
            OrderQueryModel>(
            sql,
            (order, user, orderItem, book, orderStatus) =>
            {
                ordersMap.TryGetValue(order.Id, out var orderEntry);

                orderEntry ??= order;

                orderEntry.User = user;

                if (orderEntry.OrderItems.All(x => x.OrderItemId != orderItem.OrderItemId))
                {
                    orderEntry.OrderItems.Add(orderItem);
                    orderItem.Book = book;
                    orderItem.Order = orderEntry;
                }

                orderEntry.OrderStatuses.Add(orderStatus);
                orderStatus.Order = orderEntry;

                ordersMap.TryAdd(order.Id, orderEntry);

                return orderEntry;
            },
            param,
            splitOn: "UserId,OrderItemId,BookId,OrderStatusesId");

        return result.DistinctBy(x => x.Id).Select(x => (Order)x).ToList();
    }

    public async Task AddOrderItem(OrderItem orderItem)
    {
        const string sql =
            "INSERT INTO Order_Items (Id, Order_Id, Book_Id, Price, Quantity) VALUES (@Id, @OrderId, @BookId, @Price, @Quantity)";
        await _dapperWrapper.ExecuteAsync(sql, new
        {
            orderItem.Id,
            OrderId = orderItem.Order.Id,
            BookId = orderItem.Book.Id,
            orderItem.Price,
            orderItem.Quantity
        });
    }

    public async Task AddOrderStatus(OrderStatus orderStatus)
    {
        const string sql =
            "INSERT INTO Order_Statuses (Id, Order_Id, order_state, Created_At) VALUES (@Id, @OrderId, @State, @CreatedAt)";
        await _dapperWrapper.ExecuteAsync(sql, new
        {
            orderStatus.Id,
            OrderId = orderStatus.Order.Id,
            orderStatus.State,
            orderStatus.CreatedAt
        });
    }

    public Task<IEnumerable<OrderStatus>> GetOrderStatuses(Guid orderId)
    {
        var sql = """
                  SELECT Id, Order_Id as OrderId, order_state as OrderState, Created_At as CreatedAt
                  FROM Order_Statuses os where os.order_id = @OrderId
                  """;
        return _dapperWrapper.QueryAsync<OrderStatus>(sql, new { OrderId = orderId });
    }
}