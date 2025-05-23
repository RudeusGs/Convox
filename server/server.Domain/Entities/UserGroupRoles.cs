using server.Domain.Base;
namespace server.Domain.Entities
{

    public class UserGroupRoles : EntityBase
    {
        public int UserId { get; set; }
        public string GroupId { get; set; }
        public string RoleName { get; set; }
    }
}