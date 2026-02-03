import { useEffect, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import {
  resetForgotPassword,
} from "../../api/auth.api";
import api from "../../api/axios";

export default function ResetForgotPassword() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();

  const token = searchParams.get("token");

  const [valid, setValid] = useState(null);
  const [password, setPassword] = useState("");
  const [confirm, setConfirm] = useState("");
  const [error, setError] = useState(null);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    if (!token) {
      setValid(false);
      return;
    }

    api.get("/api/auth/validate-reset-token", {
      params: { token },
      skipAuth: true,
    })
      .then(() => setValid(true))
      .catch(() => setValid(false));
  }, [token]);

  const submit = async () => {
    if (!password || !confirm) {
      setError("Password is required");
      return;
    }

    if (password !== confirm) {
      setError("Passwords do not match");
      return;
    }

    try {
      setSaving(true);
      setError(null);

      await resetForgotPassword({
        token,
        newPassword: password,
      });

      navigate("/login", { replace: true });
    } catch {
      setError("Reset link expired or already used");
    } finally {
      setSaving(false);
    }
  };

  if (valid === null) {
    return <div className="max-w-md mx-auto mt-24 text-center">Validating reset link…</div>;
  }

  if (!valid) {
    return (
      <div className="max-w-md mx-auto mt-24 text-center text-red-600">
        This reset link is invalid or has expired.
      </div>
    );
  }

  return (
    <div className="max-w-md mx-auto mt-24 bg-white p-6 rounded shadow">
      <h2 className="text-lg font-semibold mb-4">Reset Password</h2>

      {error && <div className="mb-3 text-sm text-red-600">{error}</div>}

      <input
        type="password"
        placeholder="New password"
        className="w-full border p-2 mb-2"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
      />

      <input
        type="password"
        placeholder="Confirm password"
        className="w-full border p-2 mb-4"
        value={confirm}
        onChange={(e) => setConfirm(e.target.value)}
      />

      <button
        onClick={submit}
        disabled={saving}
        className="w-full bg-blue-600 text-white py-2 rounded disabled:opacity-50"
      >
        {saving ? "Saving…" : "Reset Password"}
      </button>
    </div>
  );
}
