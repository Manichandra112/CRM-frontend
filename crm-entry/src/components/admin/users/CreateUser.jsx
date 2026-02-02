import { useEffect, useState } from "react";
import { getDomains } from "../../../api/admin/domains.api";
import { getAdminRoles } from "../../../api/admin/roles.api";
import { createUser } from "../../../api/users/users.api";

export default function CreateUser({ onSuccess, onClose }) {
  const [domains, setDomains] = useState([]);
  const [roles, setRoles] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const [form, setForm] = useState({
    username: "",
    email: "",
    domainCode: "",
    temporaryPassword: "",
    roleCodes: [],
    profile: {
      firstName: "",
      lastName: "",
      mobileNumber: "",
      department: "",
      designation: "",
    },
  });

  /* =======================
     LOAD DOMAINS & ROLES
     ======================= */
  useEffect(() => {
    Promise.all([getDomains(), getAdminRoles()])
      .then(([dRes, rRes]) => {
        setDomains(dRes.data || []);
        setRoles(rRes.data || []);
      })
      .catch(() => {
        setError("Failed to load domains or roles");
      });
  }, []);

  /* =======================
     DOMAIN CHANGE
     ======================= */
  const handleDomainChange = (value) => {
    setForm((prev) => ({
      ...prev,
      domainCode: value,
      profile: {
        ...prev.profile,
        department: value,
      },
    }));
  };

  /* =======================
     ROLE TOGGLE
     ======================= */
  const toggleRole = (roleCode) => {
    setForm((prev) => ({
      ...prev,
      roleCodes: prev.roleCodes.includes(roleCode)
        ? prev.roleCodes.filter((r) => r !== roleCode)
        : [...prev.roleCodes, roleCode],
    }));
  };

  /* =======================
     SUBMIT
     ======================= */
  const handleSubmit = async () => {
    setError(null);

    if (
      !form.username ||
      !form.email ||
      !form.domainCode ||
      !form.roleCodes.length
    ) {
      setError("Please fill all required fields");
      return;
    }

    try {
      setLoading(true);
      await createUser(form);
      onSuccess?.();
    } catch {
      setError("Failed to create user");
    } finally {
      setLoading(false);
    }
  };

  /* =======================
     UI
     ======================= */
  return (
    <div className="space-y-5">
      {/* HEADER */}
      <div className="flex justify-between items-center border-b pb-3">
        <h3 className="text-lg font-semibold text-slate-800">
          Create User
        </h3>
        <button
          onClick={onClose}
          className="text-slate-500 hover:text-slate-800 text-xl leading-none"
          title="Close"
        >
          ×
        </button>
      </div>

      {error && (
        <div className="bg-red-50 text-red-700 text-sm px-3 py-2 rounded">
          {error}
        </div>
      )}

      {/* FORM */}
      <div className="grid grid-cols-2 gap-4">
        <input
          placeholder="Username *"
          value={form.username}
          onChange={(e) => setForm({ ...form, username: e.target.value })}
          className="border px-3 py-2 rounded text-sm"
        />

        <input
          placeholder="Email *"
          value={form.email}
          onChange={(e) => setForm({ ...form, email: e.target.value })}
          className="border px-3 py-2 rounded text-sm"
        />

        <input
          placeholder="First Name"
          value={form.profile.firstName}
          onChange={(e) =>
            setForm({
              ...form,
              profile: { ...form.profile, firstName: e.target.value },
            })
          }
          className="border px-3 py-2 rounded text-sm"
        />

        <input
          placeholder="Last Name"
          value={form.profile.lastName}
          onChange={(e) =>
            setForm({
              ...form,
              profile: { ...form.profile, lastName: e.target.value },
            })
          }
          className="border px-3 py-2 rounded text-sm"
        />

        <input
          placeholder="Mobile Number"
          value={form.profile.mobileNumber}
          onChange={(e) =>
            setForm({
              ...form,
              profile: { ...form.profile, mobileNumber: e.target.value },
            })
          }
          className="border px-3 py-2 rounded text-sm"
        />

        <input
          placeholder="Designation"
          value={form.profile.designation}
          onChange={(e) =>
            setForm({
              ...form,
              profile: { ...form.profile, designation: e.target.value },
            })
          }
          className="border px-3 py-2 rounded text-sm"
        />

        {/* DOMAIN */}
        <select
          value={form.domainCode}
          onChange={(e) => handleDomainChange(e.target.value)}
          className="border px-3 py-2 rounded text-sm col-span-2"
        >
          <option value="">Select Domain *</option>
          {domains.map((d) => (
            <option key={d.domainId} value={d.domainCode}>
              {d.domainName}
            </option>
          ))}
        </select>

        {/* ROLES */}
        <div className="col-span-2">
          <p className="text-sm font-medium text-slate-700 mb-2">
            Assign Roles *
          </p>

          <div className="border rounded p-3 max-h-40 overflow-y-auto space-y-2">
            {roles.map((r) => (
              <label
                key={r.roleCode}
                className="flex items-center gap-2 text-sm cursor-pointer"
              >
                <input
                  type="checkbox"
                  checked={form.roleCodes.includes(r.roleCode)}
                  onChange={() => toggleRole(r.roleCode)}
                />
                {r.roleName}
              </label>
            ))}
          </div>
        </div>

        <input
          placeholder="Temporary Password *"
          value={form.temporaryPassword}
          onChange={(e) =>
            setForm({ ...form, temporaryPassword: e.target.value })
          }
          className="border px-3 py-2 rounded text-sm col-span-2"
        />
      </div>

      {/* ACTIONS */}
      <div className="flex justify-end gap-3 pt-2">
        <button
          onClick={onClose}
          className="px-4 py-2 text-sm border rounded"
        >
          Cancel
        </button>

        <button
          disabled={loading}
          onClick={handleSubmit}
          className="bg-blue-600 text-white px-4 py-2 rounded text-sm disabled:opacity-50"
        >
          Create User
        </button>
      </div>
    </div>
  );
}
