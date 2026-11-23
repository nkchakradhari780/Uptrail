export interface CompanyResponse {
  id: string;
  name: string;
  email: string;
  phoneNumber: string;
  address: string;
  website?: string;
  isActive: boolean;
  createdAt: string;
  createdBy?: string;
  updatedAt?: string;
  updatedBy?: string;
  activeUsersCount: number;
  activeRolesCount: number;
}

export interface CreateCompanyRequest {
  name: string;
  email: string;
  phoneNumber: string;
  address: string;
  website?: string;
  createdBy?: string;
}

export interface UpdateCompanyRequest {
  name: string;
  email: string;
  phoneNumber: string;
  address: string;
  website?: string;
  isActive?: boolean;
  updatedBy?: string;
}
