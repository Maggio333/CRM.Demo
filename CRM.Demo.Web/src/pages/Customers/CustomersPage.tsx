import { useState, useEffect } from 'react';
import { customersApi } from '../../services/customersApi';
import type { CustomerDto, CreateCustomerDto, UpdateCustomerDto } from '../../types/models';
import './CustomersPage.css';

/**
 * CustomersPage - Komponent strony do zarządzania klientami
 * 
 * Używa React Hooks:
 * - useState: do zarządzania stanem (customers, loading, error, formData, etc.)
 * - useEffect: do automatycznego ładowania danych gdy zmienia się pageNumber lub searchTerm
 * 
 * Funkcjonalność:
 * - Lista klientów z paginacją
 * - Wyszukiwanie
 * - Dodawanie/Edytowanie/Usuwanie (przez modal)
 */
function CustomersPage() {
  // ========== STATE MANAGEMENT ==========
  // useState - hook do przechowywania danych w komponencie
  // [wartość, funkcjaDoZmiany] - destructuring
  // <CustomerDto[]> - TypeScript generic określa typ
  
  // Dane z API
  const [customers, setCustomers] = useState<CustomerDto[]>([]); // Lista klientów
  const [loading, setLoading] = useState(true); // Czy trwa ładowanie danych
  const [error, setError] = useState<string | null>(null); // Błędy z API
  
  // Paginacja i wyszukiwanie
  const [pageNumber, setPageNumber] = useState(1); // Aktualna strona
  const [pageSize] = useState(10); // Rozmiar strony (const - nie zmienia się)
  const [searchTerm, setSearchTerm] = useState(''); // Wyszukiwana fraza
  
  // Modal states - kontrolują widoczność modali
  const [showModal, setShowModal] = useState(false); // Czy pokazać modal Add/Edit
  const [editingCustomer, setEditingCustomer] = useState<CustomerDto | null>(null); // Edytowany klient (null = nowy)
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false); // Czy pokazać modal potwierdzenia usunięcia
  const [customerToDelete, setCustomerToDelete] = useState<string | null>(null); // ID klienta do usunięcia
  
  // Form state - dane formularza Add/Edit
  const [formData, setFormData] = useState<CreateCustomerDto>({
    companyName: '',
    taxId: '',
    email: '',
    phoneNumber: '',
    street: '',
    city: '',
    postalCode: '',
    country: '',
  });
  const [formError, setFormError] = useState<string | null>(null); // Błędy formularza
  const [formLoading, setFormLoading] = useState(false); // Czy trwa zapisywanie formularza

  // ========== useEffect - SIDE EFFECTS ==========
  // useEffect wykonuje się gdy:
  // - Komponent się mountuje (pierwszy render)
  // - Zmienia się pageNumber lub searchTerm (dependency array)
  // 
  // Dependency array [pageNumber, searchTerm] mówi React:
  // "Wykonaj loadCustomers() gdy pageNumber LUB searchTerm się zmieni"
  useEffect(() => {
    loadCustomers();
  }, [pageNumber, searchTerm]); // Dependency array - wykonaj gdy te wartości się zmienią

  // ========== ASYNC OPERATIONS ==========
  // async/await - asynchroniczne wywołania API
  // try/catch/finally - obsługa błędów
  const loadCustomers = async () => {
    try {
      setLoading(true); // 1. Pokaż loading spinner
      setError(null); // 2. Wyczyść poprzednie błędy
      
      // 3. Wywołaj API (czeka na odpowiedź)
      const data = await customersApi.getList(pageNumber, pageSize, searchTerm || undefined);
      
      // 4. Zaktualizuj state z danymi (wywołuje re-render)
      setCustomers(data);
    } catch (err) {
      // 5. Jeśli błąd - ustaw error state
      setError('Failed to load customers');
      console.error(err);
    } finally {
      // 6. Zawsze wykonaj (nawet przy błędzie) - ukryj loading
      setLoading(false);
    }
  };

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    setPageNumber(1);
    loadCustomers();
  };

  const handleAddClick = () => {
    setEditingCustomer(null);
    setFormData({
      companyName: '',
      taxId: '',
      email: '',
      phoneNumber: '',
      street: '',
      city: '',
      postalCode: '',
      country: '',
    });
    setFormError(null);
    setShowModal(true);
  };

  const handleEditClick = (customer: CustomerDto) => {
    setEditingCustomer(customer);
    setFormData({
      companyName: customer.companyName,
      taxId: customer.taxId,
      email: customer.email,
      phoneNumber: customer.phoneNumber || '',
      street: customer.street || '',
      city: customer.city || '',
      postalCode: customer.postalCode || '',
      country: customer.country || '',
    });
    setFormError(null);
    setShowModal(true);
  };

  const handleDeleteClick = (customerId: string) => {
    setCustomerToDelete(customerId);
    setShowDeleteConfirm(true);
  };

  const handleDeleteConfirm = async () => {
    if (!customerToDelete) return;
    
    try {
      setFormLoading(true);
      await customersApi.delete(customerToDelete);
      setShowDeleteConfirm(false);
      setCustomerToDelete(null);
      loadCustomers(); // Refresh list
    } catch (err) {
      setFormError('Failed to delete customer');
      console.error(err);
    } finally {
      setFormLoading(false);
    }
  };

  // ========== FORM SUBMIT - CREATE/UPDATE ==========
  // e.preventDefault() - zapobiega domyślnemu submit (przeładowanie strony)
  // Jeden handler dla Add i Edit - różnicujemy przez editingCustomer state
  const handleFormSubmit = async (e: React.FormEvent) => {
    e.preventDefault(); // Zapobiega domyślnemu submit (przeładowanie strony)
    setFormError(null);
    setFormLoading(true);

    try {
      if (editingCustomer) {
        // UPDATE - editingCustomer !== null oznacza edycję
        const updateData: UpdateCustomerDto = {
          companyName: formData.companyName,
          taxId: formData.taxId,
          email: formData.email,
          phoneNumber: formData.phoneNumber || undefined, // || undefined - zamień pusty string na undefined
          street: formData.street || undefined,
          city: formData.city || undefined,
          postalCode: formData.postalCode || undefined,
          country: formData.country || undefined,
        };
        await customersApi.update(editingCustomer.id, updateData);
      } else {
        // CREATE - editingCustomer === null oznacza nowy rekord
        await customersApi.create(formData);
      }
      
      // Po udanym zapisie:
      setShowModal(false); // Zamknij modal
      setEditingCustomer(null); // Wyczyść edytowany obiekt
      loadCustomers(); // Odśwież listę (reload z API)
    } catch (err: any) {
      // Obsługa błędów - wyświetl komunikat użytkownikowi
      setFormError(err.response?.data?.message || 'Failed to save customer');
      console.error(err);
    } finally {
      setFormLoading(false); // Zawsze ukryj loading
    }
  };

  // ========== FORM HANDLING - CONTROLLED COMPONENTS ==========
  // Controlled Component - React kontroluje wartość inputa przez 'value' prop
  // onChange handler aktualizuje state
  // { ...prev, [name]: value } - spread operator + computed property name
  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target; // Destructuring - wyciągnij name i value z eventu
    setFormData((prev) => ({ ...prev, [name]: value })); 
    // prev - poprzedni state
    // { ...prev } - skopiuj wszystkie właściwości
    // [name]: value - nadpisz właściwość o nazwie 'name' nową wartością
    // Przykład: name="companyName", value="Acme" → { ...prev, companyName: "Acme" }
  };

  // ========== CONDITIONAL RENDERING ==========
  // Early return - jeśli loading, zwróć spinner (nie renderuj reszty)
  if (loading) {
    return <div className="loading">Loading...</div>;
  }

  return (
    <div className="customers-page">
      <div className="page-header">
        <h1>Customers</h1>
        {/* onClick - event handler, wywołuje funkcję gdy klikniesz */}
        <button className="btn-primary" onClick={handleAddClick}>
          Add Customer
        </button>
      </div>

      {/* Conditional rendering - renderuj tylko jeśli error !== null */}
      {error && <div className="error-message">{error}</div>}

      {/* Controlled Component - value i onChange kontrolują input */}
      <form onSubmit={handleSearch} className="search-form">
        <input
          type="text"
          placeholder="Search customers..."
          value={searchTerm} // React kontroluje wartość
          onChange={(e) => setSearchTerm(e.target.value)} // Aktualizuj state przy zmianie
          className="search-input"
        />
        <button type="submit" className="btn-secondary">Search</button>
      </form>

      <div className="customers-table">
        <table>
          <thead>
            <tr>
              <th>Company Name</th>
              <th>Tax ID</th>
              <th>Email</th>
              <th>Phone</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {/* Conditional rendering - ternary operator */}
            {customers.length === 0 ? (
              // Jeśli lista pusta - pokaż komunikat
              <tr>
                <td colSpan={6} style={{ textAlign: 'center' }}>
                  No customers found
                </td>
              </tr>
            ) : (
              // Jeśli lista niepusta - renderuj każdy element
              // .map() - transformuje tablicę w tablicę JSX elementów
              // key={customer.id} - React wymaga key dla list (do optymalizacji re-renderów)
              customers.map((customer) => (
                <tr key={customer.id}>
                  <td>{customer.companyName}</td>
                  <td>{customer.taxId}</td>
                  <td>{customer.email}</td>
                  <td>{customer.phoneNumber || '-'}</td> {/* || '-' - jeśli null/undefined, pokaż '-' */}
                  <td>{customer.status}</td>
                  <td>
                    {/* Arrow function w onClick - przekazuje customer do handlera */}
                    <button 
                      className="btn-small" 
                      onClick={() => handleEditClick(customer)}
                    >
                      Edit
                    </button>
                    <button 
                      className="btn-small btn-danger" 
                      onClick={() => handleDeleteClick(customer.id)}
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

      {/* ========== PAGINATION ========== */}
      <div className="pagination">
        {/* Functional update - setPageNumber((p) => ...) używa poprzedniej wartości */}
        <button
          onClick={() => setPageNumber((p) => Math.max(1, p - 1))} // p - poprzednia wartość
          disabled={pageNumber === 1} // Disable jeśli jesteś na pierwszej stronie
        >
          Previous
        </button>
        <span>Page {pageNumber}</span>
        <button
          onClick={() => setPageNumber((p) => p + 1)} // Zwiększ o 1
          disabled={customers.length < pageSize} // Disable jeśli mniej wyników niż pageSize
        >
          Next
        </button>
      </div>

      {/* ========== MODAL - CONDITIONAL RENDERING ========== */}
      {/* {showModal && ...} - renderuj modal tylko jeśli showModal === true */}
      {/* onClick w overlay - zamknij modal gdy klikniesz poza nim */}
      {/* e.stopPropagation() - zapobiega zamknięciu gdy klikniesz w modal-content */}
      {showModal && (
        <div className="modal-overlay" onClick={() => setShowModal(false)}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              {/* Ternary operator - różny tekst dla Add vs Edit */}
              <h2>{editingCustomer ? 'Edit Customer' : 'Add Customer'}</h2>
              <button 
                className="modal-close" 
                onClick={() => setShowModal(false)} // Zamknij modal
              >
                ×
              </button>
            </div>
            
            {/* Conditional rendering - pokaż błąd tylko jeśli istnieje */}
            {formError && <div className="error-message">{formError}</div>}
            
            {/* Controlled Component - formularz */}
            <form onSubmit={handleFormSubmit}>
              <div className="form-group">
                <label>Company Name *</label>
                <input
                  type="text"
                  name="companyName" // name musi odpowiadać właściwości w formData
                  value={formData.companyName} // React kontroluje wartość
                  onChange={handleInputChange} // Aktualizuj state przy zmianie
                  required // HTML5 validation
                />
              </div>
              
              <div className="form-group">
                <label>Tax ID (NIP) *</label>
                <input
                  type="text"
                  name="taxId"
                  value={formData.taxId}
                  onChange={handleInputChange}
                  required
                  maxLength={10}
                  placeholder="10 digits"
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
                <label>Street</label>
                <input
                  type="text"
                  name="street"
                  value={formData.street}
                  onChange={handleInputChange}
                />
              </div>
              
              <div className="form-group">
                <label>City</label>
                <input
                  type="text"
                  name="city"
                  value={formData.city}
                  onChange={handleInputChange}
                />
              </div>
              
              <div className="form-group">
                <label>Postal Code</label>
                <input
                  type="text"
                  name="postalCode"
                  value={formData.postalCode}
                  onChange={handleInputChange}
                />
              </div>
              
              <div className="form-group">
                <label>Country</label>
                <input
                  type="text"
                  name="country"
                  value={formData.country}
                  onChange={handleInputChange}
                />
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
                  {formLoading ? 'Saving...' : editingCustomer ? 'Update' : 'Create'}
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
            
            <p>Are you sure you want to delete this customer? This action cannot be undone.</p>
            
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

export default CustomersPage;
