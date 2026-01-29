import apiClient from './api';
import type { CustomerDto, CreateCustomerDto, UpdateCustomerDto } from '../types/models';

export const customersApi = {
  // Pobierz listę klientów
  getList: async (pageNumber: number = 1, pageSize: number = 10, searchTerm?: string) => {
    const params = new URLSearchParams({
      pageNumber: pageNumber.toString(),
      pageSize: pageSize.toString(),
    });
    if (searchTerm) {
      params.append('searchTerm', searchTerm);
    }
    const response = await apiClient.get<CustomerDto[]>(`/Customers?${params.toString()}`);
    return response.data;
  },

  // Pobierz klienta po ID
  getById: async (id: string) => {
    const response = await apiClient.get<CustomerDto>(`/Customers/${id}`);
    return response.data;
  },

  // Utwórz nowego klienta
  create: async (data: CreateCustomerDto) => {
    const response = await apiClient.post<string>(`/Customers`, data);
    return response.data;
  },

  // Aktualizuj klienta
  update: async (id: string, data: UpdateCustomerDto) => {
    await apiClient.put(`/Customers/${id}`, data);
  },

  // Usuń klienta
  delete: async (id: string) => {
    await apiClient.delete(`/Customers/${id}`);
  },
};
