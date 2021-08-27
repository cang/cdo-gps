using System;
using System.Threading.Tasks;

namespace Core.Utils.Delay
{
    public interface IRequestManager
    {
        long CreateRequest(Delegate callback, TimeSpan timeout);
        bool ResetTimeOut(long id);
        bool RemoveRequest(long id);
        bool ExcuteReponse(long id, Action<Delegate> action);
        Task<bool> Wait(long id);
        bool Contain(long id);
    }
}