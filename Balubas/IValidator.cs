using Balubas.Model;

namespace Balubas
{
    public interface IValidator
    {
        void Validate(TransactionBlock block);
    }
}