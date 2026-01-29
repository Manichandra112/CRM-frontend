import { useEffect, useState } from "react";
import {
  getAdminRoles,
} from "../../../../api/admin/roles.api";
import {
  getAdminUserRoles,
  assignUserRole,
  removeUserRole,
} from "../../../../api/admin/userRoles.api";

export default function RolesTab({ user }) {
  const [roles, setRoles] = useState([]);
  const [userRoles, setUserRoles] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!user) return;
    loadRoles();
  }, [user]);

  const loadRoles = async () => {
    setLoading(true);
    const [rolesRes, userRolesRes] = await Promise.all([
      getAdminRoles(),
      getAdminUserRoles(user.userId),
    ]);

    setRoles(rolesRes.data || []);
    setUserRoles(userRolesRes.data || []);
    setLoading(false);
  };

  const handleAssign = async (roleCode) => {
    await assignUserRole({
      userId: user.userId,
      roleCode,
    });
    loadRoles();
  };

  const handleRemove = async (roleCode) => {
    await removeUserRole({
      userId: user.userId,
      roleCode,
    });
    loadRoles();
  };

  if (loading) return <p className="text-sm">Loading roles…</p>;

  return (
    <div className="space-y-4 text-sm">
      <h4 className="font-semibold">Assigned Roles</h4>

      {userRoles.length === 0 && (
        <p className="text-slate-500">No roles assigned</p>
      )}

      {userRoles.map((r) => (
        <div
          key={r.roleCode}
          className="flex justify-between items-center border p-2 rounded"
        >
          <span>{r.roleName}</span>
          <button
            onClick={() => handleRemove(r.roleCode)}
            className="text-red-600 text-xs"
          >
            Remove
          </button>
        </div>
      ))}

      <h4 className="font-semibold pt-2">Assign New Role</h4>

      <select
        className="border rounded px-2 py-1"
        onChange={(e) => handleAssign(e.target.value)}
        defaultValue=""
      >
        <option value="" disabled>
          Select role
        </option>
        {roles.map((r) => (
          <option key={r.code} value={r.code}>
            {r.name}
          </option>
        ))}
      </select>
    </div>
  );
}
