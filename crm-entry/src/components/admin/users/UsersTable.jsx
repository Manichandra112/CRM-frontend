const UsersTable = ({
  users,
  permissions,
  currentUserId,
  onLock,
  onUnlock,
  onSelectUser,
}) => {
  return (
    <table border="1" cellPadding="8" cellSpacing="0" width="100%">
      <thead>
        <tr>
          <th>ID</th>
          <th>Username</th>
          <th>Email</th>
          <th>Roles</th>
          <th>Status</th>
          <th>Actions</th>
        </tr>
      </thead>

      <tbody>
        {users.map((u) => {
          const isLocked = u.accountStatus === "LOCKED";
          const isSelf = currentUserId === u.userId;

          return (
            <tr
              key={u.userId}
              onClick={() => onSelectUser(u.userId)}
              style={{ cursor: "pointer" }}
            >
              <td>{u.userId}</td>
              <td>{u.username}</td>
              <td>{u.email}</td>
              <td>{u.roles?.join(", ")}</td>
              <td>{u.accountStatus}</td>
              <td onClick={(e) => e.stopPropagation()}>
                {isSelf ? (
                  "—"
                ) : (
                  <>
                    {!isLocked &&
                      permissions.includes("USER_LOCK") && (
                        <button onClick={() => onLock(u.userId)}>
                          Lock
                        </button>
                      )}
                    {isLocked &&
                      permissions.includes("USER_UNLOCK") && (
                        <button onClick={() => onUnlock(u.userId)}>
                          Unlock
                        </button>
                      )}
                  </>
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
