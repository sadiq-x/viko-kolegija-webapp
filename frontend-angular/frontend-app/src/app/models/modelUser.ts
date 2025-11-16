//Model response of User Login
export interface ModelUserLoginResponse {
  EntityId: number;
  Username: string;
  RoleType: string;
}
//Model request of User Register
export interface ModelUserRegisterRequest {
  Username: string;
  PasswordHash: string;
  ConfirmPasswordHash: string;
  Name: string;
  Email: string;
  Image?: string | null;
  NumberPhone: string;
  Address: string;
  Birthday: string;
  Nationality: string;
  Gender: string;
}
//Model response of User Profile
export interface ModelUserProfileResponse {
  Id: number;
  Username: string;
  Name: string;
  Email: string;
  Image?: string | null;
  NumberPhone: string;
  Address: string;
  Birthday: string;
  Nationality: string;
  Gender: string;
}
//Model User mini to be used on Dashboard Page
export interface ModelUserMini {
  Name: string;
  Username: string;
  RoleType: string;
}

