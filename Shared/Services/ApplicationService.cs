using Ninject;
using Shared.Interfaces.Services;

namespace Shared.Services
{
    public class ApplicationService : IApplicationService
    {
        public ApplicationService(IKernel kernel)
        {
            this.Kernel = kernel;
        }

        public IKernel Kernel { get; }
    }
}
