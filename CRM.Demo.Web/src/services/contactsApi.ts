import apiClient from './api';
import type { ContactDto, CreateContactDto, UpdateContactDto } from '../types/models';

export const contactsApi = {
  // Pobierz listę kontaktów
  getList: async (pageNumber: number = 1, pageSize: number = 10, searchTerm?: string, customerId?: string) => {
    const params = new URLSearchParams({
      pageNumber: pageNumber.toString(),
      pageSize: pageSize.toString(),
    });
    if (searchTerm) {
      params.append('searchTerm', searchTerm);
    }
    if (customerId) {
      params.append('customerId', customerId);
    }
    const response = await apiClient.get<ContactDto[]>(`/Contacts?${params.toString()}`);
    return response.data;
  },

  // Pobierz kontakt po ID
  getById: async (id: string) => {
    const response = await apiClient.get<ContactDto>(`/Contacts/${id}`);
    return response.data;
  },

  // Utwórz nowy kontakt
  create: async (data: CreateContactDto) => {
    const response = await apiClient.post<string>(`/Contacts`, data);
    return response.data;
  },

  // Aktualizuj kontakt
  update: async (id: string, data: UpdateContactDto) => {
    await apiClient.put(`/Contacts/${id}`, data);
  },

  // Usuń kontakt
  delete: async (id: string) => {
    await apiClient.delete(`/Contacts/${id}`);
  },
};
