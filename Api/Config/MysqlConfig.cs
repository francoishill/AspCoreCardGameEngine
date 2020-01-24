using System.ComponentModel.DataAnnotations;

namespace AspCoreCardGameEngine.Api.Config
{
    public class MysqlConfig
    {
        [Required]
        public string ConnectionString { get; set; }
    }
}