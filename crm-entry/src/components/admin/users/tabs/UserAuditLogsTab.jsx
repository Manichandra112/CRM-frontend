const UserAuditLogsTab = ({ logs }) => {
  if (!logs.length) return <p>No audit logs</p>;

  return (
    <ul>
      {logs.map((l, i) => (
        <li key={i}>
          <b>{l.action}</b> – {l.module}
          <br />
          <small>
            {l.actorName} • {new Date(l.createdAt).toLocaleString()}
          </small>
          {l.metadata && <p>{l.metadata}</p>}
        </li>
      ))}
    </ul>
  );
};

export default UserAuditLogsTab;
