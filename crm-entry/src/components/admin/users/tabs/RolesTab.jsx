import { useEffect, useState } from "react";

import { getAdminRoles } from "../../../../api/admin/roles.api";
import {
  getUserRoles,
  assignUserRole,
  removeUserRole,
} from "../../../../api/admin/roles.api";

export default function RolesTab({ userId, onUpdated }) {
  const [allRoles, setAllRoles] = useState([]);
  const [assignedRoleCodes, setAssignedRoleCodes] = useState(new Set());

  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(false);

  /* =======================
     LOAD ROLES
     ======================= */
  useEffect(() => {
    const load = async () => {
      try {
        setLoading(true);
        setError(null);

        const [rolesRes, userRolesRes] = await Promise.all([
          getAdminRoles(),
          getUserRoles(userId),
        ]);

        setAllRoles(rolesRes.data || []);

        const roleCodes = new Set(
          (userRolesRes.data || []).map((r) => r.roleCode)
        );
        setAssignedRoleCodes(roleCodes);
      } catch (err) {
        console.error("Role load error:", err);
        setError("Failed to load roles");
      } finally {
        setLoading(false);
      }
    };

    load();
  }, [userId]);

  /* =======================
     TOGGLE ROLE
     ======================= */
  const toggleRole = (roleCode) => {
    setAssignedRoleCodes((prev) => {
      const next = new Set(prev);
      if (next.has(roleCode)) next.delete(roleCode);
      else next.add(roleCode);
      return next;
    });
    setSuccess(false);
  };

  /* =======================
     SAVE CHANGES
     ======================= */
  const handleSave = async () => {
    try {
      setSaving(true);
      setError(null);

      const currentRolesRes = await getUserRoles(userId);
      const currentCodes = new Set(
        (currentRolesRes.data || []).map((r) => r.roleCode)
      );

      const toAdd = [...assignedRoleCodes].filter(
        (code) => !currentCodes.has(code)
      );
      const toRemove = [...currentCodes].filter(
        (code) => !assignedRoleCodes.has(code)
      );

      await Promise.all([
        ...toAdd.map((roleCode) =>
          assignUserRole({ userId, roleCode })
        ),
        ...toRemove.map((roleCode) =>
          removeUserRole({ userId, roleCode })
        ),
      ]);

      setSuccess(true);
      onUpdated?.();
    } catch (err) {
      console.error("Role update failed:", err);
      setError("Failed to update roles");
    } finally {
      setSaving(false);
    }
  };

  /* =======================
     UI
     ======================= */
  if (loading) {
    return <div className="text-sm text-slate-500">Loading roles…</div>;
  }

  return (
    <div className="space-y-4">
      {error && (
        <div className="text-sm text-red-600 bg-red-50 border border-red-200 rounded px-3 py-2">
          {error}
        </div>
      )}

      {success && (
        <div className="text-sm text-green-700 bg-green-50 border border-green-200 rounded px-3 py-2">
          Roles updated successfully
        </div>
      )}

      <div className="space-y-2">
        {allRoles.map((role) => (
          <label
            key={role.roleCode}
            className="flex items-center gap-2 text-sm"
          >
            <input
              type="checkbox"
              checked={assignedRoleCodes.has(role.roleCode)}
              onChange={() => toggleRole(role.roleCode)}
            />
            <span className="text-slate-700">{role.roleName}</span>
            <span className="text-xs text-slate-400">
              ({role.roleCode})
            </span>
          </label>
        ))}
      </div>

      <div className="pt-2">
        <button
          onClick={handleSave}
          disabled={saving}
          className="px-4 py-2 bg-blue-600 text-white text-sm rounded disabled:opacity-50"
        >
          {saving ? "Saving…" : "Save Roles"}
        </button>
      </div>
    </div>
  );
}
