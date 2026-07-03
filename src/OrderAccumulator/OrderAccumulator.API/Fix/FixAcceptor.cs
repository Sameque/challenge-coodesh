using QuickFix;
using QuickFix.FIX44;
using OrderAccumulator.Application.DTOs;
using OrderAccumulator.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OrderAccumulator.API.Fix;

public class FixAcceptor : QuickFix.Application
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<FixAcceptor> _logger;

    public FixAcceptor(IServiceProvider serviceProvider, ILogger<FixAcceptor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public override void FromApp(Message message, SessionID sessionID)
    {
        if (message is NewOrderSingle orderMsg)
        {
            _logger.LogInformation("FIX Order received: {Symbol}, {Qty}, {Price}",
                orderMsg.Symbol.Value, orderMsg.OrderQty.Value, orderMsg.Price.Value);

            using var scope = _serviceProvider.CreateScope();
            var useCase = scope.ServiceProvider.GetRequiredService<ProcessOrderUseCase>();

            var request = new OrderRequest(
                Symbol: orderMsg.Symbol.Value,
                Quantity: (decimal)orderMsg.OrderQty.Value,
                Price: (decimal)orderMsg.Price.Value,
                Side: orderMsg.Side.Value == Side.BUY ? "buy" : "sell"
            );

            var response = useCase.ExecuteAsync(request).GetAwaiter().GetResult();

            if (response.Accepted)
            {
                SendExecutionReport(sessionID, orderMsg, "Accepted");
            }
            else
            {
                SendExecutionReport(sessionID, orderMsg, "Rejected");
            }
        }
    }

    private void SendExecutionReport(SessionID sessionID, NewOrderSingle orderMsg, string status)
    {
        var report = new ExecutionReport(
            new ClOrdID(orderMsg.ClOrdID.Value),
            new OrderID("ACC-123"),
            new ExecID(Guid.NewGuid().ToString()),
            new ExecTransactTime(DateTime.UtcNow),
            new ExecType(ExecType.TRADE),
            new OrdStatus(status == "Accepted" ? OrdStatus.FILLED : OrdStatus.REJECTED),
            new Symbol(orderMsg.Symbol.Value),
            new Side(orderMsg.Side.Value),
            new LeavesQty(0),
            new CumQty(orderMsg.OrderQty.Value),
            new AvgPx(orderMsg.Price.Value)
        );

        Session.SendToTarget(report, sessionID);
    }

    public override void OnCreate(SessionID sessionID) { }
    public override void OnLogon(SessionID sessionID) { }
    public override void OnLogout(SessionID sessionID) { }
    public override void ToAdmin(QuickFix.Message message, SessionID sessionID) { }
    public override void FromAdmin(QuickFix.Message message, SessionID sessionID) { }
    public override void ToApp(QuickFix.Message message, SessionID sessionID) { }
}
