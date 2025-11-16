import { RolesTypes } from "../data/roles";

//Model Users to be used on Admin Page
export interface ModelEntities {
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
  Role: RolesTypes
}