using System;
using System.Runtime.InteropServices;

namespace RNNoiseSharp
{
    internal static class RNNoiseSharpWrapper
    {
#if Android
        const string DllName = "librnnoise.so";
#else
        const string DllName = "__Internal";
#endif

        internal const int FRAME_SIZE = 480;

        internal const float SIGNAL_SCALE = short.MaxValue;

        internal const float SIGNAL_SCALE_INV = 1f / short.MaxValue;

        [DllImport(DllName, EntryPoint = "rnnoise_get_size")]
        internal static extern int rnnoise_get_size();

        [DllImport(DllName, EntryPoint = "rnnoise_init")]
        internal static extern int rnnoise_init(IntPtr state, IntPtr model);

        [DllImport(DllName, EntryPoint = "rnnoise_create")]
        internal static extern IntPtr rnnoise_create(IntPtr model);

        [DllImport(DllName, EntryPoint = "rnnoise_destroy")]
        internal static extern void rnnoise_destroy(IntPtr state);

        [DllImport(DllName, EntryPoint = "rnnoise_process_frame")]
        internal static extern unsafe float rnnoise_process_frame(IntPtr state, float* dataOut, float* dataIn);
    }
}