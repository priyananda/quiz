using System;
using System.Threading.Tasks;

namespace RK.Common.Infrastructure
{
    public interface IErrorDialogService
    {
        /// <summary>
        /// Shows the given unhandled exception to the enduser.
        /// </summary>
        /// <param name="ex">The exception to be shown.</param>
        Task ShowException(Exception ex);
    }
}
