import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'

type Machine = {
  machineId: number
  name: string
  status: string
}

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL

// Incident data contract (docs/decisions/incident-data-contract.md):
// technicians only choose Machine + Description. Status/Priority/Category
// are filled with defaults here; Manager can change Priority/Category later.
const DEFAULT_STATUS = 'Open'
const DEFAULT_PRIORITY = 'Medium'
const DEFAULT_CATEGORY = 'Other'

function CreateIncidentPage() {
  const navigate = useNavigate()

  const [machines, setMachines] = useState<Machine[]>([])
  const [isLoadingMachines, setIsLoadingMachines] = useState(true)
  const [machinesError, setMachinesError] = useState<string | null>(null)

  const [machineId, setMachineId] = useState('')
  const [description, setDescription] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [formError, setFormError] = useState<string | null>(null)
  const [successMessage, setSuccessMessage] = useState<string | null>(null)

  useEffect(() => {
    const token = localStorage.getItem('token')

    fetch(`${API_BASE_URL}/api/Machine`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    })
      .then((response) => {
        if (!response.ok) {
          throw new Error(`HTTP error ${response.status}`)
        }

        return response.json()
      })
      .then((data: Machine[]) => {
        setMachines(data)
        setIsLoadingMachines(false)
      })
      .catch((error) => {
        console.error('Failed to fetch machines:', error)
        setMachinesError('Failed to load machines.')
        setIsLoadingMachines(false)
      })
  }, [])

  const handleSubmit = (event: React.FormEvent) => {
    event.preventDefault()
    setFormError(null)
    setSuccessMessage(null)

    if (!machineId) {
      setFormError('Please select a machine.')
      return
    }

    if (description.trim().length === 0) {
      setFormError('Please enter a description.')
      return
    }

    const token = localStorage.getItem('token')
    setIsSubmitting(true)

    fetch(`${API_BASE_URL}/api/Incident`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify({
        machineId: Number(machineId),
        description,
        status: DEFAULT_STATUS,
        priority: DEFAULT_PRIORITY,
        category: DEFAULT_CATEGORY,
      }),
    })
      .then((response) => {
        if (!response.ok) {
          throw new Error(`HTTP error ${response.status}`)
        }

        return response.json()
      })
      .then(() => {
        setIsSubmitting(false)
        setSuccessMessage('Incident reported. Redirecting...')
        setTimeout(() => navigate('/incidents'), 2500)
      })
      .catch((error) => {
        console.error('Failed to create incident:', error)
        setFormError(
          'Something went wrong while reporting the incident. Please try again.',
        )
        setIsSubmitting(false)
      })
  }

  return (
    <main className="incident-create-page">
      <header className="page-header">
        <h1 className="text-4xl font-bold">Report incident</h1>
        <p>Select a machine and describe the issue.</p>
      </header>

      <section className="incident-create-form">
        {isLoadingMachines ? (
          <p>Loading machines...</p>
        ) : machinesError ? (
          <p>{machinesError}</p>
        ) : (
          <form
            onSubmit={handleSubmit}
            className="mx-auto flex max-w-md flex-col gap-4"
          >
            {formError && (
              <p className="rounded border border-red-300 bg-red-50 p-2 text-sm text-red-700">
                {formError}
              </p>
            )}

            {successMessage && (
              <p className="rounded border border-green-300 bg-green-50 p-2 text-sm text-green-700">
                {successMessage}
              </p>
            )}

            <div className="flex flex-col gap-1">
              <label htmlFor="machine" className="text-sm font-semibold">
                Machine
              </label>
              <select
                id="machine"
                value={machineId}
                onChange={(event) => setMachineId(event.target.value)}
                className="rounded border p-2"
              >
                <option value="">Select a machine</option>
                {machines.map((machine) => (
                  <option key={machine.machineId} value={machine.machineId}>
                    {machine.name}
                  </option>
                ))}
              </select>
            </div>

            <div className="flex flex-col gap-1">
              <label htmlFor="description" className="text-sm font-semibold">
                Description
              </label>
              <textarea
                id="description"
                value={description}
                onChange={(event) => setDescription(event.target.value)}
                rows={5}
                className="rounded border p-2"
              />
            </div>

            <button
              type="submit"
              disabled={isSubmitting}
              className="rounded bg-gray-900 px-4 py-2 text-white hover:bg-gray-700 disabled:opacity-50"
            >
              {isSubmitting ? 'Reporting...' : 'Report incident'}
            </button>
          </form>
        )}
      </section>
    </main>
  )
}

export default CreateIncidentPage
