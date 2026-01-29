import { useState, useEffect } from 'react';
import { tasksApi } from '../../services/tasksApi';
import { customersApi } from '../../services/customersApi';
import { contactsApi } from '../../services/contactsApi';
import type { TaskDto, CreateTaskDto, UpdateTaskDto, CustomerDto, ContactDto } from '../../types/models';
import './TasksPage.css';

function TasksPage() {
  const [tasks, setTasks] = useState<TaskDto[]>([]);
  const [customers, setCustomers] = useState<CustomerDto[]>([]);
  const [contacts, setContacts] = useState<ContactDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(10);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedCustomerId, setSelectedCustomerId] = useState<string | null>(null);
  
  // Modal states
  const [showModal, setShowModal] = useState(false);
  const [editingTask, setEditingTask] = useState<TaskDto | null>(null);
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);
  const [taskToDelete, setTaskToDelete] = useState<string | null>(null);
  
  // Form state
  const [formData, setFormData] = useState<CreateTaskDto>({
    title: '',
    description: '',
    type: 'Call',
    priority: 'Medium',
    dueDate: undefined,
    startDate: undefined,
    customerId: undefined,
    contactId: undefined,
    assignedToUserId: undefined,
    createdByUserId: '00000000-0000-0000-0000-000000000000', // Placeholder - w produkcji z sesji
  });
  const [formError, setFormError] = useState<string | null>(null);
  const [formLoading, setFormLoading] = useState(false);

  useEffect(() => {
    loadTasks();
    loadCustomers();
    loadContacts();
  }, [pageNumber, searchTerm, selectedCustomerId]);

  const loadTasks = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await tasksApi.getList(
        pageNumber, 
        pageSize, 
        searchTerm || undefined,
        selectedCustomerId || undefined
      );
      setTasks(data);
    } catch (err) {
      setError('Failed to load tasks');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const loadCustomers = async () => {
    try {
      const data = await customersApi.getList(1, 100);
      setCustomers(data);
    } catch (err) {
      console.error('Failed to load customers', err);
    }
  };

  const loadContacts = async () => {
    try {
      const data = await contactsApi.getList(1, 100);
      setContacts(data);
    } catch (err) {
      console.error('Failed to load contacts', err);
    }
  };

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    setPageNumber(1);
    loadTasks();
  };

  const handleAddClick = () => {
    setEditingTask(null);
    setFormData({
      title: '',
      description: '',
      type: 'Call',
      priority: 'Medium',
      dueDate: undefined,
      startDate: undefined,
      customerId: undefined,
      contactId: undefined,
      assignedToUserId: undefined,
      createdByUserId: '00000000-0000-0000-0000-000000000000',
    });
    setFormError(null);
    setShowModal(true);
  };

  const handleEditClick = (task: TaskDto) => {
    setEditingTask(task);
    setFormData({
      title: task.title,
      description: task.description || '',
      type: task.type,
      priority: task.priority,
      dueDate: task.dueDate ? new Date(task.dueDate).toISOString().slice(0, 16) : undefined,
      startDate: task.startDate ? new Date(task.startDate).toISOString().slice(0, 16) : undefined,
      customerId: task.customerId || undefined,
      contactId: task.contactId || undefined,
      assignedToUserId: task.assignedToUserId || undefined,
      createdByUserId: task.createdByUserId,
    });
    setFormError(null);
    setShowModal(true);
  };

  const handleDeleteClick = (taskId: string) => {
    setTaskToDelete(taskId);
    setShowDeleteConfirm(true);
  };

  const handleDeleteConfirm = async () => {
    if (!taskToDelete) return;
    
    try {
      setFormLoading(true);
      await tasksApi.delete(taskToDelete);
      setShowDeleteConfirm(false);
      setTaskToDelete(null);
      loadTasks();
    } catch (err) {
      setFormError('Failed to delete task');
      console.error(err);
    } finally {
      setFormLoading(false);
    }
  };

  const handleFormSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setFormError(null);
    setFormLoading(true);

    try {
      const submitData: CreateTaskDto | UpdateTaskDto = {
        title: formData.title,
        description: formData.description || undefined,
        type: formData.type,
        priority: formData.priority,
        dueDate: formData.dueDate ? new Date(formData.dueDate).toISOString() : undefined,
        startDate: formData.startDate ? new Date(formData.startDate).toISOString() : undefined,
        customerId: formData.customerId || undefined,
        contactId: formData.contactId || undefined,
        assignedToUserId: formData.assignedToUserId || undefined,
        ...(editingTask ? {} : { createdByUserId: formData.createdByUserId }),
      };

      if (editingTask) {
        await tasksApi.update(editingTask.id, submitData as UpdateTaskDto);
      } else {
        await tasksApi.create(submitData as CreateTaskDto);
      }
      
      setShowModal(false);
      setEditingTask(null);
      loadTasks();
    } catch (err: any) {
      console.error('Update Task Error:', err);
      console.error('Response:', err.response?.data);
      const errorMessage = err.response?.data?.message 
        || err.response?.data?.title
        || err.response?.data
        || err.message
        || 'Failed to save task';
      setFormError(typeof errorMessage === 'string' ? errorMessage : JSON.stringify(errorMessage));
    } finally {
      setFormLoading(false);
    }
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    // Dla GUID pól, jeśli wartość jest pustym stringiem, ustaw na undefined
    if (name === 'customerId' || name === 'contactId' || name === 'assignedToUserId') {
      setFormData((prev) => ({ ...prev, [name]: value === '' ? undefined : value }));
    } else {
      setFormData((prev) => ({ ...prev, [name]: value }));
    }
  };

  const formatDate = (dateString?: string) => {
    if (!dateString) return '-';
    return new Date(dateString).toLocaleDateString();
  };

  if (loading) {
    return <div className="loading">Loading...</div>;
  }

  return (
    <div className="tasks-page">
      <div className="page-header">
        <h1>Tasks</h1>
        <button className="btn-primary" onClick={handleAddClick}>
          Add Task
        </button>
      </div>

      {error && <div className="error-message">{error}</div>}

      <form onSubmit={handleSearch} className="search-form">
        <input
          type="text"
          placeholder="Search tasks..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          className="search-input"
        />
        <select
          value={selectedCustomerId || ''}
          onChange={(e) => {
            setSelectedCustomerId(e.target.value || null);
            setPageNumber(1);
          }}
          className="filter-select"
        >
          <option value="">All Customers</option>
          {customers.map((customer) => (
            <option key={customer.id} value={customer.id}>
              {customer.companyName}
            </option>
          ))}
        </select>
        <button type="submit" className="btn-secondary">Search</button>
      </form>

      <div className="tasks-table">
        <table>
          <thead>
            <tr>
              <th>Title</th>
              <th>Type</th>
              <th>Priority</th>
              <th>Status</th>
              <th>Due Date</th>
              <th>Customer</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {tasks.length === 0 ? (
              <tr>
                <td colSpan={7} style={{ textAlign: 'center' }}>
                  No tasks found
                </td>
              </tr>
            ) : (
              tasks.map((task) => (
                <tr key={task.id}>
                  <td>{task.title}</td>
                  <td>{task.type}</td>
                  <td>
                    <span className={`priority-badge priority-${task.priority.toLowerCase()}`}>
                      {task.priority}
                    </span>
                  </td>
                  <td>{task.status}</td>
                  <td>{formatDate(task.dueDate)}</td>
                  <td>{task.customerId ? 'Yes' : '-'}</td>
                  <td>
                    <button 
                      className="btn-small" 
                      onClick={() => handleEditClick(task)}
                    >
                      Edit
                    </button>
                    <button 
                      className="btn-small btn-danger" 
                      onClick={() => handleDeleteClick(task.id)}
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      <div className="pagination">
        <button
          onClick={() => setPageNumber((p) => Math.max(1, p - 1))}
          disabled={pageNumber === 1}
        >
          Previous
        </button>
        <span>Page {pageNumber}</span>
        <button
          onClick={() => setPageNumber((p) => p + 1)}
          disabled={tasks.length < pageSize}
        >
          Next
        </button>
      </div>

      {/* Modal for Create/Edit */}
      {showModal && (
        <div className="modal-overlay" onClick={() => setShowModal(false)}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h2>{editingTask ? 'Edit Task' : 'Add Task'}</h2>
              <button 
                className="modal-close" 
                onClick={() => setShowModal(false)}
              >
                ×
              </button>
            </div>
            
            {formError && <div className="error-message">{formError}</div>}
            
            <form onSubmit={handleFormSubmit}>
              <div className="form-group">
                <label>Title *</label>
                <input
                  type="text"
                  name="title"
                  value={formData.title}
                  onChange={handleInputChange}
                  required
                />
              </div>
              
              <div className="form-group">
                <label>Description</label>
                <textarea
                  name="description"
                  value={formData.description}
                  onChange={handleInputChange}
                  rows={3}
                />
              </div>
              
              <div className="form-group">
                <label>Type *</label>
                <select
                  name="type"
                  value={formData.type}
                  onChange={handleInputChange}
                  required
                >
                  <option value="Call">Call</option>
                  <option value="Meeting">Meeting</option>
                  <option value="Email">Email</option>
                  <option value="FollowUp">Follow Up</option>
                  <option value="Document">Document</option>
                  <option value="Other">Other</option>
                </select>
              </div>
              
              <div className="form-group">
                <label>Priority *</label>
                <select
                  name="priority"
                  value={formData.priority}
                  onChange={handleInputChange}
                  required
                >
                  <option value="Low">Low</option>
                  <option value="Medium">Medium</option>
                  <option value="High">High</option>
                  <option value="Urgent">Urgent</option>
                </select>
              </div>
              
              <div className="form-group">
                <label>Start Date</label>
                <input
                  type="datetime-local"
                  name="startDate"
                  value={formData.startDate || ''}
                  onChange={handleInputChange}
                />
              </div>
              
              <div className="form-group">
                <label>Due Date</label>
                <input
                  type="datetime-local"
                  name="dueDate"
                  value={formData.dueDate || ''}
                  onChange={handleInputChange}
                />
              </div>
              
              <div className="form-group">
                <label>Customer</label>
                <select
                  name="customerId"
                  value={formData.customerId || ''}
                  onChange={handleInputChange}
                >
                  <option value="">None</option>
                  {customers.map((customer) => (
                    <option key={customer.id} value={customer.id}>
                      {customer.companyName}
                    </option>
                  ))}
                </select>
              </div>
              
              <div className="form-group">
                <label>Contact</label>
                <select
                  name="contactId"
                  value={formData.contactId || ''}
                  onChange={handleInputChange}
                >
                  <option value="">None</option>
                  {contacts.map((contact) => (
                    <option key={contact.id} value={contact.id}>
                      {contact.firstName} {contact.lastName}
                    </option>
                  ))}
                </select>
              </div>
              
              <div className="form-actions">
                <button 
                  type="button" 
                  className="btn-secondary" 
                  onClick={() => setShowModal(false)}
                  disabled={formLoading}
                >
                  Cancel
                </button>
                <button 
                  type="submit" 
                  className="btn-primary"
                  disabled={formLoading}
                >
                  {formLoading ? 'Saving...' : editingTask ? 'Update' : 'Create'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Delete Confirmation Modal */}
      {showDeleteConfirm && (
        <div className="modal-overlay" onClick={() => setShowDeleteConfirm(false)}>
          <div className="modal-content modal-small" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h2>Confirm Delete</h2>
              <button 
                className="modal-close" 
                onClick={() => setShowDeleteConfirm(false)}
              >
                ×
              </button>
            </div>
            
            <p>Are you sure you want to delete this task? This action cannot be undone.</p>
            
            {formError && <div className="error-message">{formError}</div>}
            
            <div className="form-actions">
              <button 
                type="button" 
                className="btn-secondary" 
                onClick={() => setShowDeleteConfirm(false)}
                disabled={formLoading}
              >
                Cancel
              </button>
              <button 
                type="button" 
                className="btn-danger" 
                onClick={handleDeleteConfirm}
                disabled={formLoading}
              >
                {formLoading ? 'Deleting...' : 'Delete'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export default TasksPage;
