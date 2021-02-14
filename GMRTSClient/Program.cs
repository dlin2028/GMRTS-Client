using System;

namespace GMRTSClient
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            //comment
            using (var game = new Game1())
                game.Run();
        }
    }
}
