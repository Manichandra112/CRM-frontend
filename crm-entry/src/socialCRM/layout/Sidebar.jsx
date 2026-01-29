import { Link } from "react-router-dom";

export default function Sidebar() {
  return (
    <div
      style={{
        width: "220px",
        background: "#111827",
        color: "#e5e7eb",
        padding: "20px",
        display: "flex",
        flexDirection: "column"
      }}
    >
      <h2 style={{ marginBottom: "30px", color: "#fff" }}>
        SocialMediaCRM
      </h2>

      <nav style={{ display: "flex", flexDirection: "column", gap: "14px" }}>
        <Link to="/crm/socialmedia/dashboard">Dashboard</Link>
        <Link to="/crm/socialmedia/post/create">Create Post</Link>
        <Link to="/crm/socialmedia/post/multi">Multi-Page Post</Link>
        <Link to="/crm/socialmedia/analytics">Analytics</Link>
        <Link to="/crm/socialmedia/leads">Leads</Link>
        <Link to="/crm/socialmedia/leads/forms">Lead Forms</Link>

      </nav>
    </div>
  );
}
