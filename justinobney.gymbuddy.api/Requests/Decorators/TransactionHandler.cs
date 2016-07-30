using System;
using System.Transactions;
using justinobney.gymbuddy.api.Data;
using MediatR;
using Serilog;


namespace justinobney.gymbuddy.api.Requests.Decorators
{
    public class DoNotCommit : Attribute
    {
    }

    public class TransactionHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _inner;
        private readonly ILogger _log;
        private readonly AppContext _context;

        public TransactionHandler(
            IRequestHandler<TRequest, TResponse> inner,
            ILogger log,
            AppContext context
            )
        {
            _inner = inner;
            _log = log;
            _context = context;
        }

        public TResponse Handle(TRequest message)
        {
            _log.Information("Entering transaction handler");
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            };

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                var response = _inner.Handle(message);

                transaction.Complete();
                _context.SaveChanges();

                _log.Information("Completed transaction");

                return response;
            }
        }
    }
}