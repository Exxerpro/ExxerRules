using ObjCRuntime;
using UIKit;

namespace IndTrace.Challenge
{
    /// <summary>
    /// Represents the Program.
    /// </summary>
    public class Program
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, typeof(AppDelegate));
        }
    }
}
