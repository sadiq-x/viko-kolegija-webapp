export interface ModelUserLoginResponse {
    EntityId: number,
    Username: string,
    RoleType: string
};

export interface ModelUserRegisterRequest {
    Username: string,
    PasswordHash: string,
    ConfirmPasswordHash: string,
    Name: string,
    Email: string,
    Image?: string | null,
    NumberPhone: string,
    Address: string,
    Birthday: string,
    Nationality: string,
    Gender: string 
};

export interface ModelUserProfileResponse {
    Id: number,
    Username: string,
    Name: string,
    Email: string,
    Image?: string | null,
    NumberPhone: string,
    Address: string,
    Birthday: string,
    Nationality: string,
    Gender: string
};