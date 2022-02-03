using System.Collections.Generic;

namespace WebApplication.DB.Models
{
    public static class DbTables
    {
        public const string Messages = "messages";

        public static readonly Dictionary<string, string> Schemas = new()
        {
            {
                Messages,
                $"{nameof(DbMessage.Id)} PRIMARY KEY NOT NULL, {nameof(DbMessage.DtsCreate)}, {nameof(DbMessage.Phone)}, {nameof(DbMessage.AppId)}, {nameof(DbMessage.Text)}"
            }
        };
    }
}