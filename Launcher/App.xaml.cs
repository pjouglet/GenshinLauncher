using Newtonsoft.Json;
using System;
using System.Net;
using System.Windows;

namespace Launcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        System.Threading.Mutex mutex;
        protected override void OnStartup(StartupEventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslPolicyErrors) => true;


            AppDomain currentDomain = AppDomain.CurrentDomain;
            // 当前作用域出现未捕获异常时，使用MyHandler函数响应事件
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);


            //单例
            bool ret;
            mutex = new System.Threading.Mutex(true, "ElectronicNeedleTherapySystem", out ret);

            if (!ret)
            {
                MessageBox.Show(Launcher.Resources.Strings.ONLY_ONE_INSTANCE_ALLOWED);
                Environment.Exit(0);
            }


            base.OnStartup(e);
        }
        static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            Console.WriteLine("UnHandled Exception Caught : " + e.Message);
            Console.WriteLine("Runtime terminating: {0}", args.IsTerminating);

            MessageBox.Show(e.Message + "\n" + Launcher.Resources.Strings.ERROR_SUBMITION, Launcher.Resources.Strings.PROGRAM_CRASHED);
            System.IO.File.WriteAllText("err.log", e.Message + JsonConvert.SerializeObject(e));
            Environment.Exit(0);

        }
        protected override void OnExit(ExitEventArgs e)
        {

            if (GlobalValues.MainWindow.vm.proxyController != null)
                GlobalValues.MainWindow.vm.proxyController.Stop();

            base.OnExit(e);
        }
    }
}
