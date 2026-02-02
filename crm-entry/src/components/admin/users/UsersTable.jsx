const UsersTable = ({
  users = [],
  permissions = [],
  currentUserId,
  onLock,
  onUnlock,
  onSelectUser,
}) => {
  return (
    <table className="w-full text-sm">
      <thead className="bg-slate-50 text-slate-600">
        <tr>
          <th className="px-4 py-3 text-left">ID</th>
          <th className="px-4 py-3 text-left">Username</th>
          <th className="px-4 py-3 text-left">Email</th>
          {/* <th className="px-4 py-3 text-left">Domain</th> */}

          <th className="px-4 py-3 text-left">Roles</th>
          <th className="px-4 py-3 text-left">Status</th>
          <th className="px-4 py-3 text-right">Admin Control</th>
        </tr>
      </thead>

      <tbody className="divide-y">
        {users.map((u) => {
          const isLocked = u.accountStatus === "LOCKED";
          const isSelf = currentUserId === u.userId;

          const canLock =
            permissions.includes("USER_LOCK") ||
            permissions.includes("CRM_FULL_ACCESS");

          return (
            <tr
              key={u.userId}
              className="hover:bg-slate-50 cursor-pointer"
              onClick={() => onSelectUser(u.userId)}
            >
              {/* ID */}
              <td className="px-4 py-3 text-slate-600">
                {u.userId}
              </td>

              {/* USERNAME */}
              <td className="px-4 py-3 font-medium text-slate-800">
                {u.username}
              </td>

              {/* EMAIL */}
              <td className="px-4 py-3 text-slate-600">
                {u.email}
              </td>

              {/* <td className="px-4 py-3 text-slate-600">
  <span className="inline-block bg-blue-50 text-blue-700 text-xs px-2 py-1 rounded">
    {u.domainName || u.domainCode}
  </span>
</td> */}

              {/* ROLES */}
              <td className="px-4 py-3">
                {u.roles?.length ? (
                  <span className="inline-block bg-slate-100 text-slate-700 text-xs px-2 py-1 rounded">
                    {u.roles.join(", ")}
                  </span>
                ) : (
                  "—"
                )}
              </td>

              {/* STATUS */}
              <td className="px-4 py-3">
                <span
                  className={`px-2 py-1 rounded-full text-xs font-semibold ${
                    isLocked
                      ? "bg-red-100 text-red-700"
                      : "bg-green-100 text-green-700"
                  }`}
                >
                  {isLocked ? "LOCKED" : "ACTIVE"}
                </span>
              </td>

              {/* ACTIONS */}
              <td
                className="px-4 py-3 text-right"
                onClick={(e) => e.stopPropagation()}
              >
                {isSelf ? (
                  <span className="text-xs text-slate-400 italic">
                    You
                  </span>
                ) : !canLock ? (
                  <span className="text-xs text-slate-400 italic">
                    No permission
                  </span>
                ) : isLocked ? (
                  <button
                    onClick={() => onUnlock?.(u.userId)}
                    className="text-green-600 hover:underline text-sm font-medium"
                  >
                    Unlock
                  </button>
                ) : (
                  <button
                    onClick={() => onLock?.(u.userId)}
                    className="text-red-600 hover:underline text-sm font-medium"
                  >
                    Lock
                  </button>
                )}
              </td>
            </tr>
          );
        })}
      </tbody>
    </table>
  );
};

export default UsersTable;
