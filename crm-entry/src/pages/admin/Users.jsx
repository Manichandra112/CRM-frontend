import { useEffect, useState, useCallback } from "react";
import { useAuth } from "../../auth/AuthContext";

import CreateUser from "../../components/admin/users/CreateUser";
import UsersTable from "../../components/admin/users/UsersTable";
import UserDrawer from "../../components/admin/users/UserDrawer";

import { getAdminUsers } from "../../api/admin/users.api";
import { lockUser, unlockUser } from "../../api/users/users.api";
import { getDomains } from "../../api/admin/domains.api";
import { getAdminRoles } from "../../api/admin/roles.api";

import useTableFilters from "../../hooks/useTableFilters";

export default function Users() {
  const { permissions, user } = useAuth();

  /* STATE */
  const [users, setUsers] = useState([]);
  const [domains, setDomains] = useState([]);
  const [roles, setRoles] = useState([]);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const [selectedUserId, setSelectedUserId] = useState(null);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [createOpen, setCreateOpen] = useState(false);

  /* FILTERS */
  const [search, setSearch] = useState("");
  const [domainCode, setDomainCode] = useState("");
  const [roleCode, setRoleCode] = useState("");
  const [status, setStatus] = useState("");

  /* PAGINATION */
  const [page, setPage] = useState(1);
  const pageSize = 25;

  /* FETCH USERS */
  const loadUsers = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);

      const res = await getAdminUsers({ page, pageSize });

      // Supports both wrapper and raw array
      setUsers(res?.users ?? res ?? []);
    } catch (err) {
      console.error("Users fetch error:", err);
      setError("Failed to load users");
    } finally {
      setLoading(false);
    }
  }, [page, pageSize]);

  useEffect(() => {
    loadUsers();
  }, [loadUsers]);

  /* FETCH DOMAINS + ROLES */
  useEffect(() => {
    const loadReferenceData = async () => {
      try {
        const [domainsData, rolesData] = await Promise.all([
          getDomains(),
          getAdminRoles(),
        ]);

        setDomains(Array.isArray(domainsData) ? domainsData : []);
        setRoles(Array.isArray(rolesData) ? rolesData : []);
      } catch (err) {
        console.error("Reference data error:", err);
        setDomains([]);
        setRoles([]);
      }
    };

    loadReferenceData();
  }, []);

  /* FILTER USERS */
  const filteredUsers = useTableFilters(users, {
    search,
    domainCode,
    roleCode,
    status,
  });

  /* ACTIONS */
  const handleLock = async (userId) => {
    const reason = window.prompt(
      "Reason for locking this user?",
      "Violation of policy"
    );
    if (!reason) return;

    await lockUser(userId, reason);
    loadUsers();
  };

  const handleUnlock = async (userId) => {
    if (!window.confirm("Unlock this user?")) return;
    await unlockUser(userId);
    loadUsers();
  };

  const canLock =
    permissions?.includes("USER_LOCK") ||
    permissions?.includes("CRM_FULL_ACCESS");

  const canCreate =
    permissions?.includes("USER_CREATE") ||
    permissions?.includes("CRM_FULL_ACCESS");

  if (loading) {
    return <div className="text-sm text-slate-500">Loading users…</div>;
  }

  return (
    <div className="space-y-6">
      {/* HEADER */}
      <div className="flex justify-between items-center">
        <div>
          <h2 className="text-xl font-semibold text-slate-800">Users</h2>
          <p className="text-sm text-slate-500">
            Manage users, roles, domains and access
          </p>
        </div>

        {canCreate && (
          <button
            onClick={() => setCreateOpen(true)}
            className="bg-blue-600 text-white px-4 py-2 rounded text-sm"
          >
            Create User
          </button>
        )}
      </div>

      {/* ERROR */}
      {error && (
        <div className="bg-red-50 border border-red-200 text-red-700 text-sm rounded px-4 py-2">
          {error}
        </div>
      )}

      {/* FILTER BAR */}
      <div className="bg-white border border-slate-200 rounded-md p-4 flex flex-wrap gap-4">
        <input
          type="text"
          placeholder="Search name or email"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="border rounded px-3 py-2 text-sm w-56"
        />

        <select
          value={domainCode}
          onChange={(e) => setDomainCode(e.target.value)}
          className="border rounded px-3 py-2 text-sm"
        >
          <option value="">All Domains</option>
          {domains.map((d) => (
            <option key={d.domainId} value={d.domainCode}>
              {d.domainName}
            </option>
          ))}
        </select>

        <select
          value={roleCode}
          onChange={(e) => setRoleCode(e.target.value)}
          className="border rounded px-3 py-2 text-sm"
        >
          <option value="">All Roles</option>
          {roles.map((r) => (
            <option key={r.roleCode} value={r.roleCode}>
              {r.roleName}
            </option>
          ))}
        </select>

        <select
          value={status}
          onChange={(e) => setStatus(e.target.value)}
          className="border rounded px-3 py-2 text-sm"
        >
          <option value="">All Status</option>
          <option value="ACTIVE">Active</option>
          <option value="LOCKED">Locked</option>
        </select>
      </div>

      {/* USERS TABLE */}
      <UsersTable
        users={filteredUsers}
        permissions={permissions}
        currentUserId={Number(user?.sub)}
        onLock={canLock ? handleLock : undefined}
        onUnlock={canLock ? handleUnlock : undefined}
        onSelectUser={(id) => {
          setSelectedUserId(id);
          setDrawerOpen(true);
        }}
      />

      {/* USER DRAWER */}
      <UserDrawer
        open={drawerOpen}
        userId={selectedUserId}
        onClose={() => setDrawerOpen(false)}
        onUserUpdated={loadUsers}
      />

      {/* CREATE USER */}
      {createOpen && (
        <CreateUser
          onSuccess={() => {
            setCreateOpen(false);
            loadUsers();
          }}
          onClose={() => setCreateOpen(false)}
        />
      )}
    </div>
  );
}
