import { CommonModule, DatePipe } from '@angular/common';
import { Component, signal, TrackByFunction } from '@angular/core';
import { ModelUserMini } from '../../../models/modelUser';
import { AuthService } from '../../../services/authService';
import { EventService } from '../../../services/events';
import { EventParticipantListResponse } from '../../../models/modelEvents';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule, DatePipe, RouterLink],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard {
  loading = signal<boolean>(true); //Loading information status view

  user = signal<ModelUserMini[]>([]); //User information

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
            EventId: e.EventId || e.eventId,
            Name: e.Name || e.name,
            TopicName: e.TopicName || e.topicName,
            DateCreate: e.DateCreate || e.dateCreate,
            DateClose: e.DateClose || e.dateClose,
            Status: e.Status || e.status,
            Description: e.Description || e.description,
            Grade: e.Grade || e.grade,
            ParticipantDescription: e.ParticipantDescription || e.participantDescription,
          }));

          mapped.sort((a, b) => {
            const da = new Date(a.DateCreate).getTime();
            const db = new Date(b.DateCreate).getTime();
            return db - da;
          });

          this.completed.set(mapped.filter((e) => e.Status === 'Close'));
          this.enrolled.set(mapped.filter((e) => e.Status === 'Ongoing' || e.Status === 'Open'));
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
