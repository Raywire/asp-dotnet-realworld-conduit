using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Conduit.Models
{
    public class Follow : BaseEntity
    {
        public User Following { get; set; }
        public Guid FollowingId { get; set; }
        public User Follower { get; set; }
        public Guid FollowerId { get; set; }
    }
}
