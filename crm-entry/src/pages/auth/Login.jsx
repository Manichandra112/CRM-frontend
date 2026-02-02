import { useState } from "react";
import { Navigate, useNavigate } from "react-router-dom";
import { login } from "../../api/auth.api";
import { useAuth } from "../../auth/AuthContext";

const Login = () => {
  const navigate = useNavigate();
  const { setSession, isAuthenticated } = useAuth();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [showPwd, setShowPwd] = useState(false);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  // ✅ If already logged in, go to home ONCE
  if (isAuthenticated) {
    return <Navigate to="/" replace />;
  }

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (loading) return;

    setError(null);
    setLoading(true);

    try {
      const res = await login({
        email: email.trim(),
        password,
      });

      // ✅ Save token
      setSession(res.data.accessToken);

      // ✅ Route ONLY on success
      navigate("/", { replace: true });
    } catch (err) {
      // ❌ Wrong credentials → stay here
      setError("Invalid email or password");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-md mx-auto mt-24 bg-white p-6 rounded shadow">
      <h2 className="text-lg font-semibold mb-4 text-center">
        CRM Login
      </h2>

      {error && (
        <div className="mb-3 text-sm text-red-600">
          {error}
        </div>
      )}

      <form onSubmit={handleSubmit}>
        {/* Email */}
        <div className="mb-3">
          <input
            type="email"
            placeholder="Email"
            value={email}
            autoComplete="username"
            onChange={(e) => setEmail(e.target.value)}
            className="w-full border p-2 rounded"
            disabled={loading}
          />
        </div>

        {/* Password */}
        <div className="mb-4 relative">
          <input
            type={showPwd ? "text" : "password"}
            placeholder="Password"
            value={password}
            autoComplete="current-password"
            onChange={(e) => setPassword(e.target.value)}
            className="w-full border p-2 rounded pr-10"
            disabled={loading}
          />

          <button
            type="button"
            onClick={() => setShowPwd((v) => !v)}
            className="absolute right-2 top-2 text-sm text-slate-500"
            tabIndex={-1}
          >
            {showPwd ? "🙈" : "👁️"}
          </button>
        </div>

        <button
          type="submit"
          disabled={loading}
          className="w-full bg-blue-600 text-white py-2 rounded disabled:opacity-50"
        >
          {loading ? "Logging in…" : "Login"}
        </button>

        <div className="mt-3 text-sm text-center">
          <a
            href="/forgot-password"
            className="text-blue-600 hover:underline"
          >
            Forgot password?
          </a>
        </div>
      </form>
    </div>
  );
};

export default Login;
