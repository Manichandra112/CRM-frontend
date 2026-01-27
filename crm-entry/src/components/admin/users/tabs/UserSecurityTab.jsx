const UserSecurityTab = ({ data }) => {
  if (!data) return <p>No security data</p>;

  return (
    <div>
      <p>Force Password Reset: {data.forcePasswordReset ? "Yes" : "No"}</p>
      <p>MFA Enabled: {data.mfaEnabled ? "Yes" : "No"}</p>
      <p>Failed Login Count: {data.failedLoginCount}</p>
      <p>Last Login: {data.lastLoginAt ?? "-"}</p>
    </div>
  );
};

export default UserSecurityTab;
