export interface RoleResponse {
  id: string;
  companyId: string;
  companyName: string;
  name: string;
  isAdmin: boolean;
  isActive: boolean;
  createdAt: string;
  createdBy?: string;
  updatedAt?: string;
  updatedBy?: string;
}

export interface CreateRoleRequest {
  companyId: string;
  name: string;
  isAdmin?: boolean;
  createdBy?: string;
}

export interface UpdateRoleRequest {
  name: string;
  isAdmin?: boolean;
  isActive?: boolean;
  updatedBy?: string;
}
