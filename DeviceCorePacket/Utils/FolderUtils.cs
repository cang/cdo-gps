using System.IO;
using System.Security.AccessControl;

namespace CorePacket.Utils
{
    /// <summary>
    /// hỗ trợ các tùy chỉnh folder
    /// </summary>
    public static class FolderUtils
    {
        /// <summary>
        /// cấp full quyền truy cập cho folder
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool SetFullPermission(string path)
        {
            const FileSystemRights rights = FileSystemRights.FullControl;

            // *** Add Access Rule to the actual directory itself
            var accessRule = new FileSystemAccessRule("Users", rights,
                InheritanceFlags.None,
                PropagationFlags.NoPropagateInherit,
                AccessControlType.Allow);

            var info = new DirectoryInfo(path);
            var security = info.GetAccessControl(AccessControlSections.Access);

            bool result;
            security.ModifyAccessRule(AccessControlModification.Set, accessRule, out result);

            if (!result)
                return false;

            // *** Always allow objects to inherit on a directory
            var iFlags = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;

            // *** Add Access rule for the inheritance
            accessRule = new FileSystemAccessRule("Users", rights,
                iFlags,
                PropagationFlags.InheritOnly,
                AccessControlType.Allow);
            security.ModifyAccessRule(AccessControlModification.Add, accessRule, out result);

            if (!result)
                return false;

            info.SetAccessControl(security);

            return true;
        }
    }
}