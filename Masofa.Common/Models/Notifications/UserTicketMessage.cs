using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.Common.Models.Notifications
{
    public class UserTicketMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid UserTicketId { get; set; }
        public Guid? EmailId { get; set; }
        public string Message { get; set; }
        public DateTime Created { get; set; }

        [NotMapped]
        public List<Guid> AttachmentIds { get; set; } //ссылка на FileStorageItems

        public string AttachmentIdsJson { get; set; }
    }
}
