namespace ProductsApi.Service.lifetimes
{
    public interface ISingletonService
    {
        Guid GetOperationID();
    }

    public class SingletonService : ISingletonService
    {
        private readonly Guid _operationId;

        public SingletonService()
        {
            _operationId = Guid.NewGuid();
        }

        public Guid GetOperationID()
        {
            return _operationId;
        }
    }

}
