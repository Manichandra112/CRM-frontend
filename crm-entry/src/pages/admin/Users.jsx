import {
  useEffect,
  useState,
  useCallback,
} from "react";
import { useAuth } from "../../auth/AuthContext";

import { getAdminUsers } from "../../api/admin/users.api";
import { lockUser, unlockUser } from "../../api/users/users.api";
import { getDomains } from "../../api/admin/domains.api";
import { getAdminRoles } from "../../api/admin/roles.api";

import UsersTable from "../../components/admin/users/UsersTable";
import UserDrawer from "../../components/admin/users/UserDrawer";

import useTableFilters from "../../hooks/useTableFilters";

export default function Users() {
  const { permissions, user } = useAuth();

  /* =======================
     CORE STATE
     ======================= */
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const [selectedUserId, setSelectedUserId] = useState(null);
  const [drawerOpen, setDrawerOpen] = useState(false);

  /* =======================
     FILTER STATE
     ======================= */
  const [search, setSearch] = useState("");
  const [domainCode, setDomainCode] = useState("");
  const [roleCode, setRoleCode] = useState("");
  const [status, setStatus] = useState("");

  /* =======================
     REFERENCE DATA
     ======================= */
  const [domains, setDomains] = useState([]);
  const [roles, setRoles] = useState([]);

  /* =======================
     PAGINATION
     ======================= */
  const [page, setPage] = useState(1);
  const pageSize = 25;

  /* =======================
     FETCH USERS
     ======================= */
  const loadUsers = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);

      const res = await getAdminUsers({
        page,
        pageSize,
      });

      setUsers(res.data.users || []);
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

  /* =======================
     FETCH DOMAINS & ROLES
     ======================= */
  useEffect(() => {
    const loadReferenceData = async () => {
      try {
        const [domainsRes, rolesRes] = await Promise.all([
          getDomains(),
          getAdminRoles(),
        ]);

        setDomains(domainsRes.data || []);
        setRoles(rolesRes.data || []);
      } catch (err) {
        console.error("Failed to load domains / roles", err);
      }
    };

    loadReferenceData();
  }, []);

  /* =======================
     FILTERED USERS
     ======================= */
  const filteredUsers = useTableFilters(
    users,
    {
      search,
      domainCode,
      roleCode,
      status,
    }
  );

  /* =======================
     ACTIONS
     ======================= */
  const handleLock = async (userId) => {
    const reason = window.prompt(
      "Reason for locking this user?",
      "Violation of policy"
    );
    if (!reason) return;

    await lockUser(userId, reason);
    await loadUsers();
  };

  const handleUnlock = async (userId) => {
    if (!window.confirm("Unlock this user?")) return;
    await unlockUser(userId);
    await loadUsers();
  };

  const handleSelectUser = (userId) => {
    setSelectedUserId(userId);
    setDrawerOpen(true);
  };

  const handleCloseDrawer = () => {
    setDrawerOpen(false);
    setSelectedUserId(null);
  };

  /* =======================
     PERMISSIONS
     ======================= */
  const canLock =
    permissions?.includes("USER_LOCK") ||
    permissions?.includes("CRM_FULL_ACCESS");

  /* =======================
     UI
     ======================= */
  if (loading) {
    return (
      <div className="text-sm text-slate-500">
        Loading users…
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* HEADER */}
      <div>
        <h2 className="text-xl font-semibold text-slate-800">
          Users
        </h2>
        <p className="text-sm text-slate-500">
          Manage users, roles, domains and access
        </p>
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

        {/* DOMAINS */}
        <select
          value={domainCode}
          onChange={(e) => setDomainCode(e.target.value)}
          className="border rounded px-3 py-2 text-sm"
        >
          <option value="">All Domains</option>
          {domains.map((d) => (
            <option
              key={d.domainId}
              value={d.domainCode}
            >
              {d.domainName}
            </option>
          ))}
        </select>

        {/* ROLES */}
        <select
          value={roleCode}
          onChange={(e) => setRoleCode(e.target.value)}
          className="border rounded px-3 py-2 text-sm"
        >
          <option value="">All Roles</option>
          {roles.map((r) => (
            <option
              key={r.roleCode}
              value={r.roleCode}
            >
              {r.roleName}
            </option>
          ))}
        </select>

        {/* STATUS */}
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
      <div className="bg-white border border-slate-200 rounded-md overflow-hidden">
        <UsersTable
          users={filteredUsers}
          permissions={permissions}
          currentUserId={Number(user?.sub)}
          onLock={canLock ? handleLock : undefined}
          onUnlock={canLock ? handleUnlock : undefined}
          onSelectUser={handleSelectUser}
        />
      </div>

      {/* PAGINATION */}
      <div className="flex justify-between items-center text-sm">
        <button
          type="button"
          disabled={page === 1}
          onClick={() => setPage((p) => p - 1)}
          className="px-3 py-1 border rounded disabled:opacity-50"
        >
          Previous
        </button>

        <span className="text-slate-600">
          Page {page}
        </span>

        <button
          type="button"
          disabled={users.length < pageSize}
          onClick={() => setPage((p) => p + 1)}
          className="px-3 py-1 border rounded disabled:opacity-50"
        >
          Next
        </button>
      </div>

      {/* USER DRAWER */}
      <UserDrawer
        open={drawerOpen}
        userId={selectedUserId}
        onClose={handleCloseDrawer}
        onUserUpdated={loadUsers}
      />
    </div>
  );
}
