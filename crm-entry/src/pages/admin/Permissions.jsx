import { useEffect, useState } from "react";
import {
  getPermissions,
  createPermission,
  updatePermission,
} from "../../api/admin/permissions.api";

import CreatePermissionModal from "../../components/admin/permissions/CreatePermissionModal";
import EditPermissionModal from "../../components/admin/permissions/EditPermissionModal";


export default function Permissions() {
  const [permissions, setPermissions] = useState([]);
  const [loading, setLoading] = useState(true);

  const [showCreate, setShowCreate] = useState(false);
  const [editing, setEditing] = useState(null);

  /* =======================
     LOAD
     ======================= */
  const loadPermissions = async () => {
    try {
      setLoading(true);
      const res = await getPermissions();
      setPermissions(res.data || []);
    } catch (e) {
      console.error("Failed to load permissions", e);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadPermissions();
  }, []);

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
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-xl font-semibold text-slate-800">
            Permissions
          </h2>
          <p className="text-sm text-slate-500">
            Manage system permissions
          </p>
        </div>

        <button
          onClick={() => setShowCreate(true)}
          className="px-3 py-2 bg-blue-600 text-white text-sm rounded"
        >
          + Add Permission
        </button>
      </div>

      {/* TABLE */}
      <div className="bg-white border rounded-md overflow-hidden">
        <table className="w-full text-sm">
          <thead className="bg-slate-50 text-slate-600">
            <tr>
              <th className="px-4 py-3 text-left">
                Permission Code
              </th>
              <th className="px-4 py-3 text-left">
                Module
              </th>
              <th className="px-4 py-3 text-left">
                Status
              </th>
              <th className="px-4 py-3 text-right">
                Action
              </th>
            </tr>
          </thead>

          <tbody className="divide-y">
            {permissions.map((p) => (
              <tr key={p.permissionId}>
                <td className="px-4 py-3 font-mono">
                  {p.permissionCode}
                </td>
                <td className="px-4 py-3">
                  {p.module}
                </td>
                <td className="px-4 py-3">
                  <span
                    className={`px-2 py-1 text-xs rounded-full ${
                      p.active
                        ? "bg-green-100 text-green-700"
                        : "bg-red-100 text-red-700"
                    }`}
                  >
                    {p.active ? "Active" : "Inactive"}
                  </span>
                </td>
                <td className="px-4 py-3 text-right">
                  <button
                    onClick={() => setEditing(p)}
                    className="text-blue-600 hover:underline text-sm"
                  >
                    Edit
                  </button>
                </td>
              </tr>
            ))}

            {permissions.length === 0 && (
              <tr>
                <td
                  colSpan={4}
                  className="px-4 py-6 text-center text-slate-500"
                >
                  No permissions found
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      {/* CREATE */}
      {showCreate && (
        <CreatePermissionModal
          onClose={() => setShowCreate(false)}
          onCreated={loadPermissions}
        />
      )}

      {/* EDIT */}
      {editing && (
        <EditPermissionModal
          permission={editing}
          onClose={() => setEditing(null)}
          onUpdated={loadPermissions}
        />
      )}
    </div>
  );
}
