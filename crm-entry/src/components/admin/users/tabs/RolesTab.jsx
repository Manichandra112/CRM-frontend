import { useEffect, useState, useMemo } from "react";
import { getAdminRoles } from "../../../../api/admin/roles.api";
import {
  getUserRoles,
  assignUserRole,
  removeUserRole,
} from "../../../../api/admin/roles.api";

export default function RolesTab({ userId, onUpdated }) {
  const [allRoles, setAllRoles] = useState([]);
  const [initialRoleCodes, setInitialRoleCodes] = useState(new Set());
  const [selectedRoleCodes, setSelectedRoleCodes] = useState(new Set());

  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(false);

  useEffect(() => {
    if (!userId) return;

    const loadRoles = async () => {
      try {
        setLoading(true);
        setError(null);

        const [rolesData, userRolesData] = await Promise.all([
          getAdminRoles(),
          getUserRoles(userId),
        ]);

        const safeRoles = Array.isArray(rolesData) ? rolesData : [];
        const safeUserRoles = Array.isArray(userRolesData)
          ? userRolesData
          : [];

        setAllRoles(safeRoles);

        const codes = new Set(
          safeUserRoles.map((r) => r.roleCode)
        );

        setInitialRoleCodes(codes);
        setSelectedRoleCodes(new Set(codes));
      } catch (err) {
        console.error("Role load error:", err);
        setError("Failed to load roles");
      } finally {
        setLoading(false);
      }
    };

    loadRoles();
  }, [userId]);

  const toggleRole = (roleCode) => {
    setSelectedRoleCodes((prev) => {
      const next = new Set(prev);
      next.has(roleCode)
        ? next.delete(roleCode)
        : next.add(roleCode);
      return next;
    });

    setSuccess(false);
  };

  const hasChanges = useMemo(() => {
    if (initialRoleCodes.size !== selectedRoleCodes.size)
      return true;

    for (const code of initialRoleCodes) {
      if (!selectedRoleCodes.has(code)) return true;
    }

    return false;
  }, [initialRoleCodes, selectedRoleCodes]);

  const handleSave = async () => {
    try {
      setSaving(true);
      setError(null);

      const toAdd = [...selectedRoleCodes].filter(
        (c) => !initialRoleCodes.has(c)
      );

      const toRemove = [...initialRoleCodes].filter(
        (c) => !selectedRoleCodes.has(c)
      );

      await Promise.all([
        ...toAdd.map((roleCode) =>
          assignUserRole({ userId, roleCode })
        ),
        ...toRemove.map((roleCode) =>
          removeUserRole({ userId, roleCode })
        ),
      ]);

      setInitialRoleCodes(new Set(selectedRoleCodes));
      setSuccess(true);
      onUpdated?.();
    } catch (err) {
      console.error("Role update failed:", err);
      setError("Failed to update roles");
    } finally {
      setSaving(false);
    }
  };

  if (loading)
    return (
      <div className="text-sm text-slate-500">
        Loading roles…
      </div>
    );

  return (
    <div className="space-y-4">

      {error && (
        <div className="text-sm text-red-600 bg-red-50 border rounded px-3 py-2">
          {error}
        </div>
      )}

      {success && (
        <div className="text-sm text-green-700 bg-green-50 border rounded px-3 py-2">
          Roles updated successfully
        </div>
      )}

      {allRoles.length === 0 && (
        <div className="text-sm text-slate-500">
          No roles available
        </div>
      )}

      <div className="space-y-2">
        {allRoles.map((role) => (
          <label
            key={role.roleCode}
            className="flex items-center gap-2 text-sm cursor-pointer"
          >
            <input
              type="checkbox"
              checked={selectedRoleCodes.has(role.roleCode)}
              onChange={() => toggleRole(role.roleCode)}
            />
            <span>{role.roleName}</span>
            <span className="text-xs text-slate-400">
              ({role.roleCode})
            </span>
          </label>
        ))}
      </div>

      <div className="pt-2">
        <button
          onClick={handleSave}
          disabled={!hasChanges || saving}
          className="px-4 py-2 bg-blue-600 text-white text-sm rounded disabled:opacity-50"
        >
          {saving ? "Saving…" : "Save Roles"}
        </button>
      </div>
    </div>
  );
}
