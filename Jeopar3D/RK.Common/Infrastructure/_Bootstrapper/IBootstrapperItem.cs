using System.Threading.Tasks;

namespace RK.Common.Infrastructure
{
    public interface IBootstrapperItem
    {
        /// <summary>
        /// Executes the background action behind this item.
        /// </summary>
        Task Execute();

        /// <summary>
        /// Gets a short description of this item for the UI (e. g. for splash screens).
        /// </summary>
        string Description
        {
            get;
        }
    }
}
