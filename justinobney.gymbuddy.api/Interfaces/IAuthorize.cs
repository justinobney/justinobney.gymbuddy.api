namespace justinobney.gymbuddy.api.Interfaces
{
    public interface IAuthorizer<in TRequest>
    {
        bool Authorize(TRequest message);
    }
}