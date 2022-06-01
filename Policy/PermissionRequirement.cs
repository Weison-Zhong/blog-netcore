using Blog2022_netcore.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Blog2022_netcore.Policy
{
    public class PermissionRequirement: IAuthorizationRequirement
    {
        public List<Api> permissions { get; set; }
    }
}
