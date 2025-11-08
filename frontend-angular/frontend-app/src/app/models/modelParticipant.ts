export interface ModelListParticipants {
    Id : number,
    EntityId: number,
    Name: string,
    Email: string,
    Status: boolean,
    Grade: string,
    Comments: string,
}

export interface ModelListParticipantInfo {
    Grade: string,
    Comments: string,
    ParticipantDescription?: string | null
}