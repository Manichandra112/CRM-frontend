const UserDetailsTab = ({ data }) => {
  if (!data) return <p>Loading...</p>;

  return (
    <div>
      <p><b>Username:</b> {data.username}</p>
      <p><b>Email:</b> {data.email}</p>
      <p><b>Status:</b> {data.accountStatus}</p>
      <p><b>Department:</b> {data.department ?? "-"}</p>
      <p><b>Designation:</b> {data.designation ?? "-"}</p>
    </div>
  );
};

export default UserDetailsTab;
