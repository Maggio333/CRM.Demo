import apiClient from './api';
import type { NoteDto, CreateNoteDto, UpdateNoteDto } from '../types/models';

export const notesApi = {
  // Pobierz listę notatek
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
    const response = await apiClient.get<NoteDto[]>(`/Notes?${params.toString()}`);
    return response.data;
  },

  // Pobierz notatkę po ID
  getById: async (id: string) => {
    const response = await apiClient.get<NoteDto>(`/Notes/${id}`);
    return response.data;
  },

  // Utwórz nową notatkę
  create: async (data: CreateNoteDto) => {
    const response = await apiClient.post<string>(`/Notes`, data);
    return response.data;
  },

  // Aktualizuj notatkę
  update: async (id: string, data: UpdateNoteDto) => {
    await apiClient.put(`/Notes/${id}`, data);
  },

  // Usuń notatkę
  delete: async (id: string) => {
    await apiClient.delete(`/Notes/${id}`);
  },
};
