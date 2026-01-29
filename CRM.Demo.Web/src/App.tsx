import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import './App.css';

// Pages
import CustomersPage from './pages/Customers/CustomersPage';
import ContactsPage from './pages/Contacts/ContactsPage';
import TasksPage from './pages/Tasks/TasksPage';
import NotesPage from './pages/Notes/NotesPage';

function App() {
  return (
    <Router>
      <div className="app">
        <nav className="navbar">
          <div className="nav-container">
            <h1 className="nav-title">CRM Demo</h1>
            <ul className="nav-links">
              <li>
                <Link to="/">Home</Link>
              </li>
              <li>
                <Link to="/customers">Customers</Link>
              </li>
              <li>
                <Link to="/contacts">Contacts</Link>
              </li>
              <li>
                <Link to="/tasks">Tasks</Link>
              </li>
              <li>
                <Link to="/notes">Notes</Link>
              </li>
            </ul>
          </div>
        </nav>

        <main className="main-content">
          <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="/customers" element={<CustomersPage />} />
            <Route path="/contacts" element={<ContactsPage />} />
            <Route path="/tasks" element={<TasksPage />} />
            <Route path="/notes" element={<NotesPage />} />
          </Routes>
        </main>
      </div>
    </Router>
  );
}

function HomePage() {
  return (
    <div style={{ padding: '2rem' }}>
      <h1>Welcome to CRM Demo</h1>
      <p>Select a module from the navigation above.</p>
    </div>
  );
}

export default App;
