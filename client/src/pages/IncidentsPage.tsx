import { useEffect, useState } from 'react'

type Incident = {
  incidentId: number
  description: string
  status: string
  priority: string
  category: string
  aiSuggestion: string | null
  createdAt: string
  resolvedAt: string | null
  machineId: number
  machineName: string
  reportedByUserId: number
}

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL

function truncate(text: string, maxLength: number) {
  if (text.length <= maxLength) {
    return text
  }

  return `${text.slice(0, maxLength)}...`
}

function IncidentsPage() {
  const [incidents, setIncidents] = useState<Incident[]>([])
  const [selectedIncident, setSelectedIncident] = useState<Incident | null>(
    null,
  )
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const fetchIncidents = () => {
    const token = localStorage.getItem('token')
    setIsLoading(true)
    setError(null)

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
        setIsLoading(false)
      })
      .catch((error) => {
        console.error('Failed to fetch incidents:', error)
        setError('Failed to load incidents.')
        setIsLoading(false)
      })
  }

  useEffect(() => {
    fetchIncidents()
  }, [])

  return (
    <main className="incident-page">
      <header className="page-header">
        <h1 className="text-4xl font-bold">Incidents</h1>
        <p>Browse and inspect reported incidents.</p>
      </header>

      <section className="incident-layout grid grid-cols-1 gap-6 md:grid-cols-2">
        <div className="incident-list">
          <h2 className="mb-3 text-xl font-semibold">Incident list</h2>

          {isLoading ? (
            <p>Loading incidents...</p>
          ) : error ? (
            <div className="flex flex-col items-start gap-2">
              <p className="text-red-600">{error}</p>
              <button
                type="button"
                onClick={fetchIncidents}
                className="rounded border px-3 py-1 text-sm hover:bg-gray-50"
              >
                Retry
              </button>
            </div>
          ) : incidents.length === 0 ? (
            <p>No incidents found.</p>
          ) : (
            <div className="flex flex-col gap-2">
              {incidents.map((incident) => (
                <button
                  key={incident.incidentId}
                  type="button"
                  className="incident-card flex flex-col items-start gap-1 rounded border p-3 text-left hover:bg-gray-50"
                  onClick={() => setSelectedIncident(incident)}
                >
                  <strong>
                    Incident #{incident.incidentId} — {incident.machineName}
                  </strong>
                  <span>{truncate(incident.description, 60)}</span>
                  <span className="flex gap-2 text-sm text-gray-600">
                    <span>{incident.status}</span>
                    <span>·</span>
                    <span>{incident.priority}</span>
                    <span>·</span>
                    <span>{incident.category}</span>
                  </span>
                </button>
              ))}
            </div>
          )}
        </div>

        <div className="incident-detail mt-9 rounded border p-4">
          {selectedIncident ? (
            <>
              <h2 className="mb-3 text-xl font-semibold">
                Incident #{selectedIncident.incidentId}
              </h2>

              <dl className="flex flex-col gap-2">
                <div>
                  <dt className="text-sm font-semibold text-gray-600">
                    Machine
                  </dt>
                  <dd>{selectedIncident.machineName}</dd>
                </div>

                <div>
                  <dt className="text-sm font-semibold text-gray-600">
                    Description
                  </dt>
                  <dd>{selectedIncident.description}</dd>
                </div>

                <div>
                  <dt className="text-sm font-semibold text-gray-600">
                    Status
                  </dt>
                  <dd>{selectedIncident.status}</dd>
                </div>

                <div>
                  <dt className="text-sm font-semibold text-gray-600">
                    Priority
                  </dt>
                  <dd>{selectedIncident.priority}</dd>
                </div>

                <div>
                  <dt className="text-sm font-semibold text-gray-600">
                    Category
                  </dt>
                  <dd>{selectedIncident.category}</dd>
                </div>

                <div>
                  <dt className="text-sm font-semibold text-gray-600">
                    Reported at
                  </dt>
                  <dd>
                    {new Date(selectedIncident.createdAt).toLocaleString()}
                  </dd>
                </div>

                <div>
                  <dt className="text-sm font-semibold text-gray-600">
                    Resolved at
                  </dt>
                  <dd>
                    {selectedIncident.resolvedAt
                      ? new Date(selectedIncident.resolvedAt).toLocaleString()
                      : 'Not resolved'}
                  </dd>
                </div>

                <div>
                  <dt className="text-sm font-semibold text-gray-600">
                    AI suggestion
                  </dt>
                  <dd>{selectedIncident.aiSuggestion ?? 'None'}</dd>
                </div>
              </dl>
            </>
          ) : (
            <p>Select an incident to view its details.</p>
          )}
        </div>
      </section>
    </main>
  )
}

export default IncidentsPage
