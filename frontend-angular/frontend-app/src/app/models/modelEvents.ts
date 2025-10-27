export interface ModelEventsRequest {
  Name: string;
  Description: string;
  CreateById: number; 
  DateCreate: string; 
  TopicsId: number;
};

export interface EventListResponse {
  Id: number;
  Name: string;
  Description: string;
  TopicName: string;
  CreateById: number;
  DateCreate: string;     
  Status: boolean;
}

export interface EventListByIdRequest {
  CreateById: number;
}
