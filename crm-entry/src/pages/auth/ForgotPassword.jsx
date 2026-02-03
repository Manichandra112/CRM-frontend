import { useState } from "react";
import { forgotPassword } from "../../api/auth.api";

export default function ForgotPassword() {
  const [email, setEmail] = useState("");
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState(null);
  const [error, setError] = useState(null);

  const submit = async () => {
    if (!email.trim()) {
      setError("Email is required");
      return;
    }

    try {
      setLoading(true);
      setError(null);

      await forgotPassword(email.trim());

      setMessage(
        "If the email exists, a password reset link has been sent."
      );
    } catch {
      setError("Something went wrong. Please try again.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-md mx-auto mt-24 bg-white p-6 rounded shadow">
      <h2 className="text-lg font-semibold mb-2">Forgot Password</h2>

      <p className="text-sm text-slate-500 mb-4">
        Enter your email address to receive a password reset link.
      </p>

      {error && (
        <div className="mb-3 text-sm text-red-600">{error}</div>
      )}

      {message ? (
        <div className="text-sm text-green-600">{message}</div>
      ) : (
        <>
          <input
            type="email"
            placeholder="Email address"
            className="w-full border p-2 mb-4"
            value={email}
            onChange={(e) => {
              setEmail(e.target.value);
              setError(null);
            }}
            disabled={loading}
          />

          <button
            onClick={submit}
            disabled={loading}
            className="w-full bg-blue-600 text-white py-2 rounded disabled:opacity-50"
          >
            {loading ? "Sending…" : "Send Reset Link"}
          </button>
        </>
      )}
    </div>
  );
}
