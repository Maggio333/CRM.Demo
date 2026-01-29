// DTOs z API - zgodne z backendem

export interface CustomerDto {
  id: string;
  companyName: string;
  taxId: string;
  email: string;
  phoneNumber?: string;
  street?: string;
  city?: string;
  postalCode?: string;
  country?: string;
  status: string;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateCustomerDto {
  companyName: string;
  taxId: string;
  email: string;
  phoneNumber?: string;
  street?: string;
  city?: string;
  postalCode?: string;
  country?: string;
}

export interface UpdateCustomerDto {
  companyName?: string;
  taxId?: string;
  email?: string;
  phoneNumber?: string;
  street?: string;
  city?: string;
  postalCode?: string;
  country?: string;
}

export interface ContactDto {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  type: string;
  phoneNumber?: string;
  jobTitle?: string;
  department?: string;
  customerId?: string;
  status: string;
  role?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateContactDto {
  firstName: string;
  lastName: string;
  email: string;
  type: string;
  phoneNumber?: string;
  jobTitle?: string;
  department?: string;
  customerId?: string;
}

export interface UpdateContactDto {
  firstName?: string;
  lastName?: string;
  email?: string;
  type?: string;
  phoneNumber?: string;
  jobTitle?: string;
  department?: string;
  customerId?: string;
}

export interface TaskDto {
  id: string;
  title: string;
  description?: string;
  type: string;
  priority: string;
  status: string;
  dueDate?: string;
  startDate?: string;
  completedDate?: string;
  customerId?: string;
  contactId?: string;
  assignedToUserId?: string;
  createdAt: string;
  createdByUserId: string;
  updatedAt?: string;
}

export interface CreateTaskDto {
  title: string;
  description?: string;
  type: string;
  priority: string;
  dueDate?: string;
  startDate?: string;
  customerId?: string;
  contactId?: string;
  assignedToUserId?: string;
  createdByUserId: string;
}

export interface UpdateTaskDto {
  title?: string;
  description?: string;
  type?: string;
  priority?: string;
  dueDate?: string;
  startDate?: string;
  customerId?: string;
  contactId?: string;
  assignedToUserId?: string;
}

export interface NoteDto {
  id: string;
  content: string;
  title?: string;
  type: string;
  category?: string;
  customerId?: string;
  contactId?: string;
  taskId?: string;
  createdAt: string;
  createdByUserId: string;
  updatedAt?: string;
  updatedByUserId?: string;
}

export interface CreateNoteDto {
  content: string;
  title?: string;
  type: string;
  category?: string;
  customerId?: string;
  contactId?: string;
  taskId?: string;
  createdByUserId: string;
}

export interface UpdateNoteDto {
  content?: string;
  title?: string;
  type?: string;
  category?: string;
  customerId?: string;
  contactId?: string;
  taskId?: string;
}
