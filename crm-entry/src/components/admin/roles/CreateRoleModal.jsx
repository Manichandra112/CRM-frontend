import { useEffect, useState } from "react";
import { createRole } from "../../../api/admin/roles.api";
import {
  getAllPermissions,
  assignPermissionToRole,
} from "../../../api/admin/rolePermissions.api";

export default function CreateRoleModal({ onClose, onCreated }) {
  const [roleName, setRoleName] = useState("");
  const [roleCode, setRoleCode] = useState("");
  const [description, setDescription] = useState("");

  const [permissions, setPermissions] = useState([]);
  const [selectedPerms, setSelectedPerms] = useState(new Set());

  const [saving, setSaving] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
  const loadPermissions = async () => {
    const data = await getAllPermissions();
    setPermissions(Array.isArray(data) ? data : []);
  };

  loadPermissions();
}, []);


  const togglePerm = (code) => {
    setSelectedPerms((prev) => {
      const next = new Set(prev);
      if (next.has(code)) next.delete(code);
      else next.add(code);
      return next;
    });
  };

  const handleCreate = async () => {
    try {
      setSaving(true);
      setError(null);

      if (!roleName || !roleCode) {
        setError("Role name and code are required");
        return;
      }

      await createRole({
        roleName,
        roleCode,
        description,
      });

      await Promise.all(
        [...selectedPerms].map((permissionCode) =>
          assignPermissionToRole({
            roleCode,
            permissionCode,
          })
        )
      );

      onCreated();
      onClose();
    } catch (e) {
      console.error(e);
      setError("Failed to create role");
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black/30 flex items-center justify-center z-50">
      <div className="bg-white w-[520px] rounded-md shadow-lg p-5 space-y-4">
        <h3 className="text-lg font-medium">
          Create New Role
        </h3>

        {error && (
          <div className="text-sm text-red-600 bg-red-50 border rounded px-3 py-2">
            {error}
          </div>
        )}

        <input
          placeholder="Role Name"
          value={roleName}
          onChange={(e) => setRoleName(e.target.value)}
          className="w-full border rounded px-3 py-2 text-sm"
        />

        <input
          placeholder="Role Code (e.g. FINANCE_MANAGER)"
          value={roleCode}
          onChange={(e) => setRoleCode(e.target.value)}
          className="w-full border rounded px-3 py-2 text-sm font-mono"
        />

        <textarea
          placeholder="Description (optional)"
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          className="w-full border rounded px-3 py-2 text-sm"
        />

        <div className="border rounded p-3 max-h-48 overflow-auto">
          {permissions.map((p) => (
            <label
              key={p.permissionCode}
              className="flex items-center gap-2 text-sm"
            >
              <input
                type="checkbox"
                onChange={() => togglePerm(p.permissionCode)}
              />
              <span>{p.permissionCode}</span>
              <span className="text-xs text-slate-400">
                {p.description}
              </span>
            </label>
          ))}
        </div>

        <div className="flex justify-end gap-2">
          <button
            onClick={onClose}
            className="px-3 py-2 text-sm border rounded"
          >
            Cancel
          </button>
          <button
            onClick={handleCreate}
            disabled={saving}
            className="px-4 py-2 text-sm bg-blue-600 text-white rounded"
          >
            {saving ? "Creating…" : "Create Role"}
          </button>
        </div>
      </div>
    </div>
  );
}
