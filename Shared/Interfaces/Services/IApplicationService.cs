using Ninject;

namespace Shared.Interfaces.Services
{
    public interface IApplicationService
    {
        public IKernel Kernel { get; }
    }
}
