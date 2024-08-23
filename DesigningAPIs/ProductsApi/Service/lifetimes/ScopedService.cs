namespace ProductsApi.Service.lifetimes
{
    public interface IScopedService
    {
        Guid GetOperationID();
    }

    public class ScopedService : IScopedService
    {
        private readonly Guid _operationId;

        //public ScopedService( ITransientService transientService)
        //{
        //    _operationId = Guid.NewGuid();
        //}
        public ScopedService()
        {
            _operationId = Guid.NewGuid();
        }

        public Guid GetOperationID()
        {
            return _operationId;
        }
    }

}
