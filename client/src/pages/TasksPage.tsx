import { useEffect, useState } from 'react'

type MaintenanceTask = {
  maintenanceTaskId: number
  status: string
  createdAt: string
  completedAt: string | null
  incidentId: number
  assignedToUserId: number
}

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL

const STATUS_OPTIONS = ['Pending', 'InProgress', 'Completed']

function TasksPage() {
  const [tasks, setTasks] = useState<MaintenanceTask[]>([])
  const [selectedTask, setSelectedTask] = useState<MaintenanceTask | null>(
    null,
  )
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [isUpdating, setIsUpdating] = useState(false)
  const [updateError, setUpdateError] = useState<string | null>(null)

  const fetchTasks = () => {
    const token = localStorage.getItem('token')
    setIsLoading(true)
    setError(null)

    fetch(`${API_BASE_URL}/api/MaintenanceTask`, {
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
      .then((data: MaintenanceTask[]) => {
        setTasks(data)
        setIsLoading(false)
      })
      .catch((error) => {
        console.error('Failed to fetch tasks:', error)
        setError('Failed to load tasks.')
        setIsLoading(false)
      })
  }

  useEffect(() => {
    fetchTasks()
  }, [])

  const handleStatusChange = (newStatus: string) => {
    if (!selectedTask) {
      return
    }

    const token = localStorage.getItem('token')
    setIsUpdating(true)
    setUpdateError(null)

    fetch(
      `${API_BASE_URL}/api/MaintenanceTask/${selectedTask.maintenanceTaskId}`,
      {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({ status: newStatus }),
      },
    )
      .then((response) => {
        if (!response.ok) {
          throw new Error(`HTTP error ${response.status}`)
        }

        return response.json()
      })
      .then((updated: MaintenanceTask) => {
        setSelectedTask(updated)
        setTasks((prev) =>
          prev.map((task) =>
            task.maintenanceTaskId === updated.maintenanceTaskId
              ? updated
              : task,
          ),
        )
        setIsUpdating(false)
      })
      .catch((error) => {
        console.error('Failed to update task:', error)
        setUpdateError('Failed to update status. Please try again.')
        setIsUpdating(false)
      })
  }

  return (
    <main className="task-page">
      <header className="page-header">
        <h1 className="text-4xl font-bold">Maintenance tasks</h1>
        <p>Browse tasks and update their status.</p>
      </header>

      <section className="task-layout grid grid-cols-1 gap-6 md:grid-cols-2">
        <div className="task-list">
          <h2 className="mb-3 text-xl font-semibold">Task list</h2>

          {isLoading ? (
            <p>Loading tasks...</p>
          ) : error ? (
            <div className="flex flex-col items-start gap-2">
              <p className="text-red-600">{error}</p>
              <button
                type="button"
                onClick={fetchTasks}
                className="rounded border px-3 py-1 text-sm hover:bg-gray-50"
              >
                Retry
              </button>
            </div>
          ) : tasks.length === 0 ? (
            <p>No tasks found.</p>
          ) : (
            <div className="flex flex-col gap-2">
              {tasks.map((task) => (
                <button
                  key={task.maintenanceTaskId}
                  type="button"
                  className="task-card flex flex-col items-start gap-1 rounded border p-3 text-left hover:bg-gray-50"
                  onClick={() => {
                    setSelectedTask(task)
                    setUpdateError(null)
                  }}
                >
                  <strong>Task #{task.maintenanceTaskId}</strong>
                  <span className="text-sm text-gray-600">
                    Incident #{task.incidentId}
                  </span>
                  <span className="text-sm text-gray-600">
                    {task.status}
                  </span>
                </button>
              ))}
            </div>
          )}
        </div>

        <div className="task-detail mt-9 rounded border p-4">
          {selectedTask ? (
            <>
              <h2 className="mb-3 text-xl font-semibold">
                Task #{selectedTask.maintenanceTaskId}
              </h2>

              <dl className="flex flex-col gap-2">
                <div>
                  <dt className="text-sm font-semibold text-gray-600">
                    Incident
                  </dt>
                  <dd>Incident #{selectedTask.incidentId}</dd>
                </div>

                <div>
                  <dt className="text-sm font-semibold text-gray-600">
                    Assigned to
                  </dt>
                  <dd>User #{selectedTask.assignedToUserId}</dd>
                </div>

                <div>
                  <dt className="text-sm font-semibold text-gray-600">
                    Status
                  </dt>
                  <dd>{selectedTask.status}</dd>
                </div>

                <div>
                  <dt className="text-sm font-semibold text-gray-600">
                    Created at
                  </dt>
                  <dd>{new Date(selectedTask.createdAt).toLocaleString()}</dd>
                </div>

                <div>
                  <dt className="text-sm font-semibold text-gray-600">
                    Completed at
                  </dt>
                  <dd>
                    {selectedTask.completedAt
                      ? new Date(selectedTask.completedAt).toLocaleString()
                      : 'Not completed'}
                  </dd>
                </div>
              </dl>

              <div className="mt-4 flex flex-col gap-2">
                <label
                  htmlFor="status-select"
                  className="text-sm font-semibold"
                >
                  Update status
                </label>
                <select
                  id="status-select"
                  value={selectedTask.status}
                  onChange={(event) => handleStatusChange(event.target.value)}
                  disabled={isUpdating}
                  className="rounded border p-2"
                >
                  {STATUS_OPTIONS.map((status) => (
                    <option key={status} value={status}>
                      {status}
                    </option>
                  ))}
                </select>

                {updateError && (
                  <p className="text-sm text-red-600">{updateError}</p>
                )}
              </div>
            </>
          ) : (
            <p>Select a task to view its details.</p>
          )}
        </div>
      </section>
    </main>
  )
}

export default TasksPage
