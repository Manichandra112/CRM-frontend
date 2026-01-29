// import { useEffect, useState } from "react";
// import { useNavigate } from "react-router-dom";
// import { getDomains } from "../../api/domains.api";

// const DOMAIN_ROUTE_MAP = {
//   SOCIALMEDIA: "/social/dashboard",
//   SALES: "/sales/dashboard",
//   HR: "/hr/dashboard",
// };

// export default function DomainSwitcher() {
//   const navigate = useNavigate();
//   const [domains, setDomains] = useState([]);

//   useEffect(() => {
//     getDomains().then(data => {
//       // ✅ ONLY ACTIVE DOMAINS
//       setDomains(data.filter(d => d.active && DOMAIN_ROUTE_MAP[d.code]));
//     });
//   }, []);

//   if (domains.length === 0) {
//     return (
//       <div style={{ padding: 40 }}>
//         <h2>No Active Domains</h2>
//         <p>
//           No CRM domains are active yet.  
//           Go to <b>Domains</b> and activate one.
//         </p>
//       </div>
//     );
//   }

//   return (
//     <div style={{ padding: 24 }}>
//       <h2>Available CRMs</h2>

//       <div style={{ display: "flex", gap: 20, marginTop: 20 }}>
//         {domains.map(d => (
//           <div
//             key={d.id}
//             onClick={() => navigate(DOMAIN_ROUTE_MAP[d.code])}
//             style={{
//               border: "1px solid #e5e7eb",
//               padding: 20,
//               cursor: "pointer",
//               borderRadius: 8,
//               background: "#fff"
//             }}
//           >
//             <h3>{d.name}</h3>
//             <small>{d.code}</small>
//           </div>
//         ))}
//       </div>
//     </div>
//   );
// }
