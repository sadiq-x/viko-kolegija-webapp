export interface ModelEventsRequest {
  Name: string;
  Description: string;
  TopicsId: number;
}

export interface EventListResponse {
  Id: number;
  Name: string;
  Description: string;
  TopicName: string;
  CreateBy: number;
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
