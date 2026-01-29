import { useState } from "react";
import { updatePermission } from "../../../api/admin/permissions.api";

export default function EditPermissionModal({
  permission,
  onClose,
  onUpdated,
}) {
  const [active, setActive] = useState(permission.active);
  const [saving, setSaving] = useState(false);

  const handleSave = async () => {
    try {
      setSaving(true);
      await updatePermission(permission.permissionId, {
        isActive: active,
      });
      onUpdated();
      onClose();
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
            Edit Permission
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
          <div className="text-sm">
            <strong>Code:</strong> {permission.permissionCode}
          </div>

          <div className="text-sm">
            <strong>Module:</strong> {permission.module}
          </div>

          <label className="flex items-center gap-2">
            <input
              type="checkbox"
              checked={active}
              onChange={(e) => setActive(e.target.checked)}
            />
            Active
          </label>
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
            onClick={handleSave}
            disabled={saving}
            className="px-4 py-2 bg-blue-600 text-white rounded text-sm disabled:opacity-50"
          >
            {saving ? "Saving…" : "Save"}
          </button>
        </div>
      </div>
    </div>
  );
}
