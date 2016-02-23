namespace justinobney.gymbuddy.api.Interfaces
{
    public interface IPostRequestHandler<TRequest, TResponse>
    {
        void Notify(TRequest request, TResponse response);
    }
}