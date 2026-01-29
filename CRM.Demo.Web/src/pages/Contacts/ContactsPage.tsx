import { useState, useEffect } from 'react';
import { contactsApi } from '../../services/contactsApi';
import { customersApi } from '../../services/customersApi';
import type { ContactDto, CreateContactDto, UpdateContactDto, CustomerDto } from '../../types/models';
import './ContactsPage.css';

function ContactsPage() {
  const [contacts, setContacts] = useState<ContactDto[]>([]);
  const [customers, setCustomers] = useState<CustomerDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(10);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedCustomerId, setSelectedCustomerId] = useState<string | null>(null);
  
  // Modal states
  const [showModal, setShowModal] = useState(false);
  const [editingContact, setEditingContact] = useState<ContactDto | null>(null);
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);
  const [contactToDelete, setContactToDelete] = useState<string | null>(null);
  
  // Form state
  const [formData, setFormData] = useState<CreateContactDto>({
    firstName: '',
    lastName: '',
    email: '',
    type: 'Person',
    phoneNumber: '',
    jobTitle: '',
    department: '',
    customerId: undefined,
  });
  const [formError, setFormError] = useState<string | null>(null);
  const [formLoading, setFormLoading] = useState(false);

  useEffect(() => {
    loadContacts();
    loadCustomers();
  }, [pageNumber, searchTerm, selectedCustomerId]);

  const loadContacts = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await contactsApi.getList(
        pageNumber, 
        pageSize, 
        searchTerm || undefined,
        selectedCustomerId || undefined
      );
      setContacts(data);
    } catch (err) {
      setError('Failed to load contacts');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const loadCustomers = async () => {
    try {
      const data = await customersApi.getList(1, 100); // Load all for dropdown
      setCustomers(data);
    } catch (err) {
      console.error('Failed to load customers for dropdown', err);
    }
  };

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    setPageNumber(1);
    loadContacts();
  };

  const handleAddClick = () => {
    setEditingContact(null);
    setFormData({
      firstName: '',
      lastName: '',
      email: '',
      type: 'Person',
      phoneNumber: '',
      jobTitle: '',
      department: '',
      customerId: undefined,
    });
    setFormError(null);
    setShowModal(true);
  };

  const handleEditClick = (contact: ContactDto) => {
    setEditingContact(contact);
    setFormData({
      firstName: contact.firstName,
      lastName: contact.lastName,
      email: contact.email,
      type: contact.type,
      phoneNumber: contact.phoneNumber || '',
      jobTitle: contact.jobTitle || '',
      department: contact.department || '',
      customerId: contact.customerId || undefined,
    });
    setFormError(null);
    setShowModal(true);
  };

  const handleDeleteClick = (contactId: string) => {
    setContactToDelete(contactId);
    setShowDeleteConfirm(true);
  };

  const handleDeleteConfirm = async () => {
    if (!contactToDelete) return;
    
    try {
      setFormLoading(true);
      await contactsApi.delete(contactToDelete);
      setShowDeleteConfirm(false);
      setContactToDelete(null);
      loadContacts();
    } catch (err) {
      setFormError('Failed to delete contact');
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
      if (editingContact) {
        const updateData: UpdateContactDto = {
          firstName: formData.firstName,
          lastName: formData.lastName,
          email: formData.email,
          type: formData.type,
          phoneNumber: formData.phoneNumber || undefined,
          jobTitle: formData.jobTitle || undefined,
          department: formData.department || undefined,
          customerId: formData.customerId && formData.customerId !== '' ? formData.customerId : undefined,
        };
        
        console.log('Sending update data:', updateData);
        await contactsApi.update(editingContact.id, updateData);
      } else {
        await contactsApi.create(formData);
      }
      
      setShowModal(false);
      setEditingContact(null);
      loadContacts();
    } catch (err: any) {
      // Loguj pełny błąd dla debugowania
      console.error('Update Contact Error:', err);
      console.error('Response:', err.response?.data);
      console.error('Status:', err.response?.status);
      
      // Wyświetl szczegółowy komunikat błędu
      const errorMessage = err.response?.data?.message 
        || err.response?.data?.title
        || err.response?.data
        || err.message
        || 'Failed to save contact';
      
      setFormError(typeof errorMessage === 'string' ? errorMessage : JSON.stringify(errorMessage));
    } finally {
      setFormLoading(false);
    }
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    // Dla customerId, jeśli wartość jest pustym stringiem, ustaw na undefined
    if (name === 'customerId') {
      setFormData((prev) => ({ 
        ...prev, 
        [name]: value === '' ? undefined : (value as string)
      }));
    } else {
      setFormData((prev) => ({ ...prev, [name]: value }));
    }
  };

  if (loading) {
    return <div className="loading">Loading...</div>;
  }

  return (
    <div className="contacts-page">
      <div className="page-header">
        <h1>Contacts</h1>
        <button className="btn-primary" onClick={handleAddClick}>
          Add Contact
        </button>
      </div>

      {error && <div className="error-message">{error}</div>}

      <form onSubmit={handleSearch} className="search-form">
        <input
          type="text"
          placeholder="Search contacts..."
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

      <div className="contacts-table">
        <table>
          <thead>
            <tr>
              <th>Name</th>
              <th>Email</th>
              <th>Type</th>
              <th>Phone</th>
              <th>Job Title</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {contacts.length === 0 ? (
              <tr>
                <td colSpan={7} style={{ textAlign: 'center' }}>
                  No contacts found
                </td>
              </tr>
            ) : (
              contacts.map((contact) => (
                <tr key={contact.id}>
                  <td>{contact.firstName} {contact.lastName}</td>
                  <td>{contact.email}</td>
                  <td>{contact.type}</td>
                  <td>{contact.phoneNumber || '-'}</td>
                  <td>{contact.jobTitle || '-'}</td>
                  <td>{contact.status}</td>
                  <td>
                    <button 
                      className="btn-small" 
                      onClick={() => handleEditClick(contact)}
                    >
                      Edit
                    </button>
                    <button 
                      className="btn-small btn-danger" 
                      onClick={() => handleDeleteClick(contact.id)}
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
          disabled={contacts.length < pageSize}
        >
          Next
        </button>
      </div>

      {/* Modal for Create/Edit */}
      {showModal && (
        <div className="modal-overlay" onClick={() => setShowModal(false)}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h2>{editingContact ? 'Edit Contact' : 'Add Contact'}</h2>
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
                <label>First Name *</label>
                <input
                  type="text"
                  name="firstName"
                  value={formData.firstName}
                  onChange={handleInputChange}
                  required
                />
              </div>
              
              <div className="form-group">
                <label>Last Name *</label>
                <input
                  type="text"
                  name="lastName"
                  value={formData.lastName}
                  onChange={handleInputChange}
                  required
                />
              </div>
              
              <div className="form-group">
                <label>Email *</label>
                <input
                  type="email"
                  name="email"
                  value={formData.email}
                  onChange={handleInputChange}
                  required
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
                  <option value="Person">Person</option>
                  <option value="Company">Company</option>
                </select>
              </div>
              
              <div className="form-group">
                <label>Phone Number</label>
                <input
                  type="text"
                  name="phoneNumber"
                  value={formData.phoneNumber}
                  onChange={handleInputChange}
                  placeholder="9 digits"
                />
              </div>
              
              <div className="form-group">
                <label>Job Title</label>
                <input
                  type="text"
                  name="jobTitle"
                  value={formData.jobTitle}
                  onChange={handleInputChange}
                />
              </div>
              
              <div className="form-group">
                <label>Department</label>
                <input
                  type="text"
                  name="department"
                  value={formData.department}
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
                  {formLoading ? 'Saving...' : editingContact ? 'Update' : 'Create'}
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
            
            <p>Are you sure you want to delete this contact? This action cannot be undone.</p>
            
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

export default ContactsPage;
