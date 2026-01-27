import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { connectFacebook } from "../api/auth.api";
import { loadCapabilities } from "../store/capabilities.store";

export default function Login() {
  const navigate = useNavigate();

  useEffect(() => {
    loadCapabilities()
      .then((caps) => {
        if (caps.connected) {
          navigate("/dashboard", { replace: true });
        }
      })
      .catch(() => {});
  }, [navigate]);

  const handleLogout = () => {
    // Delegate logout to CRM shell
    window.location.href = "/logout";
  };

  return (
    <div style={{ padding: 40, textAlign: "center" }}>
      {/* Logout always available */}
      <div style={{ textAlign: "right" }}>
        <button
          onClick={handleLogout}
          style={{
            background: "transparent",
            border: "1px solid #ddd",
            padding: "6px 12px",
            cursor: "pointer",
            color:"black"
          }}
        >
          Logout
        </button>
      </div>

      <h1>SocialMediaCRM</h1>
      <p>Manage Facebook like Zoho Social</p>

      <button
        onClick={connectFacebook}
        style={{ marginTop: 20, padding: "10px 20px" }}
      >
        Connect Facebook
      </button>
    </div>
  );
}
