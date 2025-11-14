export interface ModelEventsRequest {
  Name: string;
  Description: string;
  CreateById: number;
  TopicsId: number;
}

export interface EventListResponse {
  Id: number;
  Name: string;
  Description: string;
  TopicName: string;
  CreateById: number;
  DateCreate: string;
  DateClose: string | null;
  Status: string;
}

export interface EventParticipantListResponse {
  Id: number;
  EventId: number;
  Name: string;
  Description: string;
  TopicName: string;
  DateCreate: string;
  DateClose: string | null;
  Status: string;
  Grade: string;
  ParticipantDescription: string;
}

export interface EventListByIdRequest {
  CreateById: number;
}
