import { useEffect, useState } from "react";
import { getUserSecurity } from "../../../../api/admin/users.api";

export default function UserSecurityTab({ userId }) {
  const [security, setSecurity] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  /* =======================
     LOAD SECURITY INFO
     ======================= */
  useEffect(() => {
    if (!userId) return;

    const loadSecurity = async () => {
      try {
        setLoading(true);
        setError(null);

        const data = await getUserSecurity(userId);
setSecurity(data);

      } catch (err) {
        console.error("Security fetch error:", err);
        setError("Failed to load security details");
      } finally {
        setLoading(false);
      }
    };

    loadSecurity();
  }, [userId]);

  /* =======================
     UI STATES
     ======================= */
  if (loading) {
    return (
      <div className="text-sm text-slate-500">
        Loading security details…
      </div>
    );
  }

  if (error) {
    return (
      <div className="text-sm text-red-600 bg-red-50 border border-red-200 rounded-md px-3 py-2">
        {error}
      </div>
    );
  }

  if (!security) {
    return (
      <div className="text-sm text-slate-500">
        No security data available
      </div>
    );
  }

  /* =======================
     DERIVED VALUES
     ======================= */
  const isLocked = Boolean(security.lockedUntil);

  /* =======================
     MAIN UI
     ======================= */
  return (
    <div className="space-y-6 text-sm">

      {/* =======================
         ACTIVITY SUMMARY
         ======================= */}
      <div className="space-y-1">
        <div className="text-slate-500">Last Login</div>
        <div className="text-lg font-medium text-slate-900">
          {security.lastLoginAt
            ? new Date(security.lastLoginAt).toLocaleString()
            : "Never"}
        </div>

        <div
          className={`inline-flex items-center text-sm font-medium ${
            isLocked ? "text-red-600" : "text-green-600"
          }`}
        >
          {isLocked ? "Account Locked" : "Account Active"}
        </div>
      </div>

      {/* =======================
         LOGIN DETAILS
         ======================= */}
      <div className="bg-slate-50 border border-slate-200 rounded-md p-4 space-y-3">
        <div className="text-xs font-semibold uppercase tracking-wide text-slate-600">
          Login Details
        </div>

        <div className="flex justify-between gap-4">
          <span className="text-slate-500">Device</span>
          <span className="text-slate-800 text-right break-all max-w-[70%]">
            {security.lastLoginDevice || "Unknown"}
          </span>
        </div>
        <div className="flex justify-between">
  <span className="text-slate-500">IP Address</span>
  <span className="text-slate-800 font-mono">
    {security.lastLoginIp || "Unknown"}
  </span>
</div>

        <div className="flex justify-between">
          <span className="text-slate-500">Location</span>
          <span className="text-slate-800">
            {security.lastLoginLocation || "Unknown"}
          </span>
        </div>

        <div className="flex justify-between">
          <span className="text-slate-500">Failed Login Attempts</span>
          <span className="text-slate-800">
            {security.failedLoginCount}
          </span>
        </div>
      </div>

      {/* =======================
         SECURITY SETTINGS
         ======================= */}
      <div className="bg-slate-50 border border-slate-200 rounded-md p-4 space-y-3">
        <div className="text-xs font-semibold uppercase tracking-wide text-slate-600">
          Security Settings
        </div>

        <div className="flex justify-between">
          <span className="text-slate-500">Password Reset Required</span>
          <span className="text-slate-800">
            {security.forcePasswordReset ? "Yes" : "No"}
          </span>
        </div>

        <div className="flex justify-between">
          <span className="text-slate-500">MFA Enabled</span>
          <span className="text-slate-800">
            {security.mfaEnabled ? "Yes" : "No"}
          </span>
        </div>
      </div>
    </div>
  );
}
