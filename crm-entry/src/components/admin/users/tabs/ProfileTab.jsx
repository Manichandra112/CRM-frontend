import { useEffect, useMemo, useState } from "react";
import {
  updateUser,
  assignManager,
  getManagers,
} from "../../../../api/users/users.api";

export default function ProfileTab({ user, onUpdated }) {
  /* =======================
     PROFILE FORM STATE
     ======================= */
  const [form, setForm] = useState({
    firstName: "",
    lastName: "",
    mobileNumber: "",
    department: "",
    designation: "",
    employmentType: "",
    workShift: "",
    assignedRegion: "",
    assignedBranch: "",
    remarks: "",
  });

  /* =======================
     MANAGER STATE
     ======================= */
  const [managers, setManagers] = useState([]);
  const [managerSearch, setManagerSearch] = useState("");
  const [managerDomain, setManagerDomain] = useState("");
  const [selectedManager, setSelectedManager] = useState(null);

  const currentManagerName =
    user?.organization?.managerName || "";

  const [saving, setSaving] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(false);

  /* =======================
     PREFILL EXISTING DATA
     ======================= */
  useEffect(() => {
    if (!user) return;

    setForm({
      firstName: user.profile?.firstName ?? "",
      lastName: user.profile?.lastName ?? "",
      mobileNumber: user.profile?.mobileNumber ?? "",
      department: user.organization?.department ?? "",
      designation: user.organization?.designation ?? "",
      employmentType: user.organization?.employmentType ?? "",
      workShift: user.organization?.workShift ?? "",
      assignedRegion: user.organization?.assignedRegion ?? "",
      assignedBranch: user.organization?.assignedBranch ?? "",
      remarks: user.organization?.remarks ?? "",
    });
  }, [user]);

  /* =======================
     LOAD MANAGERS (ONCE)
     ======================= */
useEffect(() => {
  const loadManagers = async () => {
    try {
      const data = await getManagers();
      setManagers(Array.isArray(data) ? data : []);
    } catch (err) {
      console.error("Failed to load managers", err);
    }
  };

  loadManagers();
}, []);


  /* =======================
     FILTER MANAGERS (ZOHO STYLE)
     ======================= */
  const filteredManagers = useMemo(() => {
    return managers.filter((m) => {
      const matchesDomain =
        !managerDomain || m.domainCode === managerDomain;

      const matchesSearch =
        !managerSearch ||
        m.name.toLowerCase().includes(managerSearch.toLowerCase());

      return matchesDomain && matchesSearch;
    });
  }, [managers, managerDomain, managerSearch]);

  /* =======================
     HANDLERS
     ======================= */
  const handleChange = (e) => {
    const { name, value } = e.target;
    setForm((prev) => ({ ...prev, [name]: value }));
    setSuccess(false);
  };

  const handleSave = async () => {
    try {
      setSaving(true);
      setError(null);

      // 1️⃣ Update profile / org details
      await updateUser(user.userId, {
        firstName: form.firstName,
        lastName: form.lastName,
        mobileNumber: form.mobileNumber,
        department: form.department,
        designation: form.designation,
        employmentType: form.employmentType,
        workShift: form.workShift,
        assignedRegion: form.assignedRegion,
        assignedBranch: form.assignedBranch,
        remarks: form.remarks,
      });

      // 2️⃣ Update manager ONLY if changed
      if (
        selectedManager &&
        selectedManager.name !== currentManagerName
      ) {
        await assignManager(user.userId, selectedManager.userId);
      }

      setSuccess(true);
      onUpdated?.();
    } catch (err) {
      console.error("Profile update failed", err);
      setError("Failed to save user details");
    } finally {
      setSaving(false);
    }
  };

  /* =======================
     UI
     ======================= */
  return (
    <div className="space-y-6 max-w-3xl">
      {error && (
        <div className="text-sm text-red-600 bg-red-50 border border-red-200 rounded px-3 py-2">
          {error}
        </div>
      )}

      {success && (
        <div className="text-sm text-green-700 bg-green-50 border border-green-200 rounded px-3 py-2">
          Changes saved successfully
        </div>
      )}

      {/* BASIC INFO */}
      <div className="grid grid-cols-2 gap-4">
        <input
          name="firstName"
          value={form.firstName}
          onChange={handleChange}
          placeholder="First Name"
          className="border rounded px-3 py-2 text-sm"
        />

        <input
          name="lastName"
          value={form.lastName}
          onChange={handleChange}
          placeholder="Last Name"
          className="border rounded px-3 py-2 text-sm"
        />

        <input
          name="mobileNumber"
          value={form.mobileNumber}
          onChange={handleChange}
          placeholder="Mobile Number"
          className="border rounded px-3 py-2 text-sm"
        />

        <input
          name="department"
          value={form.department}
          onChange={handleChange}
          placeholder="Department"
          className="border rounded px-3 py-2 text-sm"
        />

        <input
          name="designation"
          value={form.designation}
          onChange={handleChange}
          placeholder="Designation"
          className="border rounded px-3 py-2 text-sm"
        />

        <input
          name="employmentType"
          value={form.employmentType}
          onChange={handleChange}
          placeholder="Employment Type"
          className="border rounded px-3 py-2 text-sm"
        />

        <input
          name="workShift"
          value={form.workShift}
          onChange={handleChange}
          placeholder="Work Shift"
          className="border rounded px-3 py-2 text-sm"
        />

        <input
          name="assignedRegion"
          value={form.assignedRegion}
          onChange={handleChange}
          placeholder="Assigned Region"
          className="border rounded px-3 py-2 text-sm"
        />

        <input
          name="assignedBranch"
          value={form.assignedBranch}
          onChange={handleChange}
          placeholder="Assigned Branch"
          className="border rounded px-3 py-2 text-sm"
        />
      </div>

      <textarea
        name="remarks"
        value={form.remarks}
        onChange={handleChange}
        placeholder="Remarks"
        className="border rounded px-3 py-2 text-sm w-full"
        rows={3}
      />

      {/* CURRENT MANAGER */}
      <div className="border rounded px-3 py-2 bg-slate-50 text-sm">
        <div className="text-xs text-slate-500">Current Manager</div>
        <div className="font-medium text-slate-800">
          {currentManagerName || "Not assigned"}
        </div>
      </div>

      {/* CHANGE MANAGER */}
      <div className="space-y-2">
        <div className="flex gap-2">
          <select
            value={managerDomain}
            onChange={(e) => setManagerDomain(e.target.value)}
            className="border rounded px-2 py-1 text-sm"
          >
            <option value="">All Domains</option>
            <option value="HR">HR</option>
            <option value="SALES">Sales</option>
            <option value="SOCIALMEDIA">Social</option>
          </select>

          <input
            type="text"
            placeholder="Search manager"
            value={managerSearch}
            onChange={(e) => setManagerSearch(e.target.value)}
            className="border rounded px-3 py-1 text-sm flex-1"
          />
        </div>

        <div className="border rounded max-h-40 overflow-y-auto">
          {filteredManagers.map((m) => (
            <div
              key={m.userId}
              onClick={() => setSelectedManager(m)}
              className={`px-3 py-2 text-sm cursor-pointer hover:bg-slate-50 ${
                selectedManager?.userId === m.userId
                  ? "bg-blue-50"
                  : ""
              }`}
            >
              <div className="font-medium">{m.name}</div>
              <div className="text-xs text-slate-500">
                {m.domainCode}
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* SAVE */}
      <button
        onClick={handleSave}
        disabled={saving}
        className="px-5 py-2 bg-blue-600 text-white text-sm rounded disabled:opacity-50"
      >
        {saving ? "Saving…" : "Save Changes"}
      </button>
    </div>
  );
}
