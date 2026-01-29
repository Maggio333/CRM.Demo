import { useState, useEffect } from 'react';
import { notesApi } from '../../services/notesApi';
import { customersApi } from '../../services/customersApi';
import { contactsApi } from '../../services/contactsApi';
import { tasksApi } from '../../services/tasksApi';
import type { NoteDto, CreateNoteDto, UpdateNoteDto, CustomerDto, ContactDto, TaskDto } from '../../types/models';
import './NotesPage.css';

function NotesPage() {
  const [notes, setNotes] = useState<NoteDto[]>([]);
  const [customers, setCustomers] = useState<CustomerDto[]>([]);
  const [contacts, setContacts] = useState<ContactDto[]>([]);
  const [tasks, setTasks] = useState<TaskDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(10);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedCustomerId, setSelectedCustomerId] = useState<string | null>(null);
  
  // Modal states
  const [showModal, setShowModal] = useState(false);
  const [editingNote, setEditingNote] = useState<NoteDto | null>(null);
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);
  const [noteToDelete, setNoteToDelete] = useState<string | null>(null);
  
  // Form state
  const [formData, setFormData] = useState<CreateNoteDto>({
    content: '',
    title: '',
    type: 'General',
    category: '',
    customerId: undefined,
    contactId: undefined,
    taskId: undefined,
    createdByUserId: '00000000-0000-0000-0000-000000000000', // Placeholder
  });
  const [formError, setFormError] = useState<string | null>(null);
  const [formLoading, setFormLoading] = useState(false);

  useEffect(() => {
    loadNotes();
    loadCustomers();
    loadContacts();
    loadTasks();
  }, [pageNumber, searchTerm, selectedCustomerId]);

  const loadNotes = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await notesApi.getList(
        pageNumber, 
        pageSize, 
        searchTerm || undefined,
        selectedCustomerId || undefined
      );
      setNotes(data);
    } catch (err) {
      setError('Failed to load notes');
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

  const loadTasks = async () => {
    try {
      const data = await tasksApi.getList(1, 100);
      setTasks(data);
    } catch (err) {
      console.error('Failed to load tasks', err);
    }
  };

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    setPageNumber(1);
    loadNotes();
  };

  const handleAddClick = () => {
    setEditingNote(null);
    setFormData({
      content: '',
      title: '',
      type: 'General',
      category: '',
      customerId: undefined,
      contactId: undefined,
      taskId: undefined,
      createdByUserId: '00000000-0000-0000-0000-000000000000',
    });
    setFormError(null);
    setShowModal(true);
  };

  const handleEditClick = (note: NoteDto) => {
    setEditingNote(note);
    setFormData({
      content: note.content,
      title: note.title || '',
      type: note.type,
      category: note.category || '',
      customerId: note.customerId || undefined,
      contactId: note.contactId || undefined,
      taskId: note.taskId || undefined,
      createdByUserId: note.createdByUserId,
    });
    setFormError(null);
    setShowModal(true);
  };

  const handleDeleteClick = (noteId: string) => {
    setNoteToDelete(noteId);
    setShowDeleteConfirm(true);
  };

  const handleDeleteConfirm = async () => {
    if (!noteToDelete) return;
    
    try {
      setFormLoading(true);
      await notesApi.delete(noteToDelete);
      setShowDeleteConfirm(false);
      setNoteToDelete(null);
      loadNotes();
    } catch (err) {
      setFormError('Failed to delete note');
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
      const submitData: CreateNoteDto | UpdateNoteDto = {
        content: formData.content,
        title: formData.title || undefined,
        type: formData.type,
        category: formData.category || undefined,
        customerId: formData.customerId || undefined,
        contactId: formData.contactId || undefined,
        taskId: formData.taskId || undefined,
        ...(editingNote ? {} : { createdByUserId: formData.createdByUserId }),
      };

      if (editingNote) {
        await notesApi.update(editingNote.id, submitData as UpdateNoteDto);
      } else {
        await notesApi.create(submitData as CreateNoteDto);
      }
      
      setShowModal(false);
      setEditingNote(null);
      loadNotes();
    } catch (err: any) {
      console.error('Update Note Error:', err);
      console.error('Response:', err.response?.data);
      const errorMessage = err.response?.data?.message 
        || err.response?.data?.title
        || err.response?.data
        || err.message
        || 'Failed to save note';
      setFormError(typeof errorMessage === 'string' ? errorMessage : JSON.stringify(errorMessage));
    } finally {
      setFormLoading(false);
    }
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    // Dla GUID pól, jeśli wartość jest pustym stringiem, ustaw na undefined
    if (name === 'customerId' || name === 'contactId' || name === 'taskId') {
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
    <div className="notes-page">
      <div className="page-header">
        <h1>Notes</h1>
        <button className="btn-primary" onClick={handleAddClick}>
          Add Note
        </button>
      </div>

      {error && <div className="error-message">{error}</div>}

      <form onSubmit={handleSearch} className="search-form">
        <input
          type="text"
          placeholder="Search notes..."
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

      <div className="notes-table">
        <table>
          <thead>
            <tr>
              <th>Title</th>
              <th>Type</th>
              <th>Content Preview</th>
              <th>Category</th>
              <th>Created</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {notes.length === 0 ? (
              <tr>
                <td colSpan={6} style={{ textAlign: 'center' }}>
                  No notes found
                </td>
              </tr>
            ) : (
              notes.map((note) => (
                <tr key={note.id}>
                  <td>{note.title || '-'}</td>
                  <td>{note.type}</td>
                  <td className="content-preview">
                    {note.content.length > 50 
                      ? `${note.content.substring(0, 50)}...` 
                      : note.content}
                  </td>
                  <td>{note.category || '-'}</td>
                  <td>{formatDate(note.createdAt)}</td>
                  <td>
                    <button 
                      className="btn-small" 
                      onClick={() => handleEditClick(note)}
                    >
                      Edit
                    </button>
                    <button 
                      className="btn-small btn-danger" 
                      onClick={() => handleDeleteClick(note.id)}
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
          disabled={notes.length < pageSize}
        >
          Next
        </button>
      </div>

      {/* Modal for Create/Edit */}
      {showModal && (
        <div className="modal-overlay" onClick={() => setShowModal(false)}>
          <div className="modal-content modal-large" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h2>{editingNote ? 'Edit Note' : 'Add Note'}</h2>
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
                <label>Content *</label>
                <textarea
                  name="content"
                  value={formData.content}
                  onChange={handleInputChange}
                  required
                  rows={6}
                />
              </div>
              
              <div className="form-group">
                <label>Title</label>
                <input
                  type="text"
                  name="title"
                  value={formData.title}
                  onChange={handleInputChange}
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
                  <option value="General">General</option>
                  <option value="Call">Call</option>
                  <option value="Meeting">Meeting</option>
                  <option value="Email">Email</option>
                  <option value="FollowUp">Follow Up</option>
                  <option value="Other">Other</option>
                </select>
              </div>
              
              <div className="form-group">
                <label>Category</label>
                <input
                  type="text"
                  name="category"
                  value={formData.category}
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
              
              <div className="form-group">
                <label>Task</label>
                <select
                  name="taskId"
                  value={formData.taskId || ''}
                  onChange={handleInputChange}
                >
                  <option value="">None</option>
                  {tasks.map((task) => (
                    <option key={task.id} value={task.id}>
                      {task.title}
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
                  {formLoading ? 'Saving...' : editingNote ? 'Update' : 'Create'}
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
            
            <p>Are you sure you want to delete this note? This action cannot be undone.</p>
            
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

export default NotesPage;
