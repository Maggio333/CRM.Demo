import apiClient from './api';
import type { TaskDto, CreateTaskDto, UpdateTaskDto } from '../types/models';

export const tasksApi = {
  // Pobierz listę zadań
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
    const response = await apiClient.get<TaskDto[]>(`/Tasks?${params.toString()}`);
    return response.data;
  },

  // Pobierz zadanie po ID
  getById: async (id: string) => {
    const response = await apiClient.get<TaskDto>(`/Tasks/${id}`);
    return response.data;
  },

  // Utwórz nowe zadanie
  create: async (data: CreateTaskDto) => {
    const response = await apiClient.post<string>(`/Tasks`, data);
    return response.data;
  },

  // Aktualizuj zadanie
  update: async (id: string, data: UpdateTaskDto) => {
    await apiClient.put(`/Tasks/${id}`, data);
  },

  // Usuń zadanie
  delete: async (id: string) => {
    await apiClient.delete(`/Tasks/${id}`);
  },
};
