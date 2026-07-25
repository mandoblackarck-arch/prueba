export default function Template({ children }: { children: React.ReactNode }) {
  return <div className="animate__animated animate__fadeInUp page-transition"style={{ animationDuration: '1s', animationDelay: '0.3s' }}>{children}</div>
}
