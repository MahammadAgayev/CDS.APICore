using System;

namespace CDS.APICore.Helpers
{
    public static class SafeActionExecutor
    {
        public static void Execute(Action action)
        {
            try
            {
                action?.Invoke();
            }
            catch { }
        }
    }
}
