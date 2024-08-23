namespace ProductsApi.Service.lifetimes
{
    public interface ITransientService
    {
        Guid GetOperationID();
    }

    public class TransientService : ITransientService
    {
        private readonly Guid _operationId;

        public TransientService()
        {
            _operationId = Guid.NewGuid();
        }

        public Guid GetOperationID()
        {
            return _operationId;
        }
    }

}
