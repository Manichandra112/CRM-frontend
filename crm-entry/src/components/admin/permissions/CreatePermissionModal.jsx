import { useState } from "react";
import { createPermission } from "../../../api/admin/permissions.api";

export default function CreatePermissionModal({
  onClose,
  onCreated,
}) {
  const [permissionCode, setPermissionCode] = useState("");
  const [description, setDescription] = useState("");
  const [module, setModule] = useState("");

  const [saving, setSaving] = useState(false);
  const [error, setError] = useState(null);

  const handleCreate = async () => {
    try {
      setSaving(true);
      setError(null);

      if (!permissionCode || !module) {
        setError("Permission code and module are required");
        return;
      }

      await createPermission({
        permissionCode: permissionCode.trim(),
        description: description.trim(),
        module: module.trim(),
      });

      onCreated();
      onClose();
    } catch (e) {
      console.error(e);
      setError("Failed to create permission");
    } finally {
      setSaving(false);
    }
  };

  return (
    /* OVERLAY */
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      {/* BACKDROP */}
      <div
        className="absolute inset-0 bg-black/40"
        onClick={onClose}
      />

      {/* MODAL */}
      <div className="relative bg-white rounded-md shadow-xl w-full max-w-md mx-4">
        {/* HEADER */}
        <div className="flex items-center justify-between px-4 py-3 border-b">
          <h3 className="text-sm font-semibold text-slate-800">
            Create Permission
          </h3>
          <button
            onClick={onClose}
            className="text-slate-400 hover:text-slate-600"
          >
            ✕
          </button>
        </div>

        {/* BODY */}
        <div className="px-4 py-4 space-y-3">
          {error && (
            <div className="text-sm text-red-600">
              {error}
            </div>
          )}

          <input
            placeholder="Permission Code (IMMUTABLE)"
            value={permissionCode}
            onChange={(e) =>
              setPermissionCode(e.target.value)
            }
            className="w-full border rounded px-3 py-2 text-sm font-mono"
          />

          <input
            placeholder="Module (HR, USER, AUTH, etc.)"
            value={module}
            onChange={(e) => setModule(e.target.value)}
            className="w-full border rounded px-3 py-2 text-sm"
          />

          <textarea
            placeholder="Description (optional)"
            value={description}
            onChange={(e) =>
              setDescription(e.target.value)
            }
            className="w-full border rounded px-3 py-2 text-sm"
            rows={3}
          />
        </div>

        {/* FOOTER */}
        <div className="flex justify-end gap-2 px-4 py-3 border-t">
          <button
            onClick={onClose}
            className="px-3 py-2 border rounded text-sm"
          >
            Cancel
          </button>
          <button
            onClick={handleCreate}
            disabled={saving}
            className="px-4 py-2 bg-blue-600 text-white rounded text-sm disabled:opacity-50"
          >
            {saving ? "Creating…" : "Create"}
          </button>
        </div>
      </div>
    </div>
  );
}
