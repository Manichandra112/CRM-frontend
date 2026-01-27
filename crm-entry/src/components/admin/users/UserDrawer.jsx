import { useEffect, useState } from "react";
import {
  getAdminUserDetails,
  getAdminUserSecurity,
  getAdminUserAuditLogs,
} from "../../../api/admin.api";

import UserDetailsTab from "./tabs/UserDetailsTab";
import UserSecurityTab from "./tabs/UserSecurityTab";
import UserAuditLogsTab from "./tabs/UserAuditLogsTab";

const UserDrawer = ({ userId, onClose }) => {
  const [tab, setTab] = useState("details");
  const [details, setDetails] = useState(null);
  const [security, setSecurity] = useState(null);
  const [logs, setLogs] = useState([]);

  useEffect(() => {
    if (!userId) return;

    getAdminUserDetails(userId).then((r) => setDetails(r.data));
    getAdminUserSecurity(userId).then((r) => setSecurity(r.data));
    getAdminUserAuditLogs(userId).then((r) => setLogs(r.data));
  }, [userId]);

  if (!userId) return null;

  return (
    <div style={drawerStyle}>
      <button onClick={onClose}>✕</button>

      <div style={{ display: "flex", gap: 8 }}>
        <button onClick={() => setTab("details")}>Details</button>
        <button onClick={() => setTab("security")}>Security</button>
        <button onClick={() => setTab("audit")}>Audit</button>
      </div>

      <hr />

      {tab === "details" && <UserDetailsTab data={details} />}
      {tab === "security" && <UserSecurityTab data={security} />}
      {tab === "audit" && <UserAuditLogsTab logs={logs} />}
    </div>
  );
};

const drawerStyle = {
  position: "fixed",
  top: 0,
  right: 0,
  width: 420,
  height: "100vh",
  background: "#fff",
  borderLeft: "1px solid #ccc",
  padding: 16,
  overflowY: "auto",
  zIndex: 1000,
};

export default UserDrawer;
