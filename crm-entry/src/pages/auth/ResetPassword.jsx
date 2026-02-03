import { useState } from "react";
import { useAuth } from "../../auth/AuthContext";
import { forceChangePassword } from "../../api/auth.api";

export default function ResetPassword() {
  const { logout } = useAuth();

  const [pwd, setPwd] = useState("");
  const [confirm, setConfirm] = useState("");
  const [error, setError] = useState(null);
  const [saving, setSaving] = useState(false);

  const submit = async () => {
    if (!pwd || !confirm) {
      setError("Password is required");
      return;
    }

    if (pwd !== confirm) {
      setError("Passwords do not match");
      return;
    }

    try {
      setSaving(true);
      setError(null);

      await forceChangePassword({
        newPassword: pwd,
      });

      logout(); // forces fresh login session
    } catch {
      setError("Failed to update password");
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="max-w-md mx-auto mt-24 bg-white p-6 rounded shadow">
      <h2 className="text-lg font-semibold mb-4">Set New Password</h2>

      {error && (
        <div className="mb-3 text-sm text-red-600">{error}</div>
      )}

      <input
        type="password"
        placeholder="New password"
        className="w-full border p-2 mb-2"
        value={pwd}
        onChange={(e) => {
          setPwd(e.target.value);
          setError(null);
        }}
      />

      <input
        type="password"
        placeholder="Confirm password"
        className="w-full border p-2 mb-4"
        value={confirm}
        onChange={(e) => {
          setConfirm(e.target.value);
          setError(null);
        }}
      />

      <button
        onClick={submit}
        disabled={saving}
        className="w-full bg-blue-600 text-white py-2 rounded disabled:opacity-50"
      >
        {saving ? "Saving…" : "Save & Login Again"}
      </button>
    </div>
  );
}
