using System;
using System.Runtime.InteropServices;
using System.Text;

namespace OSCVRCWiz.Resources.Audio.SoundTouch
{
    class SoundTouchInterop64
    {
        private const string SoundTouchDllName = "SoundTouch_x64.dll";

        [DllImport(SoundTouchDllName)]
        public static extern IntPtr soundtouch_createInstance();

        [DllImport(SoundTouchDllName)]
        public static extern void soundtouch_destroyInstance(IntPtr h);

        [DllImport(SoundTouchDllName)]
        public static extern void soundtouch_getVersionString2(StringBuilder versionString, int bufferSize);

        [DllImport(SoundTouchDllName)]
        public static extern uint soundtouch_getVersionId();

        [DllImport(SoundTouchDllName)]
        public static extern void soundtouch_setRate(IntPtr h, float newRate);

        [DllImport(SoundTouchDllName)]
        public static extern void soundtouch_setTempo(IntPtr h, float newTempo);

        [DllImport(SoundTouchDllName)]
        public static extern void soundtouch_setRateChange(IntPtr h, float newRate);

        [DllImport(SoundTouchDllName)]
        public static extern void soundtouch_setTempoChange(IntPtr h, float newTempo);

        [DllImport(SoundTouchDllName)]
        public static extern void soundtouch_setPitch(IntPtr h, float newPitch);

        [DllImport(SoundTouchDllName)]
        public static extern void soundtouch_setPitchOctaves(IntPtr h, float newPitch);

        [DllImport(SoundTouchDllName)]
        public static extern void soundtouch_setPitchSemiTones(IntPtr h, float newPitch);

        [DllImport(SoundTouchDllName)]
        public static extern void soundtouch_setChannels(IntPtr h, uint numChannels);

        [DllImport(SoundTouchDllName)]
        public static extern void soundtouch_setSampleRate(IntPtr h, uint srate);

        [DllImport(SoundTouchDllName)]
        public static extern void soundtouch_flush(IntPtr h);

        [DllImport(SoundTouchDllName)]
        public static extern void soundtouch_putSamples(IntPtr h, [MarshalAs(UnmanagedType.LPArray)] float[] samples, int numSamples);

        [DllImport(SoundTouchDllName)]
        public static extern void soundtouch_clear(IntPtr h);

        [DllImport(SoundTouchDllName)]
        public static extern bool soundtouch_setSetting(IntPtr h, SoundTouchSettings settingId, int value);

        [DllImport(SoundTouchDllName)]
        public static extern int soundtouch_getSetting(IntPtr h, SoundTouchSettings settingId);

        [DllImport(SoundTouchDllName)]
        public static extern int soundtouch_numUnprocessedSamples(IntPtr h);

        [DllImport(SoundTouchDllName)]
        public static extern uint soundtouch_receiveSamples(IntPtr h, [MarshalAs(UnmanagedType.LPArray)] float[] outBuffer, uint maxSamples);

        [DllImport(SoundTouchDllName)]
        public static extern uint soundtouch_numSamples(IntPtr h);

        [DllImport(SoundTouchDllName)]
        public static extern int soundtouch_isEmpty(IntPtr h);

    }
}