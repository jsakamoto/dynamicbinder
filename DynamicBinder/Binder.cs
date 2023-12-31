namespace Toolbelt
{
    internal static class Binder
    {
        internal static void UnwrapBinder(object[] args)
        {
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] is DynamicBinder dynamicBinder) args[i] = dynamicBinder.Object;
                else if (args[i] is LateBinder lateBinder) args[i] = lateBinder.Object;
            }
        }

    }
}
