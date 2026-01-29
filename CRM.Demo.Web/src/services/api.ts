import axios from 'axios';

// Base URL dla API
// W Dockerze (produkcja): używa względnej ścieżki /api (proxy przez nginx)
// Lokalnie (development): używa VITE_API_BASE_URL lub domyślnie localhost:5000
const getApiBaseUrl = () => {
    // Jeśli jest ustawiona zmienna środowiskowa, użyj jej
    if (import.meta.env.VITE_API_BASE_URL) {
        return import.meta.env.VITE_API_BASE_URL;
    }
    
    // W produkcji (Docker build) użyj względnej ścieżki (nginx proxy)
    // import.meta.env.PROD jest true gdy Vite build jest w production mode
    if (import.meta.env.PROD) {
        return '/api';
    }
    
    // W development użyj localhost:5000 (API w Dockerze)
    return 'http://localhost:5000/api';
};

const API_BASE_URL = getApiBaseUrl();

// Debug log (tylko w development)
if (!import.meta.env.PROD) {
    console.log('API Base URL:', API_BASE_URL);
}

// Utworzenie instancji Axios z konfiguracją
const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Interceptor dla błędów (opcjonalnie)
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    console.error('API Error:', error);
    return Promise.reject(error);
  }
);

export default apiClient;
