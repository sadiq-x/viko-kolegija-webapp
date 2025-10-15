export interface ModelUserLogin {
    EntityId: number,
    Username: string
}

export interface ModelUserProfile {
    Id: number,
    Username: string,
    Name: string,
    Email: string,
    Image?: string | null,
    NumberPhone: string,
    Address: string 
}