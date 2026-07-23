export default function Template({ children }: { children: React.ReactNode }) {
  return <div className="animate__animated animate__fadeIn page-transition">{children}</div>;
}
