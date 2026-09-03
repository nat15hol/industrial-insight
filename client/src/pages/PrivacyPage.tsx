import { Link } from 'react-router-dom'

function PrivacyPage() {
  return (
    <main className="mx-auto max-w-2xl p-6">
      <h1 className="mb-2 text-4xl font-bold">Privacy Notice</h1>

      <p className="text-sm opacity-70">
        This notice describes how Industrial Insight, an educational student
        prototype, handles account data. It is not a production-grade legal
        privacy policy.
      </p>

      <div className="h-8" />
      <section className="mb-6">
        <h2 className="mb-2 text-xl font-semibold">Data We Collect</h2>
        <p>
          When you create an account, we collect your name, email address,
          and a hashed version of your password. We also store your assigned
          role (Manager or Technician), which determines what you can access
          in the application.
        </p>
      </section>

      <section className="mb-6">
        <h2 className="mb-2 text-xl font-semibold">How It's Used</h2>
        <p>
          Account information is used solely to authenticate you, maintain
          your session, and enforce role-based access control within the
          application. It is not used for marketing, analytics, or shared
          with any third party.
        </p>
      </section>

      <section className="mb-6">
        <h2 className="mb-2 text-xl font-semibold">Password Security</h2>
        <p>
          Passwords are hashed using BCrypt before being stored. We never
          store or have access to your plain-text password.
        </p>
      </section>

      <section className="mb-6">
        <h2 className="mb-2 text-xl font-semibold">
          Where Data Is Stored
        </h2>
        <p>
          All application data, including account information, is stored in
          a SQL Server database running on the same machine where the
          application is hosted for demonstration purposes. The prototype
          does not transmit data to any external server, cloud service, or
          third party — there is no centralized data collection by the
          developers beyond the local database instance used to run the
          system.
        </p>
      </section>

      <section className="mb-6">
        <h2 className="mb-2 text-xl font-semibold">Data Retention</h2>
        <p>
          Account data is retained for as long as the account exists. As
          this is a prototype, no automated data retention or deletion
          mechanism has been implemented; data removal would currently
          require manual database administration.
        </p>
      </section>

      <section className="mb-6">
        <h2 className="mb-2 text-xl font-semibold">Your Rights</h2>
        <p>
          Under GDPR, you would ordinarily have the right to access, correct,
          export, or delete your personal data. This prototype does not yet
          implement self-service tools for these rights (e.g. account
          deletion or data export) — this is documented as a future
          improvement rather than a current feature.
        </p>
      </section>

      <section className="mb-10">
        <h2 className="mb-2 text-xl font-semibold">Prototype Disclaimer</h2>
        <p>
          Industrial Insight is a student project built for educational
          purposes. It is not a production system, has not undergone a
          formal security or compliance review, and should not be used with
          real personal or operational data.
        </p>
      </section>

      <p className="text-sm opacity-70">
        <Link to="/login" className="underline">
          Back to Login
        </Link>
      </p>
    </main>
  )
}

export default PrivacyPage
