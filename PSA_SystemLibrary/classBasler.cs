using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using HalconLibrary;
using MeiLibrary;
using DefineLibrary;

namespace PSA_SystemLibrary
{
    public class ClassBasler
    {
        [DllImport(@"VisionDll\ProtecGrabberDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void _pvInitialize();

        [DllImport(@"VisionDll\ProtecGrabberDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void _pvCloseVisionMainDlg();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int pSetCallBack(IntPtr pHandle, IntPtr pBuff, int nWidth, int nHeight);

        [DllImport(@"VisionDll\ProtecGrabberDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void _pvSetCallBack(int nCam, [MarshalAs(UnmanagedType.FunctionPtr)] pSetCallBack pCallBack);

        [DllImport(@"VisionDll\ProtecGrabberDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void _pvSetWndHandle(int nCam, IntPtr pHandle);

        [DllImport(@"VisionDll\ProtecGrabberDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void _pvSWTrigger(int nCamIndex);

        [DllImport(@"VisionDll\ProtecGrabberDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void _pvSetTriggerMode(int nCamIndex, bool bSetHardTrigger);

        [DllImport(@"VisionDll\ProtecGrabberDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void _pvSetExpose(int nCamIndex, int nExposeTime_us);

        [DllImport(@"VisionDll\ProtecGrabberDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void _pvGetInit(int nCamIndex);

        public static pSetCallBack ResultCall00;
        public static pSetCallBack ResultCall01;


        public void init()
        {
            _pvInitialize();
            ResultCall00 = ResultCall0;
            ResultCall01 = ResultCall1;
            _pvSetWndHandle(0, IntPtr.Zero);
            _pvSetCallBack(0, ResultCall00);
            _pvSetWndHandle(1, IntPtr.Zero);
            _pvSetCallBack(1, ResultCall01);
        }

        public static int ResultCall0(IntPtr pHandle, IntPtr pBuff, int nWidth, int nHeight)
        {
            
            //HObject Image;

            //HOperatorSet.GenImage1(out Image, "byte", nWidth, nHeight, pBuff);
            //HOperatorSet.WriteImage(Image, "bmp", 0, "D:Test");
            
            return 0;
        }
        
        public static int ResultCall1(IntPtr pHandle, IntPtr pBuff, int nWidth, int nHeight)
        {
            return 0;
        }
    }
}
