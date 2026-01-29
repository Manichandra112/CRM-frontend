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

        const res = await getUserSecurity(userId);
        setSecurity(res.data);
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
     UI
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
      <div className="text-sm text-red-600 bg-red-50 border border-red-200 rounded px-3 py-2">
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

  return (
    <div className="space-y-3 text-sm">
      <div className="flex justify-between">
        <span className="text-slate-500">Last Login</span>
        <span className="text-slate-800">
          {security.lastLogin
            ? new Date(security.lastLogin).toLocaleString()
            : "Never"}
        </span>
      </div>

      <div className="flex justify-between">
        <span className="text-slate-500">Failed Login Attempts</span>
        <span className="text-slate-800">
          {security.failedAttempts ?? 0}
        </span>
      </div>

      <div className="flex justify-between">
        <span className="text-slate-500">Account Status</span>
        <span
          className={`font-medium ${
            security.isLocked
              ? "text-red-600"
              : "text-green-600"
          }`}
        >
          {security.isLocked ? "Locked" : "Active"}
        </span>
      </div>

      <div className="flex justify-between">
        <span className="text-slate-500">Password Reset Required</span>
        <span className="text-slate-800">
          {security.passwordResetRequired ? "Yes" : "No"}
        </span>
      </div>

      <div className="flex justify-between">
        <span className="text-slate-500">MFA Enabled</span>
        <span className="text-slate-800">
          {security.mfaEnabled ? "Yes" : "No"}
        </span>
      </div>
    </div>
  );
}
