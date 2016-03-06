using System;
using System.Threading;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace CodeSnippetLibrary.Timer
{
    public class CodeTimer
    {
        //引用Win32 API中的QueryPerformanceCounter()方法
        //该方法用来查询任意时刻高精度计数器的实际值
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        //引用Win32 API 中的QueryPerformanceFrequency()方法
        //该方法返回高精度计数器每秒的计数值
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        public CodeTimer()
        {
            _startTime = 0;
            _stopTime = 0;

            if (!QueryPerformanceFrequency(out _frequency))
            {
                throw new Win32Exception();
            }

            //让等待线程工作
            Thread.Sleep(0);
            QueryPerformanceCounter(out _startTime);
        }

        /// <summary>
        /// 停止计时
        /// </summary>
        public TimeSpan Count()
        {
            QueryPerformanceCounter(out _stopTime);
            var ms = (double)(_stopTime - _startTime) * 1000 / _frequency;
            return TimeSpan.FromMilliseconds(ms);
        }

        private readonly long _startTime;
        private long _stopTime;
        private readonly long _frequency;
    }
}