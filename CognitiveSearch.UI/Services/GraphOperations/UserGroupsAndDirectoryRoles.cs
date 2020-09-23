using Microsoft.Graph;
using System.Collections.Generic;

namespace CognitiveSearch.UI.Services.GraphOperations
{
    public class UserGroupsAndDirectoryRoles
    {
        public UserGroupsAndDirectoryRoles()
        {
            this.GroupIds = new List<string>();
            this.Groups = new List<Group>();
            this.DirectoryRoles = new List<DirectoryRole>();
        }

        /// <summary>Gets or sets a value indicating whether this user's groups claim will result in an overage </summary>
        /// <value>
        ///   <c>true</c> if this instance has overage claim; otherwise, <c>false</c>.</value>
        public bool HasOverageClaim { get; set; }

        /// <summary>Gets or sets the group ids.</summary>
        /// <value>The group ids.</value>
        public List<string> GroupIds { get; set; }

        /// <summary>Gets or sets the groups.</summary>
        /// <value>The groups.</value>
        public List<Group> Groups { get; set; }

        /// <summary>Gets or sets the App roles</summary>
        public List<DirectoryRole> DirectoryRoles { get; set; }

    }
}
