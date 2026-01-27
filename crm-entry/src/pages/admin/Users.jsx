import { useEffect, useState } from "react";
import { useAuth } from "../../auth/AuthContext";
import {
  getAdminUsers,
  lockUser,
  unlockUser,
} from "../../api/admin.api";

import UsersTable from "../../components/admin/users/UsersTable";
import UserDrawer from "../../components/admin/users/UserDrawer";

const Users = () => {
  const { permissions, user } = useAuth();

  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedUserId, setSelectedUserId] = useState(null);

  useEffect(() => {
    fetchUsers();
  }, []);

  const fetchUsers = async () => {
    try {
      setLoading(true);
      const res = await getAdminUsers(1, 25);
      setUsers(res.data.users);
    } catch (err) {
      console.error(err);
      setError("Failed to load users");
    } finally {
      setLoading(false);
    }
  };

  const handleLock = async (userId) => {
    if (!window.confirm("Lock this user?")) return;
    await lockUser(userId);
    fetchUsers();
  };

  const handleUnlock = async (userId) => {
    if (!window.confirm("Unlock this user?")) return;
    await unlockUser(userId);
    fetchUsers();
  };

  if (loading) return <p>Loading users...</p>;
  if (error) return <p style={{ color: "red" }}>{error}</p>;

  return (
    <>
      <h2>User Management</h2>

      <UsersTable
        users={users}
        permissions={permissions}
        currentUserId={Number(user?.sub)}
        onLock={handleLock}
        onUnlock={handleUnlock}
        onSelectUser={setSelectedUserId}
      />

      <UserDrawer
        userId={selectedUserId}
        onClose={() => setSelectedUserId(null)}
      />
    </>
  );
};

export default Users;
