using System;

namespace Potman.Common.Pools.Abstractions
{
    public interface IPoolable : IDisposable
    {
        void Release();
    }
}