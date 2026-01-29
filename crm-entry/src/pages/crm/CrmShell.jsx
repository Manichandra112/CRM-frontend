import { useParams } from "react-router-dom";
import SocialEntry from "../../socialCRM/SocialEntry";

export default function CrmShell() {
  const { domainCode } = useParams();

  switch (domainCode) {
    case "socialmedia":
      return <SocialEntry />;

    case "sales":
      return <div>Sales CRM (coming soon)</div>;

    case "hr":
      return <div>HR CRM (coming soon)</div>;

    default:
      return (
        <div style={{ padding: 40 }}>
          <h2>CRM Not Available</h2>
          <p>This CRM does not exist or is not enabled.</p>
        </div>
      );
  }
}
