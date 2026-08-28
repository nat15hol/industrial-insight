import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

type Incident = {
  incidentId: number
  description: string
  machineId: number
}

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL

const DEFAULT_STATUS = 'Pending'

function AssignTaskPage() {
  const navigate = useNavigate()
  const { user } = useAuth()

  const [incidents, setIncidents] = useState<Incident[]>([])
  const [isLoadingIncidents, setIsLoadingIncidents] = useState(true)
  const [incidentsError, setIncidentsError] = useState<string | null>(null)

  const [incidentId, setIncidentId] = useState('')
  const [assignedToUserId, setAssignedToUserId] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [formError, setFormError] = useState<string | null>(null)
  const [successMessage, setSuccessMessage] = useState<string | null>(null)

  useEffect(() => {
    const token = localStorage.getItem('token')

    fetch(`${API_BASE_URL}/api/Incident`, {
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
      .then((data: Incident[]) => {
        setIncidents(data)
        setIsLoadingIncidents(false)
      })
      .catch((error) => {
        console.error('Failed to fetch incidents:', error)
        setIncidentsError('Failed to load incidents.')
        setIsLoadingIncidents(false)
      })
  }, [])

  const handleSubmit = (event: React.FormEvent) => {
    event.preventDefault()
    setFormError(null)
    setSuccessMessage(null)

    if (!incidentId) {
      setFormError('Please select an incident.')
      return
    }

    if (!assignedToUserId.trim()) {
      setFormError('Please enter a user ID to assign the task to.')
      return
    }

    const token = localStorage.getItem('token')
    setIsSubmitting(true)

    fetch(`${API_BASE_URL}/api/MaintenanceTask`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify({
        status: DEFAULT_STATUS,
        incidentId: Number(incidentId),
        assignedToUserId: Number(assignedToUserId),
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
        setSuccessMessage('Task assigned. Redirecting...')
        setTimeout(() => navigate('/tasks'), 2000)
      })
      .catch((error) => {
        console.error('Failed to assign task:', error)
        setFormError(
          'Something went wrong while assigning the task. Please try again.',
        )
        setIsSubmitting(false)
      })
  }

  if (user?.role !== 'Manager') {
    return (
      <main className="task-assign-page">
        <header className="page-header">
          <h1 className="text-4xl font-bold">Assign maintenance task</h1>
        </header>

        <p className="rounded border border-red-300 bg-red-50 p-3 text-red-700">
          Only Managers can assign maintenance tasks.
        </p>
      </main>
    )
  }

  return (
    <main className="task-assign-page">
      <header className="page-header">
        <h1 className="text-4xl font-bold">Assign maintenance task</h1>
        <p>Select an incident and assign it to a technician.</p>
      </header>

      <section className="task-assign-form">
        {isLoadingIncidents ? (
          <p>Loading incidents...</p>
        ) : incidentsError ? (
          <p>{incidentsError}</p>
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
              <label htmlFor="incident" className="text-sm font-semibold">
                Incident
              </label>
              <select
                id="incident"
                value={incidentId}
                onChange={(event) => setIncidentId(event.target.value)}
                className="rounded border p-2"
              >
                <option value="">Select an incident</option>
                {incidents.map((incident) => (
                  <option
                    key={incident.incidentId}
                    value={incident.incidentId}
                  >
                    #{incident.incidentId} — Machine #{incident.machineId} —{' '}
                    {incident.description.slice(0, 40)}
                  </option>
                ))}
              </select>
            </div>

            <div className="flex flex-col gap-1">
              <label
                htmlFor="assignedToUserId"
                className="text-sm font-semibold"
              >
                Assign to (User ID)
              </label>
              <input
                id="assignedToUserId"
                type="number"
                min={1}
                value={assignedToUserId}
                onChange={(event) => setAssignedToUserId(event.target.value)}
                className="rounded border p-2"
              />
            </div>

            <button
              type="submit"
              disabled={isSubmitting}
              className="rounded bg-gray-900 px-4 py-2 text-white hover:bg-gray-700 disabled:opacity-50"
            >
              {isSubmitting ? 'Assigning...' : 'Assign task'}
            </button>
          </form>
        )}
      </section>
    </main>
  )
}

export default AssignTaskPage
