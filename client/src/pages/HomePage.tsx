import { useEffect, useState } from 'react'

type DashboardStats = {
  totalMachines: number
  openIncidents: number
}
type LatestPipeline = {
  status: string
  dataQualityPct: number
}

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL

function HomePage() {
  const [stats, setStats] = useState<DashboardStats | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [pipeline, setPipeline] = useState<LatestPipeline | null>(null)
  const [isPipelineLoading, setIsPipelineLoading] = useState(true)
  const [pipelineError, setPipelineError] = useState<string | null>(null)

  useEffect(() => {
    const token = localStorage.getItem('token')

    fetch(`${API_BASE_URL}/api/Dashboard/stats`, {
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
      .then((data: DashboardStats) => {
        setStats(data)
        setIsLoading(false)
      })
      .catch((error) => {
        console.error('Failed to fetch dashboard stats:', error)
        setError('Failed to load dashboard statistics.')
        setIsLoading(false)
      })
  }, [])

  useEffect(() => {
    const token = localStorage.getItem('token')

    fetch(`${API_BASE_URL}/api/Dashboard/pipeline`, {
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
      .then((data: LatestPipeline) => {
        setPipeline(data)
        setIsPipelineLoading(false)
      })
      .catch((error) => {
        console.error('Failed to fetch latest pipeline:', error)
        setPipelineError('Failed to load latest pipeline.')
        setIsPipelineLoading(false)
      })
  }, [])

  if (isLoading) {
    return <p>Loading dashboard...</p>
  }

  if (error) {
    return <p>{error}</p>
  }

  if (!stats) {
    return <p>No dashboard data available.</p>
  }

  return (
    <main>
      <header className="mb-8">
        <h1 className="text-4xl font-bold">Dashboard</h1>
        <p className="text-gray-600">
          Overview of the current system status.
        </p>
      </header>

      <section className="grid grid-cols-1 gap-6 md:grid-cols-2">
        <article className="rounded border p-6">
          <h2 className="text-sm font-semibold text-gray-600">
            Total Machines
          </h2>
          <p className="mt-2 text-4xl font-bold">{stats.totalMachines}</p>
        </article>

        <article className="rounded border p-6">
          <h2 className="text-sm font-semibold text-gray-600">
            Open Incidents
          </h2>
          <p className="mt-2 text-4xl font-bold">{stats.openIncidents}</p>
        </article>

        <article className="rounded border p-6">
          <h2 className="text-sm font-semibold text-gray-600">
            Latest Pipeline
          </h2>

          {isPipelineLoading ? (
            <p className="mt-2">Loading...</p>
          ) : pipelineError ? (
            <p className="mt-2">{pipelineError}</p>
          ) : (
            <p className="mt-2 text-4xl font-bold">{pipeline?.status}</p>
          )}
        </article>

        <article className="rounded border p-6">
          <h2 className="text-sm font-semibold text-gray-600">
            Data Quality
          </h2>

          {isPipelineLoading ? (
            <p className="mt-2">Loading...</p>
          ) : pipelineError ? (
            <p className="mt-2">{pipelineError}</p>
          ) : (
            <p className="mt-2 text-4xl font-bold">
              {pipeline?.dataQualityPct}%
            </p>
          )}
        </article>
      
      </section>
    </main>
  )
}

export default HomePage