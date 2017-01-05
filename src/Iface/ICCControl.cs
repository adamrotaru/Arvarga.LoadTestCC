using System;

namespace LoadTestCC.Proto1.Iface
{
    /// <summary>
    /// Command-and-Control, control interface
    /// </summary>
    public interface ICCControl : ICCAgent
    {
        void Start();
        void Stop();
        void SetTarget(long newTargetCount);
        string GetStatus();
    }
}