namespace Common.Utils
{
    public static class ConnectionIdUtils
    {
        public static string Shorten(string connectionId)
        {
            if (string.IsNullOrEmpty(connectionId))
                return "<none>";
            return connectionId[^3..];
        }
    }
}
