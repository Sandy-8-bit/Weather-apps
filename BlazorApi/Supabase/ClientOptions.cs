
namespace Supabase
{
    internal class ClientOptions : SupabaseOptions
    {
        public new bool AutoRefreshToken { get; set; }

        public bool? PersistSession { get; set; }
    }
}