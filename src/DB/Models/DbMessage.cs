using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.DB.Models
{
    [Table(DbTables.Messages)]
    public class DbMessage
    {
        [Key]
        public string Id { get; set; }
        public DateTime DtsCreate { get; set; }
        public string Phone { get; set; }
        public int AppId { get; set; } = Constants.AppConstants.AppId;
        public string Text { get; set; }
    }
}