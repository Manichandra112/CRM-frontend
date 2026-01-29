import { useEffect, useMemo, useState } from "react";

import {
  getAllPermissions,
  getRolePermissionsByRoleCode,
  assignPermissionToRole,
  removePermissionFromRole,
} from "../../../api/admin/rolePermissions.api";

export default function RolePermissionsEditor({ role }) {
  const [allPermissions, setAllPermissions] = useState([]);
  const [initialAssigned, setInitialAssigned] = useState(new Set());
  const [selectedPermissions, setSelectedPermissions] = useState(new Set());

  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(false);

  /* =======================
     LOAD PERMISSIONS
     ======================= */
  useEffect(() => {
    if (!role?.roleCode) return;

    const load = async () => {
      try {
        setLoading(true);
        setError(null);
        setSuccess(false);

        const [allRes, assignedRes] = await Promise.all([
          getAllPermissions(),
          getRolePermissionsByRoleCode(role.roleCode),
        ]);

        const all = allRes.data || [];
        const assignedCodes = new Set(
          assignedRes.data?.permissions || []
        );

        setAllPermissions(all);
        setInitialAssigned(assignedCodes);
        setSelectedPermissions(new Set(assignedCodes));
      } catch (e) {
        console.error("Permission load failed", e);
        setError("Failed to load permissions");
      } finally {
        setLoading(false);
      }
    };

    load();
  }, [role.roleCode]);

  /* =======================
     GROUP BY MODULE
     ======================= */
  const permissionsByModule = useMemo(() => {
    const grouped = {};
    allPermissions.forEach((perm) => {
      const module = perm.module || "OTHER";
      if (!grouped[module]) grouped[module] = [];
      grouped[module].push(perm);
    });
    return grouped;
  }, [allPermissions]);

  /* =======================
     TOGGLE PERMISSION
     ======================= */
  const togglePermission = (code) => {
    setSelectedPermissions((prev) => {
      const next = new Set(prev);
      if (next.has(code)) next.delete(code);
      else next.add(code);
      return next;
    });
    setSuccess(false);
  };

  /* =======================
     CHANGE DETECTION
     ======================= */
  const hasChanges = useMemo(() => {
    if (initialAssigned.size !== selectedPermissions.size)
      return true;

    for (const code of initialAssigned) {
      if (!selectedPermissions.has(code)) return true;
    }
    return false;
  }, [initialAssigned, selectedPermissions]);

  /* =======================
     SAVE
     ======================= */
  const handleSave = async () => {
    try {
      setSaving(true);
      setError(null);

      const toAdd = [...selectedPermissions].filter(
        (c) => !initialAssigned.has(c)
      );

      const toRemove = [...initialAssigned].filter(
        (c) => !selectedPermissions.has(c)
      );

      await Promise.all([
        ...toAdd.map((permissionCode) =>
          assignPermissionToRole({
            roleCode: role.roleCode,
            permissionCode,
          })
        ),
        ...toRemove.map((permissionCode) =>
          removePermissionFromRole({
            roleCode: role.roleCode,
            permissionCode,
          })
        ),
      ]);

      setInitialAssigned(new Set(selectedPermissions));
      setSuccess(true);
    } catch (e) {
      console.error("Permission update failed", e);
      setError("Failed to update permissions");
    } finally {
      setSaving(false);
    }
  };

  /* =======================
     UI STATES
     ======================= */
  if (loading) {
    return (
      <div className="text-sm text-slate-500">
        Loading permissions…
      </div>
    );
  }

  return (
    <div className="space-y-4">
      {/* HEADER */}
      <div>
        <h3 className="text-lg font-medium text-slate-800">
          {role.roleName}
        </h3>
        <p className="text-sm text-slate-500">
          Manage permissions for this role
        </p>
      </div>

      {/* ERROR */}
      {error && (
        <div className="text-sm text-red-600 bg-red-50 border border-red-200 rounded px-3 py-2">
          {error}
        </div>
      )}

      {/* SUCCESS */}
      {success && (
        <div className="text-sm text-green-700 bg-green-50 border border-green-200 rounded px-3 py-2">
          Permissions updated successfully
        </div>
      )}

      {/* PERMISSIONS (GROUPED) */}
      <div className="border rounded-md max-h-[420px] overflow-auto divide-y">
        {Object.entries(permissionsByModule).map(
          ([module, perms]) => {
            const allChecked = perms.every((p) =>
              selectedPermissions.has(p.permissionCode)
            );

            return (
              <div key={module}>
                {/* MODULE HEADER */}
                <div className="flex items-center justify-between bg-slate-50 px-4 py-2 border-b">
                  <div className="font-medium text-slate-700">
                    {module}
                    <span className="ml-2 text-xs text-slate-400">
                      ({perms.length})
                    </span>
                  </div>

                  <button
                    type="button"
                    onClick={() => {
                      setSelectedPermissions((prev) => {
                        const next = new Set(prev);
                        if (allChecked) {
                          perms.forEach((p) =>
                            next.delete(p.permissionCode)
                          );
                        } else {
                          perms.forEach((p) =>
                            next.add(p.permissionCode)
                          );
                        }
                        return next;
                      });
                    }}
                    className="text-xs text-blue-600 hover:underline"
                  >
                    {allChecked ? "Clear all" : "Select all"}
                  </button>
                </div>

                {/* MODULE PERMISSIONS */}
                <div className="divide-y">
                  {perms.map((perm) => {
                    const checked =
                      selectedPermissions.has(
                        perm.permissionCode
                      );

                    return (
                      <label
                        key={perm.permissionCode}
                        className="flex items-start gap-3 px-4 py-3 text-sm cursor-pointer hover:bg-slate-50"
                      >
                        <input
                          type="checkbox"
                          checked={checked}
                          onChange={() =>
                            togglePermission(
                              perm.permissionCode
                            )
                          }
                          className="mt-1"
                        />
                        <div>
                          <div className="font-medium text-slate-800">
                            {perm.permissionCode}
                          </div>
                          {perm.description && (
                            <div className="text-xs text-slate-500">
                              {perm.description}
                            </div>
                          )}
                        </div>
                      </label>
                    );
                  })}
                </div>
              </div>
            );
          }
        )}
      </div>

      {/* ACTIONS */}
      <div className="flex justify-end">
        <button
          onClick={handleSave}
          disabled={!hasChanges || saving}
          className="px-4 py-2 bg-blue-600 text-white text-sm rounded disabled:opacity-50"
        >
          {saving ? "Saving…" : "Save Permissions"}
        </button>
      </div>
    </div>
  );
}
