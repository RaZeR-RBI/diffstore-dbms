namespace Standalone.Nancy
{
    public static class Util
    {
        public static bool ResponseMayHaveBody(string method)
        {
            switch (method)
            {
                case "HEAD":
                case "PATCH":
                case "PUT":
                    return false;
                default: return true;
            }
        }
    }
}