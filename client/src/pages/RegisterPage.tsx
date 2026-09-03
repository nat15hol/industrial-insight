import { useState } from 'react'
import type { FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

function RegisterPage() {
  const { register } = useAuth()
  const navigate = useNavigate()

  const [name, setName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [error, setError] = useState('')

  const hasMinLength = password.length >= 8
  const hasUppercase = /[A-Z]/.test(password)
  const hasNumber = /\d/.test(password)
  const hasSpecialCharacter = /[^A-Za-z0-9]/.test(password)

  const passwordsMatch =
    password.length > 0 &&
    confirmPassword.length > 0 &&
    password === confirmPassword

  const canSubmit =
    name.trim() !== '' &&
    email.trim() !== '' &&
    hasMinLength &&
    hasUppercase &&
    hasNumber &&
    hasSpecialCharacter &&
    passwordsMatch

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    setError('')

    if (
      !hasMinLength ||
      !hasUppercase ||
      !hasNumber ||
      !hasSpecialCharacter
    ) {
      setError(
        'Please meet all password requirements before creating your account.',
      )
      return
    }

    if (password !== confirmPassword) {
      setError('Passwords do not match.')
      return
    }

    try {
      await register(name, email, password)
      navigate('/login')
    } catch (error) {
      setError(
        error instanceof Error ? error.message : 'Registration failed.',
      )
    }
  }

  const requirementClass = (met: boolean) =>
    met ? 'text-green-600' : 'text-gray-500'

  return (
    <main className="mx-auto max-w-md p-6">
      <div className="space-y-8">
        <div>
          <h1 className="mb-2 text-4xl font-bold">Create Account</h1>

          <p className="text-sm text-gray-500">
            Create an account to access Industrial Insight.
          </p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label htmlFor="name" className="mb-1 block">
              Name
            </label>

            <input
              id="name"
              type="text"
              value={name}
              onChange={(event) => setName(event.target.value)}
              required
              autoComplete="name"
              className="w-full rounded border p-2"
            />
          </div>

          <div>
            <label htmlFor="email" className="mb-1 block">
              Email
            </label>

            <input
              id="email"
              type="email"
              value={email}
              onChange={(event) => setEmail(event.target.value)}
              required
              autoComplete="email"
              className="w-full rounded border p-2"
            />
          </div>

          <div>
            <label htmlFor="password" className="mb-1 block">
              Password
            </label>

            <input
              id="password"
              type="password"
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              required
              autoComplete="new-password"
              className="w-full rounded border p-2"
            />

            {password.length > 0 && (
              <div className="mt-2 space-y-1 text-sm">
                <p className={requirementClass(hasMinLength)}>
                  {hasMinLength ? '✓' : '○'} At least 8 characters
                </p>

                <p className={requirementClass(hasUppercase)}>
                  {hasUppercase ? '✓' : '○'} One uppercase letter
                </p>

                <p className={requirementClass(hasNumber)}>
                  {hasNumber ? '✓' : '○'} One number
                </p>

                <p className={requirementClass(hasSpecialCharacter)}>
                  {hasSpecialCharacter ? '✓' : '○'} One special character
                </p>
              </div>
            )}
          </div>

          <div>
            <label htmlFor="confirmPassword" className="mb-1 block">
              Confirm Password
            </label>

            <input
              id="confirmPassword"
              type="password"
              value={confirmPassword}
              onChange={(event) => setConfirmPassword(event.target.value)}
              required
              autoComplete="new-password"
              className="w-full rounded border p-2"
            />

            {confirmPassword.length > 0 && (
              <p
                className={`mt-1 text-sm ${
                  passwordsMatch ? 'text-green-600' : 'text-red-600'
                }`}
              >
                {passwordsMatch
                  ? '✓ Passwords match'
                  : 'Passwords do not match'}
              </p>
            )}
          </div>

          {error && (
            <p className="text-sm text-red-600" role="alert">
              {error}
            </p>
          )}

          <button
            type="submit"
            disabled={!canSubmit}
            className={`w-full rounded px-4 py-2 text-white transition ${
              canSubmit
                ? 'bg-blue-600 hover:bg-blue-700'
                : 'cursor-not-allowed bg-gray-400'
            }`}
          >
            Create Account
          </button>
        </form>

        <p className="pb-4 text-sm">
          Already have an account?{' '}
          <Link to="/login" className="text-blue-600 hover:underline">
            Sign in
          </Link>
        </p>

        <p className="text-xs text-gray-500">
  By creating an account, you agree to use this prototype responsibly.
  Account information is used for authentication and application
  functionality. Passwords are hashed and never stored in plain text.{' '}
  <Link to="/privacy" className="underline">
    Privacy Notice
  </Link>
</p>
      </div>
    </main>
  )
}

export default RegisterPage