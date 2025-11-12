import { CommonModule, DatePipe } from '@angular/common';
import { Component, signal, TrackByFunction } from '@angular/core';
import { ModelUserMini } from '../../../models/modelUser';
import { AuthService } from '../../../services/authService';
import { EventService } from '../../../services/events';
import { EventParticipantListResponse } from '../../../models/modelEvents';

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule, DatePipe],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard {
  //Loading information status view
  loading = signal<boolean>(true);

  //User info
  user = signal<ModelUserMini[]>([]);

  //Courses list
  enrolled = signal<EventParticipantListResponse[]>([]); //Courses actives
  completed = signal<EventParticipantListResponse[]>([]); //Courses completed

  constructor(private authService: AuthService, private eventService: EventService) {}

  ngOnInit(): void {
    this.getUserInfo();
    this.getCourses();
  }
  //Function to insert the username and role type of user logged
  private getUserInfo() {
    const userInfo = this.authService.getAuthUser();
    if (!userInfo) {
      this.user.set([]);
      return;
    }
    const parsed = JSON.parse(userInfo!);
    this.user.set([{ Name: parsed.name, Username: parsed.username, RoleType: parsed.roleType }]);
  }

  private getCourses() {
    this.eventService.getEventByEntityId().subscribe({
      next: (res) => {
        if (Array.isArray(res) && !!res) {
          const mapped = res.map((e: any) => ({
            Id: e.Id || e.id,
            Name: e.Name || e.name,
            TopicName: e.TopicName || e.topicName,
            CreateById: e.CreateById || e.createById,
            DateCreate: e.DateCreate || e.dateCreate,
            DateClose: e.DateClose || e.dateClose,
            Status: e.Status || e.status,
            Description: e.Description || e.Description,
            Grade: e.Grade || e.grade,
          }));
          this.completed.set(mapped.filter((e) => e.Status === 'Close'));
          this.enrolled.set(mapped.filter((e) => e.Status === 'Ongoing' || 'Open'));
          this.loading.set(false);
          return;
        }
        this.loading.set(false);
        return;
      },
    });
  }

  // trackBy para performance
  trackById: TrackByFunction<EventParticipantListResponse> = (_: number, item) => item.Id;
}
